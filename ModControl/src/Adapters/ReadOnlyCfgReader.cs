using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using PreLuna.ModControl.Core.Operations;
using PreLuna.ModControl.Core.Storage;

namespace PreLuna.ModControl.Adapters
{
    /// <summary>
    /// Generic parser for frozen targets whose only exposed contract is an existing BepInEx
    /// CFG. It never writes the file; it returns a read-only descriptor and formatted rows.
    /// </summary>
    internal static class ReadOnlyCfgReader
    {
        internal static ReadOnlyCfgSnapshot Read(ReadOnlyCfgTarget target)
        {
            var path = Path.Combine(BepInEx.Paths.ConfigPath, target.FileName);
            if (!File.Exists(path))
            {
                return Unavailable(target, TargetStatus.Uninitialized, "未找到冻结模块配置文件");
            }

            var values = new List<string>(target.Fields.Count);
            foreach (var field in target.Fields)
                values.Add(ReadValue(path, field));

            var descriptor = new TargetDescriptor(
                target.Id,
                target.DisplayName,
                target.Version,
                TargetStatus.Available,
                TargetCapabilities.ReadOnly,
                Array.Empty<SettingDefinition>(),
                permissionGranted: false,
                permissionReason: target.PermissionReason);
            return new ReadOnlyCfgSnapshot(descriptor, ConfigFileRevisionStore.Capture(path), values);
        }

        static ReadOnlyCfgSnapshot Unavailable(ReadOnlyCfgTarget target, TargetStatus status, string reason)
        {
            var values = new List<string>(target.Fields.Count);
            foreach (var _ in target.Fields) values.Add("不可用");
            var descriptor = new TargetDescriptor(
                target.Id,
                target.DisplayName,
                target.Version,
                status,
                TargetCapabilities.ReadOnly,
                Array.Empty<SettingDefinition>(),
                permissionGranted: false,
                permissionReason: reason);
            return new ReadOnlyCfgSnapshot(descriptor, string.Empty, values);
        }

        static string ReadValue(string path, ReadOnlyCfgField field)
        {
            if (!TryRead(path, field.Section, field.Key, out var raw)) return "未知";
            raw = raw.Trim();
            switch (field.Format)
            {
                case ReadOnlyCfgFormat.Boolean:
                    return bool.TryParse(raw, out var boolean) ? (boolean ? "是" : "否") : raw;
                case ReadOnlyCfgFormat.Number:
                    return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
                        ? number.ToString("0.###", CultureInfo.InvariantCulture)
                        : raw;
                case ReadOnlyCfgFormat.Integer:
                    return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer)
                        ? integer.ToString(CultureInfo.InvariantCulture)
                        : raw;
                default:
                    return raw;
            }
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
