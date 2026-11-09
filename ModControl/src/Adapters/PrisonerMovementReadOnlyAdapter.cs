using System;
using System.Collections.Generic;
using System.IO;
using PreLuna.ModControl.Core.Operations;
using PreLuna.ModControl.Core.Storage;

namespace PreLuna.ModControl.Adapters
{
    // Frozen APEX module: read the existing CFG only. There is no control API and no write path.
    internal sealed class PrisonerMovementReadOnlyAdapter : ITargetProvider
    {
        internal const string TargetId = "PreLuna.PrisonerMovementCustom";
        const string FileName = "PreLuna.PrisonerMovementCustom.cfg";
        const string Section = "APEX: Prisoners";

        static readonly SettingDefinition[] Definitions =
        {
            new SettingDefinition("recoil", "允许后坐力修正", SettingKind.Boolean, SettingValue.Boolean(true)),
            new SettingDefinition("superSlide", "允许超级滑铲", SettingKind.Boolean, SettingValue.Boolean(true)),
            new SettingDefinition("runReload", "允许奔跑换弹", SettingKind.Boolean, SettingValue.Boolean(true))
        };

        static readonly string[] Keys = { "Allow Recoil Reduce", "Allow Super Slide", "Allow Run-Reload" };

        public TargetSnapshot ReadSnapshot()
        {
            var path = Path.Combine(BepInEx.Paths.ConfigPath, FileName);
            if (!File.Exists(path))
            {
                return Unavailable(TargetStatus.Uninitialized, "未找到冻结模块配置文件");
            }

            var values = new Dictionary<string, SettingValue>(StringComparer.Ordinal);
            for (var index = 0; index < Definitions.Length; index++)
            {
                if (TryRead(path, Section, Keys[index], out var raw) && bool.TryParse(raw.Trim(), out var boolean))
                    values[Definitions[index].Key] = SettingValue.Boolean(boolean);
                else
                    values[Definitions[index].Key] = Definitions[index].DefaultValue;
            }

            var revision = ConfigFileRevisionStore.Capture(path);
            var descriptor = new TargetDescriptor(
                TargetId,
                "PreLuna 移动与生命（只读）",
                "frozen",
                TargetStatus.Available,
                TargetCapabilities.ReadOnly,
                Definitions,
                permissionGranted: false,
                permissionReason: "冻结模块只读：未提供运行控制接口");
            return new TargetSnapshot(descriptor, revision, values, values);
        }

        public ValidationResult Validate(ChangeSet changes, TargetSnapshot snapshot)
            => ValidationResult.Invalid(new ValidationIssue(string.Empty, "冻结模块只读，未提供控制接口"));

        public ProviderResult SetPersistent(ChangeSet changes, TargetSnapshot snapshot, string expectedRevision)
            => ProviderResult.Unavailable("冻结模块只读，未提供控制接口");

        static TargetSnapshot Unavailable(TargetStatus status, string reason)
        {
            var descriptor = new TargetDescriptor(
                TargetId,
                "PreLuna 移动与生命（只读）",
                "frozen",
                status,
                TargetCapabilities.ReadOnly,
                Definitions,
                permissionGranted: false,
                permissionReason: reason);
            return new TargetSnapshot(descriptor, string.Empty, null, null);
        }

        static bool TryRead(string path, string section, string key, out string value)
        {
            value = string.Empty;
            var inSection = false;
            foreach (var line in File.ReadLines(path))
            {
                var text = line.Trim();
                if (text.Length == 0 || text[0] == '#' || text[0] == ';') continue;
                if (text.Length >= 2 && text[0] == '[' && text[text.Length - 1] == ']')
                {
                    inSection = string.Equals(text.Substring(1, text.Length - 2), section, StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (!inSection) continue;
                var equals = text.IndexOf('=');
                if (equals <= 0) continue;
                if (string.Equals(text.Substring(0, equals).Trim(), key, StringComparison.OrdinalIgnoreCase))
                {
                    value = text.Substring(equals + 1).Trim();
                    return true;
                }
            }

            return false;
        }
    }
}
