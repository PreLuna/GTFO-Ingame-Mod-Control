using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Core.Keybinds;
using PreLuna.ModControl.Core.Operations;

static class KeybindSessionTests
{
    sealed record Result(string Name, bool Passed, string Error);
    static readonly List<Result> Results = new();
    static void Check(bool value, string message = "assertion failed") { if (!value) throw new Exception(message); }
    static void Case(string name, Action test)
    {
        try { test(); Results.Add(new(name, true, null)); }
        catch (Exception e) { Results.Add(new(name, false, e.ToString())); }
    }

    sealed class Target : ITargetProvider
    {
        internal int Code = 288, Writes;
        internal bool Old, Throw, Fail, Conflict, ReadOnly;
        public TargetSnapshot ReadSnapshot()
        {
            if (Throw) throw new Exception("target read failed");
            var values = new Dictionary<string, SettingValue>();
            if (!Old) values["key.toggle"] = SettingValue.Integer(Code);
            return new TargetSnapshot(new TargetDescriptor("target", "Target", "1", TargetStatus.Available,
                TargetCapabilities.Validate | TargetCapabilities.SavePersistent,
                Old ? Array.Empty<SettingDefinition>() : new[] {
                    new SettingDefinition("key.toggle", "Toggle", SettingKind.Integer, SettingValue.Integer(288), 0, 509)
                }, permissionGranted: !ReadOnly), "1", values, values);
        }
        public ValidationResult Validate(ChangeSet changes, TargetSnapshot snapshot) => ValidationResult.Valid();
        public ProviderResult SetPersistent(ChangeSet changes, TargetSnapshot snapshot, string revision)
        {
            if (Conflict) return ProviderResult.Conflict("external edit");
            if (Fail) return ProviderResult.Failed("disk full");
            Writes++;
            changes.Values["key.toggle"].TryGetInteger(out Code);
            return ProviderResult.Success(ReadSnapshot());
        }
    }

    static KeybindSession Session(Target first, Target second)
        => new(new[] { new KeybindAction("First", "A", "key.toggle", "First"), new KeybindAction("Second", "B", "key.toggle", "Second") },
            new Dictionary<string, Func<ITargetProvider>> { ["A"] = () => first, ["B"] = () => second });

    static int Main(string[] args)
    {
        Case("cross-target-duplicates-are-advisory-and-saving-is-allowed", () => {
            var a = new Target(); var b = new Target { Code = 289 }; var s = Session(a, b); s.Refresh();
            Check(s.Duplicates().Count == 0);
            Check(s.Set("Second", 288).Contains("已保存"));
            Check(s.Duplicates().Count == 1 && s.Duplicates()[0].Actions.Count == 2);
            Check(a.Writes == 0 && b.Writes == 1);
        });
        Case("none-clears-without-counting-as-conflict", () => {
            var a = new Target(); var b = new Target(); var s = Session(a, b); s.Refresh();
            s.Set("First", 0); s.Set("Second", 0);
            Check(s.Duplicates().Count == 0 && s.Values["First"] == 0 && a.Code == 0);
        });
        Case("missing-and-older-targets-do-not-create-writable-defaults", () => {
            var a = new Target { Old = true }; var s = Session(a, null); s.Refresh();
            Check(s.Values.Count == 0 && s.Set("First", 288).Contains("未保存"));
            Check(a.Writes == 0);
        });
        Case("one-broken-target-does-not-hide-the-other", () => {
            var s = Session(new Target { Throw = true }, new Target()); s.Refresh();
            Check(s.Values.Count == 1 && s.Values.ContainsKey("Second"));
        });
        Case("save-failure-and-external-conflict-keep-effective-values", () => {
            var a = new Target { Fail = true }; var s = Session(a, null); s.Refresh();
            Check(s.Set("First", 289).Contains("disk full") && s.Values["First"] == 288);
            a.Fail = false; a.Conflict = true;
            Check(s.Set("First", 289).Contains("external edit") && s.Values["First"] == 288 && a.Writes == 0);
        });
        Case("read-only-and-invalid-bindings-cannot-write", () => {
            var a = new Target { ReadOnly = true }; var s = Session(a, null); s.Refresh();
            s.Set("First", 289); a.ReadOnly = false; s.Set("First", -1); s.Set("missing", 288);
            Check(a.Writes == 0 && a.Code == 288);
        });
        Case("target-arrival-removal-and-runtime-rebinding-refresh-correctly", () => {
            Target live = null;
            var s = new KeybindSession(new[] { new KeybindAction("First", "A", "key.toggle", "First") },
                new Dictionary<string, Func<ITargetProvider>> { ["A"] = () => live });
            s.Refresh(); Check(s.Values.Count == 0);
            live = new Target(); s.Refresh(); Check(s.Values["First"] == 288);
            live.Code = 289; s.Refresh(); Check(s.Values["First"] == 289);
            live.Throw = true; s.Refresh(); Check(s.Values.Count == 0);
        });
        if (args.Length > 0) File.WriteAllText(args[0], JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        foreach (var r in Results) Console.WriteLine((r.Passed ? "PASS " : "FAIL ") + r.Name + " " + r.Error);
        return Results.Exists(r => !r.Passed) ? 1 : 0;
    }
}
