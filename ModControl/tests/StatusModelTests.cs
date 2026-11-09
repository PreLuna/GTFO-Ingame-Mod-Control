using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Core;

static class StatusModelTests
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

    static bool Contains(IReadOnlyList<string> lines, string text)
    {
        foreach (var line in lines) if (line.Contains(text, StringComparison.Ordinal)) return true;
        return false;
    }

    static ModControlStatus Status(bool initialized = true, bool verified = true, string lastError = "")
        => new ModControlStatus(
            ModControlInfo.Version,
            ModControlInfo.ExpectedArchiveVersion,
            initialized,
            verified,
            true,
            "成功，框架已完成初始化",
            new[]
            {
                new InstalledPlugin("PreLuna.EnemyHealthBar", "Enemy Health Bar", "0.3.0", ""),
                new InstalledPlugin("PreLuna.ThirdPersonHUD", "Third Person HUD", "0.13.0", "")
            },
            lastError);

    static int Main(string[] args)
    {
        Case("overview-shows-read-only-framework-and-counts", () =>
        {
            var lines = Status().OverviewLines();
            Check(Contains(lines, ModControlInfo.Version), "module version");
            Check(Contains(lines, "0.0.838"), "archive version");
            Check(Contains(lines, "2 个"), "target count");
            Check(Contains(lines, "白名单即时自动保存"), "save mode");
            Check(Contains(lines, "已初始化"), "framework initialized");
        });

        Case("overview-reports-uninitialized-and-unverified", () =>
        {
            var lines = Status(false, false).OverviewLines();
            Check(Contains(lines, "未初始化"), "uninitialized state");
            Check(Contains(lines, "未验证"), "unverified state");
        });

        Case("diagnostics-show-registration-and-last-error", () =>
        {
            var lines = Status(lastError: "测试错误").DiagnosticsLines();
            Check(Contains(lines, "Archive 模块注册：成功"), "registration");
            Check(Contains(lines, "0.0.838"), "archive version");
            Check(Contains(lines, "最近错误：测试错误"), "last error");
        });

        var output = args.Length > 0 ? args[0] : "StatusModelTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
