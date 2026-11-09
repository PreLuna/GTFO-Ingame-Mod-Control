using System;
using System.Collections.Generic;
using System.Globalization;
using PreLuna.ModControl.Core.Operations;

namespace PreLuna.ModControl.Features
{
    /// <summary>
    /// Chinese text shared by all target pages and diagnostics. Keep target status and
    /// operation outcome words here so a new page cannot accidentally leak English enum names.
    /// </summary>
    internal static class ModControlText
    {
        internal static string TargetStatusText(TargetStatus status)
        {
            switch (status)
            {
                case TargetStatus.Available: return "可用";
                case TargetStatus.Missing: return "未安装";
                case TargetStatus.Incompatible: return "版本不兼容";
                case TargetStatus.Uninitialized: return "未初始化";
                case TargetStatus.Destroyed: return "已销毁";
                default: return "未知状态";
            }
        }

        internal static string Outcome(OperationOutcome outcome)
        {
            switch (outcome)
            {
                case OperationOutcome.Success: return "成功";
                case OperationOutcome.ValidationRejected: return "校验拒绝";
                case OperationOutcome.PermissionDenied: return "权限拒绝";
                case OperationOutcome.StateConflict: return "状态冲突";
                case OperationOutcome.CapabilityUnsupported: return "能力不支持";
                case OperationOutcome.TargetUnavailable: return "目标不可用";
                case OperationOutcome.SaveFailed: return "保存失败";
                case OperationOutcome.Reentrant: return "操作重叠";
                default: return "未知结果";
            }
        }

        internal static string Describe(OperationResult result)
        {
            if (result == null) return "没有操作结果。";
            if (result.Succeeded)
                return string.IsNullOrWhiteSpace(result.Message) ? "已保存" : result.Message;
            return Outcome(result.Outcome) + "：" + (string.IsNullOrWhiteSpace(result.Message) ? "未提供原因" : result.Message);
        }

        internal static string FormatValues(IReadOnlyDictionary<string, SettingValue> values)
        {
            if (values == null || values.Count == 0) return "无";
            var parts = new List<string>();
            foreach (var pair in values) parts.Add(pair.Key + "：" + FormatValue(pair.Value));
            return string.Join("  ", parts.ToArray());
        }

        internal static string FormatValue(SettingValue value)
        {
            if (value == null) return "未知";
            if (value.TryGetBoolean(out var boolean)) return boolean ? "是" : "否";
            if (value.TryGetInteger(out var integer)) return integer.ToString(CultureInfo.InvariantCulture);
            if (value.TryGetNumber(out var number)) return number.ToString("0.###", CultureInfo.InvariantCulture);
            return value.ToString();
        }

        internal static string YesNo(bool value) => value ? "是" : "否";

        internal static string MissingPage(string displayName, string expectedVersion)
            => displayName + "：未加载" + (string.IsNullOrWhiteSpace(expectedVersion) ? "目标插件" : " " + expectedVersion);
    }
}
