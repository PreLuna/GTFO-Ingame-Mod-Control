using System;

namespace PreLuna.ModControl.Core.Operations
{
    public sealed class SettingDefinition
    {
        public SettingDefinition(
            string key,
            string displayName,
            SettingKind kind,
            SettingValue defaultValue,
            double? minimum = null,
            double? maximum = null)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            DisplayName = displayName ?? key;
            Kind = kind;
            DefaultValue = defaultValue;
            Minimum = minimum;
            Maximum = maximum;
        }

        public string Key { get; }
        public string DisplayName { get; }
        public SettingKind Kind { get; }
        public SettingValue DefaultValue { get; }
        public double? Minimum { get; }
        public double? Maximum { get; }

        public ValidationIssue Validate(SettingValue value)
        {
            if (value == null) return new ValidationIssue(Key, "缺少值");
            if (value.Kind != Kind) return new ValidationIssue(Key, "值类型不匹配");

            if (Kind == SettingKind.Boolean)
                return value.TryGetBoolean(out _) ? null : new ValidationIssue(Key, "不是合法布尔值");

            if (Kind == SettingKind.Integer)
            {
                if (!value.TryGetInteger(out var integer)) return new ValidationIssue(Key, "不是合法整数");
                return CheckRange(integer);
            }

            if (!value.TryGetNumber(out var number) || double.IsNaN(number) || double.IsInfinity(number))
                return new ValidationIssue(Key, "不是合法数值");
            return CheckRange(number);
        }

        ValidationIssue CheckRange(double value)
        {
            if (Minimum.HasValue && value < Minimum.Value) return new ValidationIssue(Key, "小于最小值 " + Minimum.Value);
            if (Maximum.HasValue && value > Maximum.Value) return new ValidationIssue(Key, "大于最大值 " + Maximum.Value);
            return null;
        }
    }
}
