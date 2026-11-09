using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PreLuna.ModControl.Adapters;
using PreLuna.ModControl.Core.Operations;
using PreLuna.ModControl.Core.Storage;
using TheArchive.Core.FeaturesAPI.Components;
using TheArchive.Core.FeaturesAPI.Settings;

namespace PreLuna.ModControl.Features
{
    /// <summary>
    /// Direct CFG page for frozen modules that expose no runtime control API.
    /// Writes only the selected local CFG lines; the target reads them on next launch.
    /// </summary>
    internal sealed class RestartCfgTarget
    {
        internal RestartCfgTarget(
            string id,
            string displayName,
            string version,
            string fileName,
            string notice,
            IReadOnlyList<RestartCfgField> fields)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DisplayName = displayName ?? id;
            Version = version ?? "frozen";
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Notice = notice ?? "重启后生效：只写本地 CFG，不提供运行控制接口。";
            Fields = fields ?? Array.Empty<RestartCfgField>();
        }

        internal string Id { get; }
        internal string DisplayName { get; }
        internal string Version { get; }
        internal string FileName { get; }
        internal string Notice { get; }
        internal IReadOnlyList<RestartCfgField> Fields { get; }
    }

    internal sealed class RestartCfgField
    {
        internal RestartCfgField(
            string key,
            string section,
            string configKey,
            string label,
            SettingKind kind,
            SettingValue defaultValue,
            double? minimum = null,
            double? maximum = null)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Section = section ?? string.Empty;
            ConfigKey = configKey ?? throw new ArgumentNullException(nameof(configKey));
            Label = label ?? key;
            Definition = new SettingDefinition(key, label, kind, defaultValue, minimum, maximum);
        }

        internal string Key { get; }
        internal string Section { get; }
        internal string ConfigKey { get; }
        internal string Label { get; }
        internal SettingDefinition Definition { get; }
    }

    public interface IRestartCfgSettings : IWritableTargetSettings
    {
        FLabel Notice { get; }
    }

    internal sealed class RestartCfgPage<TSettings> : ITargetPage
        where TSettings : class, IRestartCfgSettings
    {
        readonly RestartCfgTarget target;
        readonly IReadOnlyDictionary<string, string> keyMap;
        string revision = string.Empty;
        string resultMessage = string.Empty;

        internal RestartCfgPage(RestartCfgTarget target, IReadOnlyDictionary<string, string> keyMap)
        {
            this.target = target ?? throw new ArgumentNullException(nameof(target));
            this.keyMap = keyMap ?? throw new ArgumentNullException(nameof(keyMap));
        }

        public Type SettingsType => typeof(TSettings);

        public void Refresh(object settings)
        {
            if (settings is not TSettings typed) return;
            var path = Path.Combine(BepInEx.Paths.ConfigPath, target.FileName);
            if (!File.Exists(path))
            {
                ModControlPageSupport.SetLabel(typed.Status, target.DisplayName + "：未找到 " + target.FileName);
                ModControlPageSupport.SetLabel(typed.Notice, target.Notice);
                ModControlPageSupport.SetLabel(typed.Result, resultMessage);
                ModControlPageSupport.SetLabel(typed.CurrentValues, "当前生效：不可用");
                ModControlPageSupport.SetLabel(typed.SavedValues, "已保存：不可用");
                return;
            }

            revision = ConfigFileRevisionStore.Capture(path);
            var values = new Dictionary<string, SettingValue>(StringComparer.Ordinal);
            foreach (var field in target.Fields)
            {
                values[field.Key] = ReadValue(path, field) ?? field.Definition.DefaultValue;
            }

            var descriptor = new TargetDescriptor(
                target.Id,
                target.DisplayName,
                target.Version,
                TargetStatus.Available,
                TargetCapabilities.SavePersistent,
                target.Fields.Select(field => field.Definition).ToArray(),
                permissionGranted: true,
                permissionReason: target.Notice);
            var snapshot = new TargetSnapshot(descriptor, revision, values, values);
            typed.ApplySnapshot(snapshot);
            ModControlPageSupport.SetLabel(typed.Status, target.DisplayName + " / 冻结 CFG / 重启生效");
            ModControlPageSupport.SetLabel(typed.Notice, target.Notice);
            ModControlPageSupport.SetLabel(typed.Result, resultMessage);
            ModControlPageSupport.SetLabel(typed.CurrentValues, "当前 CFG：" + ModControlText.FormatValues(values));
            ModControlPageSupport.SetLabel(typed.SavedValues, "重启后读取该 CFG");
        }

        public void OnSettingChanged(FeatureSetting setting, object settings)
        {
            if (setting == null || settings is not TSettings typed) return;
            var property = setting.Prop?.Name;
            if (string.IsNullOrWhiteSpace(property) || !keyMap.TryGetValue(property, out var key))
            {
                Refresh(typed);
                return;
            }

            var field = target.Fields.FirstOrDefault(item => string.Equals(item.Key, key, StringComparison.Ordinal));
            if (field == null)
            {
                Refresh(typed);
                return;
            }

            var value = TargetValueReader.FromSetting(setting);
            var issue = value == null ? new ValidationIssue(key, "缺少值") : field.Definition.Validate(value);
            if (issue != null)
            {
                resultMessage = "未保存：" + issue.Message;
                Refresh(typed);
                return;
            }

            var path = Path.Combine(BepInEx.Paths.ConfigPath, target.FileName);
            try
            {
                revision = ConfigFileRevisionStore.Save(
                    path,
                    revision,
                    new[] { new ConfigFileWrite(field.Section, field.ConfigKey, Format(value)) });
                resultMessage = "已写入本地 CFG；重启游戏后生效。";
            }
            catch (ConfigFileConflictException)
            {
                resultMessage = "CFG 已被外部修改，未覆盖；请刷新后再试。";
            }
            catch (Exception error)
            {
                resultMessage = "保存失败：" + error.Message;
            }

            Refresh(typed);
        }

        public void OnOpened()
        {
        }

        public void OnClosed()
        {
        }

        static SettingValue ReadValue(string path, RestartCfgField field)
        {
            if (!TryRead(path, field.Section, field.ConfigKey, out var raw)) return null;
            raw = raw.Trim();
            if (field.Definition.Kind == SettingKind.Boolean)
                return bool.TryParse(raw, out var boolean) ? SettingValue.Boolean(boolean) : null;
            return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
                ? SettingValue.Number(number)
                : null;
        }

        static string Format(SettingValue value)
        {
            if (value.TryGetBoolean(out var boolean)) return boolean ? "true" : "false";
            if (value.TryGetNumber(out var number)) return number.ToString("0.#####", CultureInfo.InvariantCulture);
            if (value.TryGetInteger(out var integer)) return integer.ToString(CultureInfo.InvariantCulture);
            return string.Empty;
        }

        static bool TryRead(string path, string section, string key, out string value)
        {
            value = string.Empty;
            var inSection = string.IsNullOrWhiteSpace(section);
            foreach (var line in File.ReadLines(path))
            {
                var text = line.Trim();
                if (text.Length == 0 || text[0] == '#' || text[0] == ';') continue;
                if (text.Length >= 2 && text[0] == '[' && text[text.Length - 1] == ']')
                {
                    inSection = string.IsNullOrWhiteSpace(section) ||
                                string.Equals(text.Substring(1, text.Length - 2), section, StringComparison.OrdinalIgnoreCase);
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
