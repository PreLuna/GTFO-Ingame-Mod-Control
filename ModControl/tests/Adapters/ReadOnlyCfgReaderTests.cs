using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PreLuna.ModControl.Adapters;
using PreLuna.ModControl.Core.Operations;

static class ReadOnlyCfgReaderTests
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
        var directory = Path.Combine(Path.GetTempPath(), "preluna-readonly-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return directory;
    }

    static ReadOnlyCfgSnapshot ReadWith(string fileName, string content, ReadOnlyCfgTarget target)
    {
        var directory = TempDirectory();
        File.WriteAllText(Path.Combine(directory, fileName), content);
        BepInEx.Paths.ConfigPath = directory;
        return ReadOnlyCfgReader.Read(target);
    }

    static int Main(string[] args)
    {
        Case("reads-flashlight-selected-values", () =>
        {
            var snapshot = ReadWith("PreLuna.FlashlightCustom.cfg",
                "[01 - General]\nEnabled = false\nMode = CASUAL\nAttachLightToWeaponBone = true\n" +
                "[02 - Core Layer (RF_Core)]\nAngleStep = 42\n" +
                "[05 - Helmet Synth (melee/tools) — all modes]\nAngle = 70\nIntensity = 0.5\nRange = 12.5\n" +
                "[06 - Perlin Flicker (continuous tremor)]\nEnabled = true\n" +
                "[07 - Rare Flicker (bad-contact bursts)]\nEnabled = false\n" +
                "[08 - Horror Flicker (long dark pause)]\nEnabled = true\n",
                ReadOnlyCfgTargets.Flashlight);
            Check(snapshot.Descriptor.Status == TargetStatus.Available, "status");
            Check((snapshot.Descriptor.Capabilities & TargetCapabilities.ReadOnly) != 0, "read-only");
            Check(!snapshot.Descriptor.PermissionGranted, "permission");
            Check(snapshot.Values.Count == 10, "count");
            Check(snapshot.Values[0] == "否", "enabled");
            Check(snapshot.Values[1] == "CASUAL", "mode");
            Check(snapshot.Values[3] == "42", "angle");
            Check(snapshot.Values[6] == "12.5", "range");
            Check(snapshot.Values[9] == "是", "horror");
        });

        Case("keeps-same-key-in-different-sections", () =>
        {
            var snapshot = ReadWith("PreLuna.CoopBotsCustom.cfg",
                "[Bots Open Containers]\nEnabled = false\n" +
                "[Bots Teleport]\nEnabled = true\n",
                ReadOnlyCfgTargets.CoopBots);
            Check(snapshot.Values[6] == "否", "containers");
            Check(snapshot.Values[7] == "是", "teleport");
            Check(snapshot.Values[8] == "未知", "missing key");
        });

        Case("missing-config-is-unavailable", () =>
        {
            var directory = TempDirectory();
            BepInEx.Paths.ConfigPath = directory;
            var snapshot = ReadOnlyCfgReader.Read(ReadOnlyCfgTargets.PrisonerHealth);
            Check(snapshot.Descriptor.Status == TargetStatus.Uninitialized, "status");
            Check(!snapshot.Descriptor.PermissionGranted, "permission");
            Check(snapshot.Values.All(value => value == "不可用"), "values");
        });

        Case("reads-captain-chinese-keys", () =>
        {
            var snapshot = ReadWith("PreLuna.Captain.cfg",
                "[机器人强化剂]\n启用 = false\n" +
                "[机器人任务]\n启用 = true\n任务传送 = true\n" +
                "[中文显示]\n通讯菜单 = false\n" +
                "[自动拾取]\n机器人收集距离 = 9.5\n",
                ReadOnlyCfgTargets.Captain);
            Check(snapshot.Descriptor.Status == TargetStatus.Available, "status");
            Check(snapshot.Values[0] == "否", "robot booster");
            Check(snapshot.Values[2] == "是", "task teleport");
            Check(snapshot.Values[6] == "否", "chinese menu");
            Check(snapshot.Values[7] == "9.5", "pickup range");
        });

        Case("selected-target-shape", () =>
        {
            Check(ReadOnlyCfgTargets.Flashlight.Fields.Count == 10, "flashlight");
            Check(ReadOnlyCfgTargets.PrisonerHealth.Fields.Count == 7, "health");
            Check(ReadOnlyCfgTargets.BioScanner.Fields.Count == 5, "bio");
            Check(ReadOnlyCfgTargets.CoopBots.Fields.Count == 10, "coopbots");
            Check(ReadOnlyCfgTargets.Captain.Fields.Count == 8, "captain");
            Check(ReadOnlyCfgTargets.Flashlight.Id == "PreLuna.FlashlightCustom", "id");
        });

        var output = args.Length > 0 ? args[0] : "ReadOnlyCfgReaderTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
