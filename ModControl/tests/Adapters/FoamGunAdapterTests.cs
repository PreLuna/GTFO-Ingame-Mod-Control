using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Adapters;
using PreLuna.ModControl.Core.Operations;

static class FoamGunAdapterTests
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
        Case("factory-finds-target-and-maps-five-settings", () =>
        {
            var adapter = FoamGunAdapterFactory.TryCreate();
            Check(adapter != null, "adapter");
            var snapshot = adapter.ReadSnapshot();
            Check(snapshot.Descriptor.Status == TargetStatus.Available, "status");
            Check(snapshot.Descriptor.Version == "1.0.2", "version");
            Check(snapshot.Descriptor.Settings.Count == 5, "settings");
            Check(snapshot.Revision == "5", "revision");
            Check(snapshot.TryGetEffective("launchForce", out var value) && value.TryGetNumber(out var force) && force == 32, "force");
        });

        Case("set-persistent-maps-success-snapshot", () =>
        {
            var adapter = FoamGunAdapterFactory.TryCreate();
            var snapshot = adapter.ReadSnapshot();
            var changes = new ChangeSet("5", new Dictionary<string, SettingValue> { ["shotInterval"] = SettingValue.Number(.12) });
            var result = adapter.SetPersistent(changes, snapshot, "5");
            Check(result.Outcome == ProviderOutcome.Success, "outcome");
            Check(result.Snapshot != null && result.Snapshot.Revision == "6", "revision");
        });

        var output = args.Length > 0 ? args[0] : "FoamGunAdapterTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
