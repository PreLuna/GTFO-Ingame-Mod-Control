using System;

namespace PreLuna.ModControl.Core.Operations
{
    public sealed class SettingValue
    {
        SettingValue(SettingKind kind, object raw)
        {
            Kind = kind;
            Raw = raw;
        }

        public SettingKind Kind { get; }
        public object Raw { get; }

        public static SettingValue Boolean(bool value) => new SettingValue(SettingKind.Boolean, value);
        public static SettingValue Integer(int value) => new SettingValue(SettingKind.Integer, value);
        public static SettingValue Number(double value) => new SettingValue(SettingKind.Number, value);

        public bool TryGetBoolean(out bool value)
        {
            if (Kind == SettingKind.Boolean && Raw is bool typed)
            {
                value = typed;
                return true;
            }

            value = false;
            return false;
        }

        public bool TryGetInteger(out int value)
        {
            if (Kind == SettingKind.Integer && Raw is int typed)
            {
                value = typed;
                return true;
            }

            value = 0;
            return false;
        }

        public bool TryGetNumber(out double value)
        {
            if (Kind == SettingKind.Number && Raw is double typed)
            {
                value = typed;
                return true;
            }

            value = 0;
            return false;
        }

        public bool ValueEquals(SettingValue other)
        {
            if (other == null || Kind != other.Kind || Raw == null || other.Raw == null) return false;
            if (Kind == SettingKind.Number)
                return Raw is double left && other.Raw is double right && Math.Abs(left - right) < 1e-9;
            return Raw.Equals(other.Raw);
        }

        public override string ToString() => Raw?.ToString() ?? string.Empty;
    }
}
