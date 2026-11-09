using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Adapters;
using PreLuna.ModControl.Core.Operations;

static class CaptainChineseAdapterTests
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
        Case("factory-finds-target-and-maps-two-settings", () =>
        {
            var adapter = CaptainChineseAdapterFactory.TryCreate();
            Check(adapter != null, "adapter");
            var snapshot = adapter.ReadSnapshot();
            Check(snapshot.Descriptor.Status == TargetStatus.Available, "status");
            Check(snapshot.Descriptor.Version == "0.5.4", "version");
            Check(snapshot.Descriptor.Settings.Count == 2, "settings");
            Check(snapshot.Revision == "3", "revision");
            Check(snapshot.TryGetEffective("chineseMenus", out var value) && value.TryGetBoolean(out var enabled) && enabled, "chineseMenus");
        });

        Case("set-persistent-maps-success-snapshot", () =>
        {
            var adapter = CaptainChineseAdapterFactory.TryCreate();
            var snapshot = adapter.ReadSnapshot();
            var changes = new ChangeSet("3", new Dictionary<string, SettingValue> { ["chineseItems"] = SettingValue.Boolean(false) });
            var result = adapter.SetPersistent(changes, snapshot, "3");
            Check(result.Outcome == ProviderOutcome.Success, "outcome");
            Check(result.Snapshot != null && result.Snapshot.Revision == "4", "revision");
        });

        var output = args.Length > 0 ? args[0] : "CaptainChineseAdapterTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
