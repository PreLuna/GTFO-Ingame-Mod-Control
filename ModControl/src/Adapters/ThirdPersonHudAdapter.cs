using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using PreLuna.ModControl.Core.Operations;

namespace PreLuna.ModControl.Adapters
{
    // The HUD target is resolved by name only. ModControl must keep working when the HUD
    // is absent or older than the control API, so this adapter does not compile against it.
    internal static class ThirdPersonHudAdapterFactory
    {
        internal static ThirdPersonHudAdapter TryCreate()
        {
            var type = FindApiType();
            if (type == null) return null;

            var snapshot = type.GetMethod("GetSnapshotJson", BindingFlags.Public | BindingFlags.Static, null, Type.EmptyTypes, null);
            var validate = type.GetMethod("ValidateJson", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
            var save = type.GetMethod("SetPersistentJson", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
            if (snapshot == null || validate == null || save == null) return null;
            return new ThirdPersonHudAdapter(snapshot, validate, save);
        }

        static Type FindApiType()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var type = assembly.GetType("PreLuna.ThirdPersonHUD.Control.ThirdPersonHudControlApi", false);
                    if (type != null) return type;
                }
                catch
                {
                    // Broken/unloaded assemblies must not hide the rest of the target list.
                }
            }

            return null;
        }
    }

    internal sealed class ThirdPersonHudAdapter : ITargetProvider
    {
        static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        readonly MethodInfo getSnapshot;
        readonly MethodInfo validate;
        readonly MethodInfo setPersistent;

        internal ThirdPersonHudAdapter(MethodInfo getSnapshot, MethodInfo validate, MethodInfo setPersistent)
        {
            this.getSnapshot = getSnapshot;
            this.validate = validate;
            this.setPersistent = setPersistent;
        }

        public TargetSnapshot ReadSnapshot()
        {
            var payload = ReadPayload();
            return payload == null ? Unavailable(TargetStatus.Missing) : MapSnapshot(payload);
        }

        public ValidationResult Validate(ChangeSet changes, TargetSnapshot snapshot)
        {
            var changeJson = BuildChangeJson(changes);
            if (changeJson == null) return ValidationResult.Invalid(new ValidationIssue(string.Empty, "无法构造 HUD 变更集"));
            var result = InvokeResult(validate, changeJson);
            if (result == null) return ValidationResult.Invalid(new ValidationIssue(string.Empty, "第三人称 HUD 未返回校验结果"));
            if (result.Outcome == 0 || result.Outcome == 1) return ValidationResult.Valid();
            return ValidationResult.Invalid(MapIssues(result));
        }

        public ProviderResult SetPersistent(ChangeSet changes, TargetSnapshot snapshot, string expectedRevision)
        {
            var changeJson = BuildChangeJson(changes);
            if (changeJson == null) return ProviderResult.Failed("无法构造 HUD 变更集");
            var result = InvokeResult(setPersistent, changeJson);
            if (result == null) return ProviderResult.Failed("第三人称 HUD 未返回保存结果");
            switch (result.Outcome)
            {
                case 0:
                case 1:
                    return ProviderResult.Success(MapSnapshot(result.Snapshot), string.IsNullOrEmpty(result.Message) ? "已保存" : result.Message);
                case 3:
                    return ProviderResult.Conflict(string.IsNullOrEmpty(result.Message) ? "HUD 配置已被外部修改" : result.Message);
                case 4:
                    return ProviderResult.Unavailable(string.IsNullOrEmpty(result.Message) ? "第三人称 HUD 当前不可用" : result.Message);
                default:
                    return ProviderResult.Failed(string.IsNullOrEmpty(result.Message) ? "HUD 保存失败" : result.Message, MapIssues(result));
            }
        }

        HudSnapshotJson ReadPayload()
        {
            try
            {
                var json = getSnapshot.Invoke(null, null) as string;
                if (string.IsNullOrWhiteSpace(json)) return null;
                return JsonSerializer.Deserialize<HudSnapshotJson>(json, JsonOptions);
            }
            catch
            {
                return null;
            }
        }

        static HudResultJson InvokeResult(MethodInfo method, string json)
        {
            try
            {
                var raw = method.Invoke(null, new object[] { json }) as string;
                if (string.IsNullOrWhiteSpace(raw)) return null;
                return JsonSerializer.Deserialize<HudResultJson>(raw, JsonOptions);
            }
            catch
            {
                return null;
            }
        }

        static string BuildChangeJson(ChangeSet changes)
        {
            try
            {
                var dto = new HudChangeJson
                {
                    ExpectedRevision = changes?.ExpectedRevision ?? string.Empty,
                    Values = new Dictionary<string, HudValueJson>(StringComparer.Ordinal)
                };
                if (changes != null)
                {
                    foreach (var pair in changes.Values)
                    {
                        var value = ToJson(pair.Value);
                        if (value == null) return null;
                        dto.Values[pair.Key] = value;
                    }
                }

                return JsonSerializer.Serialize(dto);
            }
            catch
            {
                return null;
            }
        }

        static TargetSnapshot MapSnapshot(HudSnapshotJson snapshot)
        {
            if (snapshot == null) return Unavailable(TargetStatus.Missing);
            var settings = new List<SettingDefinition>();
            if (snapshot.Settings != null)
            {
                foreach (var setting in snapshot.Settings)
                {
                    var kind = MapKind(setting.Kind);
                    var value = MapValue(setting.DefaultValue);
                    if (value == null) continue;
                    settings.Add(new SettingDefinition(setting.Key, setting.DisplayName, kind, value, setting.Minimum, setting.Maximum));
                }
            }

            var descriptor = new TargetDescriptor(
                snapshot.TargetId ?? "PreLuna.ThirdPersonHUD",
                snapshot.DisplayName ?? "PreLuna 第三人称 HUD",
                snapshot.Version ?? string.Empty,
                MapStatus(snapshot.Status),
                MapCapabilities(snapshot.Capabilities),
                settings);
            return new TargetSnapshot(descriptor, snapshot.Revision, MapValues(snapshot.Saved), MapValues(snapshot.Effective));
        }

        static TargetSnapshot Unavailable(TargetStatus status)
            => new TargetSnapshot(
                new TargetDescriptor("PreLuna.ThirdPersonHUD", "PreLuna 第三人称 HUD", string.Empty, status, TargetCapabilities.None,
                    Array.Empty<SettingDefinition>()),
                string.Empty,
                null,
                null);

        static TargetStatus MapStatus(int status)
        {
            switch (status)
            {
                case 0: return TargetStatus.Available;
                case 1: return TargetStatus.Uninitialized;
                case 3: return TargetStatus.Incompatible;
                default: return TargetStatus.Missing;
            }
        }

        static TargetCapabilities MapCapabilities(int capabilities)
        {
            var mapped = TargetCapabilities.None;
            if ((capabilities & 1) != 0) mapped |= TargetCapabilities.Validate;
            if ((capabilities & 2) != 0) mapped |= TargetCapabilities.SavePersistent;
            return mapped;
        }

        static SettingKind MapKind(int kind)
            => kind == 0 ? SettingKind.Boolean : kind == 2 ? SettingKind.Integer : SettingKind.Number;

        static SettingValue MapValue(HudValueJson value)
        {
            if (value == null) return null;
            switch (value.Kind)
            {
                case 0:
                    return value.Boolean.HasValue ? SettingValue.Boolean(value.Boolean.Value) : null;
                case 1:
                    return value.Number.HasValue ? SettingValue.Number(value.Number.Value) : null;
                case 2:
                    return value.Integer.HasValue ? SettingValue.Integer(value.Integer.Value) : null;
                default:
                    return null;
            }
        }

        static IDictionary<string, SettingValue> MapValues(Dictionary<string, HudValueJson> source)
        {
            var values = new Dictionary<string, SettingValue>(StringComparer.Ordinal);
            if (source != null)
            {
                foreach (var pair in source)
                {
                    var value = MapValue(pair.Value);
                    if (value != null) values[pair.Key] = value;
                }
            }

            return values;
        }

        static HudValueJson ToJson(SettingValue value)
        {
            if (value == null) return null;
            switch (value.Kind)
            {
                case SettingKind.Boolean:
                    return value.TryGetBoolean(out var boolean) ? new HudValueJson { Kind = 0, Boolean = boolean } : null;
                case SettingKind.Number:
                    return value.TryGetNumber(out var number) ? new HudValueJson { Kind = 1, Number = number } : null;
                case SettingKind.Integer:
                    return value.TryGetInteger(out var integer) ? new HudValueJson { Kind = 2, Integer = integer } : null;
                default:
                    return null;
            }
        }

        static IReadOnlyList<ValidationIssue> MapIssues(HudResultJson result)
        {
            var issues = new List<ValidationIssue>();
            if (result?.Issues != null)
                foreach (var issue in result.Issues)
                    issues.Add(new ValidationIssue(issue.Key, issue.Message));
            if (issues.Count == 0)
                issues.Add(new ValidationIssue(string.Empty, result?.Message ?? "第三人称 HUD 操作失败"));
            return issues;
        }

        sealed class HudChangeJson
        {
            public string ExpectedRevision { get; set; }
            public Dictionary<string, HudValueJson> Values { get; set; }
        }

        sealed class HudSnapshotJson
        {
            public string TargetId { get; set; }
            public string DisplayName { get; set; }
            public string Version { get; set; }
            public int Status { get; set; }
            public string Revision { get; set; }
            public int Capabilities { get; set; }
            public List<HudSettingJson> Settings { get; set; }
            public Dictionary<string, HudValueJson> Saved { get; set; }
            public Dictionary<string, HudValueJson> Effective { get; set; }
        }

        sealed class HudSettingJson
        {
            public string Key { get; set; }
            public string DisplayName { get; set; }
            public int Kind { get; set; }
            public HudValueJson DefaultValue { get; set; }
            public double? Minimum { get; set; }
            public double? Maximum { get; set; }
        }

        sealed class HudValueJson
        {
            public int Kind { get; set; }
            public bool? Boolean { get; set; }
            public double? Number { get; set; }
            public int? Integer { get; set; }
        }

        sealed class HudResultJson
        {
            public int Outcome { get; set; }
            public string Message { get; set; }
            public List<HudIssueJson> Issues { get; set; }
            public HudSnapshotJson Snapshot { get; set; }
        }

        sealed class HudIssueJson
        {
            public string Key { get; set; }
            public string Message { get; set; }
        }
    }
}
