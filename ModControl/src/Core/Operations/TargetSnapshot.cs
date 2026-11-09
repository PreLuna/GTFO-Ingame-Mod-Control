using System;
using System.Collections.Generic;

namespace PreLuna.ModControl.Core.Operations
{
    public sealed class TargetSnapshot
    {
        public TargetSnapshot(
            TargetDescriptor descriptor,
            string revision,
            IDictionary<string, SettingValue> saved,
            IDictionary<string, SettingValue> effective)
        {
            Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            Revision = revision ?? string.Empty;
            Saved = Copy(saved);
            Effective = Copy(effective);
        }

        public TargetDescriptor Descriptor { get; }
        public string Revision { get; }
        public IReadOnlyDictionary<string, SettingValue> Saved { get; }
        public IReadOnlyDictionary<string, SettingValue> Effective { get; }

        public bool TryGetSaved(string key, out SettingValue value) => Saved.TryGetValue(key, out value);
        public bool TryGetEffective(string key, out SettingValue value) => Effective.TryGetValue(key, out value);

        static IReadOnlyDictionary<string, SettingValue> Copy(IDictionary<string, SettingValue> source)
        {
            var copy = new Dictionary<string, SettingValue>(StringComparer.Ordinal);
            if (source != null)
                foreach (var pair in source) copy[pair.Key] = pair.Value;
            return copy;
        }
    }
}
