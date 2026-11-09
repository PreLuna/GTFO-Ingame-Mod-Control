using System;
using System.Collections.Generic;
using PreLuna.EnemyHealthBar.Control;
using PreLuna.ModControl.Core.Operations;

namespace PreLuna.ModControl.Adapters
{
    internal sealed class EnemyHealthBarAdapter : ITargetProvider
    {
        public TargetSnapshot ReadSnapshot()
            => MapSnapshot(EnemyHealthBarControlApi.GetSnapshot());

        public ValidationResult Validate(ChangeSet changes, TargetSnapshot snapshot)
        {
            var result = EnemyHealthBarControlApi.Validate(ToChange(changes));
            if (result == null)
                return ValidationResult.Invalid(new ValidationIssue(string.Empty, "敌人血条没有返回校验结果"));
            if (result.Outcome == EnemyHudControlOutcome.Success || result.Outcome == EnemyHudControlOutcome.NoChanges)
                return ValidationResult.Valid();
            return ValidationResult.Invalid(MapIssues(result));
        }

        public ProviderResult SetPersistent(ChangeSet changes, TargetSnapshot snapshot, string expectedRevision)
            => MapResult(EnemyHealthBarControlApi.SetPersistent(ToChange(changes)));

        static EnemyHudControlChange ToChange(ChangeSet changes)
        {
            var values = new Dictionary<string, EnemyHudControlValue>(StringComparer.Ordinal);
            if (changes != null)
            {
                foreach (var pair in changes.Values)
                {
                    var value = ToTargetValue(pair.Value);
                    if (value != null) values[pair.Key] = value;
                }
            }

            return new EnemyHudControlChange(changes?.ExpectedRevision ?? string.Empty, values);
        }

        static EnemyHudControlValue ToTargetValue(SettingValue value)
        {
            if (value == null) return null;
            switch (value.Kind)
            {
                case SettingKind.Boolean:
                    return value.TryGetBoolean(out var boolean) ? EnemyHudControlValue.Boolean(boolean) : null;
                case SettingKind.Integer:
                    return value.TryGetInteger(out var integer) ? EnemyHudControlValue.Integer(integer) : null;
                case SettingKind.Number:
                    return value.TryGetNumber(out var number) ? EnemyHudControlValue.Number(number) : null;
                default:
                    return null;
            }
        }

        static TargetDescriptor MapDescriptor(EnemyHudControlSnapshot snapshot)
        {
            var settings = new List<SettingDefinition>();
            if (snapshot?.Settings != null)
                foreach (var setting in snapshot.Settings) settings.Add(MapSetting(setting));

            return new TargetDescriptor(
                snapshot?.TargetId ?? EnemyHealthBarControlApi.TargetId,
                snapshot?.DisplayName ?? "PreLuna 敌人血条",
                snapshot?.Version ?? string.Empty,
                MapStatus(snapshot?.Status ?? EnemyHudControlStatus.Missing),
                MapCapabilities(snapshot?.Capabilities ?? EnemyHudControlCapabilities.None),
                settings);
        }

        static TargetSnapshot MapSnapshot(EnemyHudControlSnapshot snapshot)
        {
            if (snapshot == null)
                return new TargetSnapshot(MapDescriptor(null), string.Empty, null, null);

            return new TargetSnapshot(
                MapDescriptor(snapshot),
                snapshot.Revision,
                MapValues(snapshot.Saved),
                MapValues(snapshot.Effective));
        }

        static SettingDefinition MapSetting(EnemyHudControlSetting setting)
            => new SettingDefinition(
                setting.Key,
                setting.DisplayName,
                MapKind(setting.Kind),
                MapValue(setting.DefaultValue),
                setting.Minimum,
                setting.Maximum);

        static SettingKind MapKind(EnemyHudControlKind kind)
        {
            switch (kind)
            {
                case EnemyHudControlKind.Boolean: return SettingKind.Boolean;
                case EnemyHudControlKind.Integer: return SettingKind.Integer;
                default: return SettingKind.Number;
            }
        }

        static TargetStatus MapStatus(EnemyHudControlStatus status)
        {
            switch (status)
            {
                case EnemyHudControlStatus.Available: return TargetStatus.Available;
                case EnemyHudControlStatus.Incompatible: return TargetStatus.Incompatible;
                case EnemyHudControlStatus.Uninitialized: return TargetStatus.Uninitialized;
                default: return TargetStatus.Missing;
            }
        }

        static TargetCapabilities MapCapabilities(EnemyHudControlCapabilities capabilities)
        {
            var mapped = TargetCapabilities.None;
            if ((capabilities & EnemyHudControlCapabilities.Validate) != 0) mapped |= TargetCapabilities.Validate;
            if ((capabilities & EnemyHudControlCapabilities.SavePersistent) != 0) mapped |= TargetCapabilities.SavePersistent;
            return mapped;
        }

        static IDictionary<string, SettingValue> MapValues(IReadOnlyDictionary<string, EnemyHudControlValue> source)
        {
            var values = new Dictionary<string, SettingValue>(StringComparer.Ordinal);
            if (source == null) return values;
            foreach (var pair in source)
            {
                var value = MapValue(pair.Value);
                if (value != null) values[pair.Key] = value;
            }

            return values;
        }

        static SettingValue MapValue(EnemyHudControlValue value)
        {
            if (value == null) return null;
            switch (value.Kind)
            {
                case EnemyHudControlKind.Boolean:
                    return value.TryGetBoolean(out var boolean) ? SettingValue.Boolean(boolean) : null;
                case EnemyHudControlKind.Integer:
                    return value.TryGetInteger(out var integer) ? SettingValue.Integer(integer) : null;
                default:
                    return value.TryGetNumber(out var number) ? SettingValue.Number(number) : null;
            }
        }

        static ProviderResult MapResult(EnemyHudControlResult result)
        {
            if (result == null)
                return ProviderResult.Failed("敌人血条没有返回结果");
            switch (result.Outcome)
            {
                case EnemyHudControlOutcome.Success:
                case EnemyHudControlOutcome.NoChanges:
                    return ProviderResult.Success(MapSnapshot(result.Snapshot), result.Message);
                case EnemyHudControlOutcome.StateConflict:
                    return ProviderResult.Conflict(result.Message);
                case EnemyHudControlOutcome.Unavailable:
                    return ProviderResult.Unavailable(result.Message);
                default:
                    return ProviderResult.Failed(result.Message, MapIssues(result));
            }
        }

        static IReadOnlyList<ValidationIssue> MapIssues(EnemyHudControlResult result)
        {
            var issues = new List<ValidationIssue>();
            if (result?.Issues != null)
                foreach (var issue in result.Issues)
                    issues.Add(new ValidationIssue(issue.Key, issue.Message));
            if (issues.Count == 0 && result != null && !string.IsNullOrEmpty(result.Message))
                issues.Add(new ValidationIssue(string.Empty, result.Message));
            return issues;
        }
    }
}
