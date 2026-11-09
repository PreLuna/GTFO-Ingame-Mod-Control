using System;
using System.Collections.Generic;

namespace PreLuna.ModControl.Core.Operations
{
    public sealed class TargetDescriptor
    {
        public TargetDescriptor(
            string id,
            string displayName,
            string version,
            TargetStatus status,
            TargetCapabilities capabilities,
            IReadOnlyList<SettingDefinition> settings,
            bool permissionGranted = true,
            string permissionReason = "")
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DisplayName = displayName ?? id;
            Version = version ?? string.Empty;
            Status = status;
            Capabilities = capabilities;
            Settings = settings == null ? Array.Empty<SettingDefinition>() : new List<SettingDefinition>(settings).ToArray();
            PermissionGranted = permissionGranted;
            PermissionReason = permissionReason ?? string.Empty;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string Version { get; }
        public TargetStatus Status { get; }
        public TargetCapabilities Capabilities { get; }
        public IReadOnlyList<SettingDefinition> Settings { get; }
        public bool PermissionGranted { get; }
        public string PermissionReason { get; }

        public bool Has(TargetCapabilities capability) => (Capabilities & capability) == capability;

        public SettingDefinition Find(string key)
        {
            foreach (var setting in Settings)
                if (string.Equals(setting.Key, key, StringComparison.Ordinal)) return setting;
            return null;
        }
    }
}
