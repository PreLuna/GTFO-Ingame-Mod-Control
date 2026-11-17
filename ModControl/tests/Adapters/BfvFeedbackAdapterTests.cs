using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Adapters;
using PreLuna.ModControl.Core.Operations;

static class BfvFeedbackAdapterTests
{
    sealed record Result(string Name, bool Passed, string Error);
    static readonly List<Result> Results = new();

    static void Check(bool value, string message = "assertion failed")
    {
        if (!value) throw new Exception(message);
    }

    static void Case(string name, Action test)
    {
        try { test(); Results.Add(new(name, true, null)); }
        catch (Exception error) { Results.Add(new(name, false, error.ToString())); }
    }

    static int Main(string[] args)
    {
        Case("factory-maps-bool-number-string", () =>
        {
            var adapter = BfvFeedbackAdapterFactory.TryCreate();
            Check(adapter != null, "adapter");
            var snapshot = adapter.ReadSnapshot();
            Check(snapshot.Descriptor.Status == TargetStatus.Available, "status");
            Check(snapshot.Descriptor.Version == "0.2.0", "version");
            Check(snapshot.Descriptor.Settings.Count == 3, "settings");
            Check(snapshot.Descriptor.Find("enabled")?.Kind == SettingKind.Boolean, "bool");
            Check(snapshot.Descriptor.Find("styleIndex")?.Kind == SettingKind.Number, "number");
            var color = snapshot.Descriptor.Find("colorNormal");
            Check(color?.Kind == SettingKind.String, "string kind");
            Check(snapshot.TryGetEffective("colorNormal", out var value) && value.TryGetString(out var hex) && hex == "#FFFFFF", "string value");
        });

        Case("set-persistent-maps-string-change", () =>
        {
            var adapter = BfvFeedbackAdapterFactory.TryCreate();
            var snapshot = adapter.ReadSnapshot();
            var changes = new ChangeSet("4", new Dictionary<string, SettingValue>
            {
                ["colorNormal"] = SettingValue.String("#FF8C00"),
                ["styleIndex"] = SettingValue.Number(2)
            });
            var result = adapter.SetPersistent(changes, snapshot, "4");
            Check(result.Outcome == ProviderOutcome.Success, "outcome");
            Check(result.Snapshot != null && result.Snapshot.Revision == "5", "revision");
        });

        var output = args.Length > 0 ? args[0] : "BfvFeedbackAdapterTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
