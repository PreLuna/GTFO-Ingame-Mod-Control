using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Core.Operations;
using PreLuna.ModControl.Features;

static class TargetPageTextTests
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
        Case("localizes-target-statuses", () =>
        {
            Check(ModControlText.TargetStatusText(TargetStatus.Available) == "可用", "available");
            Check(ModControlText.TargetStatusText(TargetStatus.Missing) == "未安装", "missing");
            Check(ModControlText.TargetStatusText(TargetStatus.Incompatible) == "版本不兼容", "incompatible");
            Check(ModControlText.TargetStatusText(TargetStatus.Uninitialized) == "未初始化", "uninitialized");
            Check(ModControlText.TargetStatusText(TargetStatus.Destroyed) == "已销毁", "destroyed");
        });

        Case("localizes-operation-outcomes", () =>
        {
            Check(ModControlText.Outcome(OperationOutcome.Success) == "成功", "success");
            Check(ModControlText.Outcome(OperationOutcome.StateConflict) == "状态冲突", "conflict");
            Check(ModControlText.Outcome(OperationOutcome.SaveFailed) == "保存失败", "save");
        });

        Case("formats-snapshot-values-in-chinese", () =>
        {
            var values = new Dictionary<string, SettingValue>(StringComparer.Ordinal)
            {
                ["open"] = SettingValue.Boolean(true),
                ["count"] = SettingValue.Integer(3),
                ["scale"] = SettingValue.Number(1.25)
            };
            var text = ModControlText.FormatValues(values);
            Check(text.Contains("open：是"), "boolean");
            Check(text.Contains("count：3"), "integer");
            Check(text.Contains("scale：1.25"), "number");
            Check(ModControlText.YesNo(false) == "否", "yes/no");
        });

        Case("describes-success-and-failure", () =>
        {
            Check(ModControlText.Describe(new OperationResult(OperationOutcome.Success, "已保存")) == "已保存", "success");
            Check(ModControlText.Describe(new OperationResult(OperationOutcome.ValidationRejected, "范围错误"))
                == "校验拒绝：范围错误", "failure");
            Check(ModControlText.Describe(null) == "没有操作结果。", "null");
        });

        var output = args.Length > 0 ? args[0] : "TargetPageTextTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
