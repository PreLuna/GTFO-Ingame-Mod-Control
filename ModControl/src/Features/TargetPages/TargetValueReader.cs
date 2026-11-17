using System;
using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.FeaturesAPI.Settings;
using TheArchive.Core.Models;
using TheArchive.Utilities;

namespace PreLuna.ModControl.Features
{
    /// <summary>
    /// Small adapter-side helper for reading snapshot values and converting the
    /// TheArchive setting value into the operation model. No target-specific keys live here.
    /// </summary>
    internal static class TargetValueReader
    {
        internal static bool Bool(TargetSnapshot snapshot, string key, bool fallback)
        {
            if (snapshot == null) return fallback;
            if (snapshot.TryGetEffective(key, out var value) && value.TryGetBoolean(out var effective))
                return effective;
            return snapshot.TryGetSaved(key, out value) && value.TryGetBoolean(out var saved)
                ? saved
                : fallback;
        }

        internal static double Number(TargetSnapshot snapshot, string key, double fallback)
        {
            if (snapshot == null) return fallback;
            if (snapshot.TryGetEffective(key, out var value) && value.TryGetNumber(out var effective))
                return effective;
            return snapshot.TryGetSaved(key, out value) && value.TryGetNumber(out var saved)
                ? saved
                : fallback;
        }

        internal static string String(TargetSnapshot snapshot, string key, string fallback)
        {
            if (snapshot == null) return fallback;
            if (snapshot.TryGetEffective(key, out var value) && value.TryGetString(out var effective))
                return effective;
            return snapshot.TryGetSaved(key, out value) && value.TryGetString(out var saved)
                ? saved
                : fallback;
        }

        internal static SettingValue FromSetting(FeatureSetting setting)
        {
            var raw = setting?.GetValue();
            if (raw is bool boolean) return SettingValue.Boolean(boolean);
            if (raw is float single) return SettingValue.Number(single);
            if (raw is double number) return SettingValue.Number(number);
            if (raw is int integer) return SettingValue.Integer(integer);
            if (raw is string text) return SettingValue.String(text);
            if (raw is SColor color) return SettingValue.String(color.ToHexString());
            return null;
        }
    }
}
