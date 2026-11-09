using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Core;

static class TargetCatalogTests
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
        Case("filters-non-preluna-and-self", () =>
        {
            var targets = TargetCatalog.SelectPreLunaTargets(new[]
            {
                new InstalledPlugin("dev.AuriRex.gtfo.TheArchive", "TheArchive", "0.0.838", ""),
                new InstalledPlugin("PreLuna.ModControl", "ModControl", "0.1.0", ""),
                new InstalledPlugin("PreLuna.EnemyHealthBar", "EHB", "0.3.0", ""),
                new InstalledPlugin("Hikaria.BoosterTweaker", "Booster", "1.0.0", "")
            });
            Check(targets.Count == 1, "expected one PreLuna target");
            Check(targets[0].Guid == "PreLuna.EnemyHealthBar", "wrong target");
        });

        Case("deduplicates-guid-case-insensitively", () =>
        {
            var targets = TargetCatalog.SelectPreLunaTargets(new[]
            {
                new InstalledPlugin("PreLuna.EnemyHealthBar", "first", "0.3.0", ""),
                new InstalledPlugin("preluna.enemyhealthbar", "duplicate", "0.2.0", "")
            });
            Check(targets.Count == 1, "duplicate GUID should be ignored");
            Check(targets[0].Version == "0.3.0", "first value should win");
        });

        Case("sorts-by-guid", () =>
        {
            var targets = TargetCatalog.SelectPreLunaTargets(new[]
            {
                new InstalledPlugin("PreLuna.ThirdPersonHUD", "HUD", "0.13.0", ""),
                new InstalledPlugin("PreLuna.EnemyHealthBar", "EHB", "0.3.0", ""),
                new InstalledPlugin("PreLuna.Captain", "Captain", "0.6.0", "")
            });
            Check(targets.Count == 3, "expected three targets");
            Check(targets[0].Guid == "PreLuna.Captain", "first target");
            Check(targets[1].Guid == "PreLuna.EnemyHealthBar", "second target");
            Check(targets[2].Guid == "PreLuna.ThirdPersonHUD", "third target");
        });

        Case("handles-null-and-empty-entries", () =>
        {
            var targets = TargetCatalog.SelectPreLunaTargets(null);
            Check(targets.Count == 0, "null input should be empty");
            var mixed = TargetCatalog.SelectPreLunaTargets(new InstalledPlugin[]
            {
                null,
                new InstalledPlugin(null, null, null, null),
                new InstalledPlugin("  ", "blank", "", ""),
                new InstalledPlugin("PreLuna.EnemyHealthBar", null, null, null)
            });
            Check(mixed.Count == 1, "only the valid PreLuna entry should remain");
            Check(mixed[0].Name == string.Empty, "null name should be normalized");
            Check(mixed[0].Version == string.Empty, "null version should be normalized");
        });

        var output = args.Length > 0 ? args[0] : "TargetCatalogTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
