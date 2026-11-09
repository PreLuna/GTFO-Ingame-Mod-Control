using System;
using System.Collections.Generic;

namespace PreLuna.ModControl.Core.Operations
{
    public sealed class ChangeSet
    {
        public ChangeSet(string expectedRevision, IReadOnlyDictionary<string, SettingValue> values)
        {
            ExpectedRevision = expectedRevision ?? string.Empty;
            var copy = new Dictionary<string, SettingValue>(StringComparer.Ordinal);
            if (values != null)
                foreach (var pair in values) copy[pair.Key] = pair.Value;
            Values = copy;
        }

        public string ExpectedRevision { get; }
        public IReadOnlyDictionary<string, SettingValue> Values { get; }
    }
}
