using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Adapters;
using PreLuna.ModControl.Core.Operations;

static class PrisonerMovementReadOnlyAdapterTests
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

    static string TempDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), "preluna-prisoner-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return directory;
    }

    static int Main(string[] args)
    {
        Case("reads-three-frozen-switches-as-read-only", () =>
        {
            var directory = TempDirectory();
            var previous = BepInEx.Paths.ConfigPath;
            try
            {
                BepInEx.Paths.ConfigPath = directory;
                File.WriteAllText(Path.Combine(directory, "PreLuna.PrisonerMovementCustom.cfg"),
                    "[APEX: Prisoners]\nAllow Recoil Reduce = false\nAllow Super Slide = true\nAllow Run-Reload = false\n");
                var snapshot = new PrisonerMovementReadOnlyAdapter().ReadSnapshot();
                Check(snapshot.Descriptor.Status == TargetStatus.Available, "status");
                Check((snapshot.Descriptor.Capabilities & TargetCapabilities.ReadOnly) != 0, "read-only capability");
                Check(!snapshot.Descriptor.PermissionGranted, "permission");
                Check(snapshot.TryGetEffective("recoil", out var recoil) && recoil.TryGetBoolean(out var recoilValue) && !recoilValue, "recoil");
                Check(snapshot.TryGetEffective("superSlide", out var slide) && slide.TryGetBoolean(out var slideValue) && slideValue, "slide");
                Check(snapshot.TryGetEffective("runReload", out var reload) && reload.TryGetBoolean(out var reloadValue) && !reloadValue, "reload");
            }
            finally
            {
                BepInEx.Paths.ConfigPath = previous;
            }
        });

        Case("missing-config-is-unavailable", () =>
        {
            var directory = TempDirectory();
            var previous = BepInEx.Paths.ConfigPath;
            try
            {
                BepInEx.Paths.ConfigPath = directory;
                var snapshot = new PrisonerMovementReadOnlyAdapter().ReadSnapshot();
                Check(snapshot.Descriptor.Status == TargetStatus.Uninitialized, "status");
                Check(!snapshot.Descriptor.PermissionGranted, "permission");
            }
            finally
            {
                BepInEx.Paths.ConfigPath = previous;
            }
        });

        var output = args.Length > 0 ? args[0] : "PrisonerMovementReadOnlyAdapterTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
