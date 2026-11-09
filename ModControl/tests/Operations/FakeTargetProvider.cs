using System;
using System.Collections.Generic;
using PreLuna.ModControl.Core.Operations;

namespace PreLuna.ModControl.Tests
{
    sealed class FakeTargetProvider : ITargetProvider
    {
        readonly Dictionary<string, SettingValue> saved = new();
        readonly Dictionary<string, SettingValue> effective = new();
        readonly SettingDefinition[] settings;

        public FakeTargetProvider(string id = "target")
        {
            Id = id;
            settings = new[]
            {
                new SettingDefinition("enabled", "Enabled", SettingKind.Boolean, SettingValue.Boolean(true)),
                new SettingDefinition("distance", "Distance", SettingKind.Number, SettingValue.Number(60), minimum: 5, maximum: 300)
            };
            saved["enabled"] = SettingValue.Boolean(true);
            saved["distance"] = SettingValue.Number(60);
            effective["enabled"] = SettingValue.Boolean(true);
            effective["distance"] = SettingValue.Number(60);
        }

        public string Id { get; }
        public string Revision { get; private set; } = "r1";
        public TargetStatus Status { get; set; } = TargetStatus.Available;
        public TargetCapabilities Capabilities { get; set; } =
            TargetCapabilities.Validate | TargetCapabilities.SavePersistent;
        public bool PermissionGranted { get; set; } = true;
        public string PermissionReason { get; set; } = string.Empty;
        public bool ThrowRead { get; set; }
        public ProviderOutcome? SetPersistentOutcome { get; set; }
        public Exception SetPersistentException { get; set; }
        public ValidationResult Validation { get; set; } = ValidationResult.Valid();
        public Action OnSetPersistent { get; set; }
        public int SetPersistentCalls { get; private set; }
        public int ValidateCalls { get; private set; }

        public TargetDescriptor Describe() => new TargetDescriptor(
            Id,
            "Fake Target",
            "1.0.0",
            Status,
            Capabilities,
            settings,
            permissionGranted: PermissionGranted,
            permissionReason: PermissionReason);

        public TargetSnapshot ReadSnapshot()
        {
            if (ThrowRead) throw new InvalidOperationException("read failed");
            return Snapshot();
        }

        public ValidationResult Validate(ChangeSet changes, TargetSnapshot snapshot)
        {
            ValidateCalls++;
            return Validation;
        }

        public ProviderResult SetPersistent(ChangeSet changes, TargetSnapshot snapshot, string expectedRevision)
        {
            SetPersistentCalls++;
            OnSetPersistent?.Invoke();
            if (SetPersistentException != null) throw SetPersistentException;
            if (SetPersistentOutcome.HasValue)
                return new ProviderResult(SetPersistentOutcome.Value, "injected persistent outcome");
            foreach (var pair in changes.Values)
            {
                saved[pair.Key] = pair.Value;
                effective[pair.Key] = pair.Value;
            }
            BumpRevision();
            return ProviderResult.Success(Snapshot());
        }

        public void BumpRevision()
        {
            var number = 0;
            if (Revision.Length > 1) int.TryParse(Revision.Substring(1), out number);
            Revision = "r" + (number + 1);
        }

        public void SetSaved(string key, SettingValue value) => saved[key] = value;
        public SettingValue GetSaved(string key) => saved[key];
        public SettingValue GetEffective(string key) => effective[key];

        TargetSnapshot Snapshot() => new TargetSnapshot(
            Describe(),
            Revision,
            saved,
            effective);
    }
}
