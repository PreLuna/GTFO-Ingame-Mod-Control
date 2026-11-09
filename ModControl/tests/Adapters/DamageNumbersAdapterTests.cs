using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Adapters;
using PreLuna.ModControl.Core.Operations;

static class DamageNumbersAdapterTests
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
        Case("factory-finds-target-and-maps-six-settings", () =>
        {
            var adapter = DamageNumbersAdapterFactory.TryCreate();
            Check(adapter != null, "adapter");
            var snapshot = adapter.ReadSnapshot();
            Check(snapshot.Descriptor.Status == TargetStatus.Available, "status");
            Check(snapshot.Descriptor.Version == "0.1.1", "version");
            Check(snapshot.Descriptor.Settings.Count == 6, "settings");
            Check(snapshot.Revision == "4", "revision");
            Check(snapshot.TryGetEffective("fontSize", out var value) && value.TryGetNumber(out var font) && font == 24, "font");
        });

        Case("set-persistent-maps-success-snapshot", () =>
        {
            var adapter = DamageNumbersAdapterFactory.TryCreate();
            var snapshot = adapter.ReadSnapshot();
            var changes = new ChangeSet("4", new Dictionary<string, SettingValue> { ["lifetime"] = SettingValue.Number(1.2) });
            var result = adapter.SetPersistent(changes, snapshot, "4");
            Check(result.Outcome == ProviderOutcome.Success, "outcome");
            Check(result.Snapshot != null && result.Snapshot.Revision == "5", "revision");
        });

        var output = args.Length > 0 ? args[0] : "DamageNumbersAdapterTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
