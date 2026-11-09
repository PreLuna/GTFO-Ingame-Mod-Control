using System;
using System.Collections.Generic;
using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.FeaturesAPI.Settings;

namespace PreLuna.ModControl.Features
{
    /// <summary>
    /// Reusable writable target page: adapter creation, coordinator lifecycle, label
    /// formatting, and property-to-target-key mapping are identical for every target.
    /// Target-specific data stays in the settings object's ApplySnapshot and the key map.
    /// </summary>
    internal sealed class WritableTargetPage<TSettings> : ITargetPage
        where TSettings : class, IWritableTargetSettings
    {
        readonly string displayName;
        readonly string expectedVersion;
        readonly Func<ITargetProvider> createProvider;
        readonly IReadOnlyDictionary<string, string> keyMap;
        ITargetProvider provider;
        OperationCoordinator coordinator;
        string resultMessage = string.Empty;

        internal WritableTargetPage(
            string displayName,
            string expectedVersion,
            Func<ITargetProvider> createProvider,
            IReadOnlyDictionary<string, string> keyMap)
        {
            this.displayName = displayName ?? "目标";
            this.expectedVersion = expectedVersion ?? string.Empty;
            this.createProvider = createProvider ?? throw new ArgumentNullException(nameof(createProvider));
            this.keyMap = keyMap ?? throw new ArgumentNullException(nameof(keyMap));
        }

        public Type SettingsType => typeof(TSettings);

        public void Refresh(object settings)
        {
            if (settings is not TSettings typed) return;
            EnsureSession();

            if (provider == null)
            {
                SetUnavailable(typed, ModControlText.MissingPage(displayName, expectedVersion));
                return;
            }

            var snapshot = coordinator?.Snapshot;
            if (snapshot == null || snapshot.Descriptor == null || snapshot.Descriptor.Status != TargetStatus.Available)
            {
                SetUnavailable(typed, displayName + "：目标当前不可用");
                return;
            }

            typed.ApplySnapshot(snapshot);
            var version = string.IsNullOrWhiteSpace(snapshot.Descriptor.Version) ? "未知版本" : snapshot.Descriptor.Version;
            ModControlPageSupport.SetLabel(
                typed.Status,
                displayName + " " + version + " / " + ModControlText.TargetStatusText(snapshot.Descriptor.Status));
            ModControlPageSupport.SetLabel(typed.Result, resultMessage);
            ModControlPageSupport.SetLabel(typed.CurrentValues, "当前生效：" + ModControlText.FormatValues(snapshot.Effective));
            ModControlPageSupport.SetLabel(typed.SavedValues, "已保存：" + ModControlText.FormatValues(snapshot.Saved));
        }

        public void OnSettingChanged(FeatureSetting setting, object settings)
        {
            if (setting == null || settings is not TSettings typed) return;
            EnsureSession();

            var snapshot = coordinator?.Snapshot;
            if (provider == null || snapshot == null || snapshot.Descriptor == null ||
                snapshot.Descriptor.Status != TargetStatus.Available)
            {
                resultMessage = "目标不可用，未保存。";
                Refresh(typed);
                return;
            }

            if (!snapshot.Descriptor.PermissionGranted ||
                !snapshot.Descriptor.Has(TargetCapabilities.SavePersistent))
            {
                resultMessage = "目标只读：" + (string.IsNullOrWhiteSpace(snapshot.Descriptor.PermissionReason)
                    ? "未授予写入能力。"
                    : snapshot.Descriptor.PermissionReason);
                Refresh(typed);
                return;
            }

            if (!keyMap.TryGetValue(setting.Prop?.Name ?? string.Empty, out var key) || string.IsNullOrWhiteSpace(key))
            {
                Refresh(typed);
                return;
            }

            var value = TargetValueReader.FromSetting(setting);
            if (value == null)
            {
                Refresh(typed);
                return;
            }

            resultMessage = ModControlText.Describe(coordinator.SetImmediate(key, value));
            if (coordinator.Snapshot != null) typed.ApplySnapshot(coordinator.Snapshot);
            Refresh(typed);
        }

        public void OnOpened()
        {
        }

        public void OnClosed()
        {
        }

        void EnsureSession()
        {
            if (provider == null) provider = createProvider();
            if (provider == null) return;
            if (coordinator == null) coordinator = new OperationCoordinator(provider);
            else coordinator.Refresh();
        }

        void SetUnavailable(TSettings settings, string status)
        {
            ModControlPageSupport.SetLabel(settings.Status, status);
            ModControlPageSupport.SetLabel(settings.Result, resultMessage);
            ModControlPageSupport.SetLabel(settings.CurrentValues, "当前生效：不可用");
            ModControlPageSupport.SetLabel(settings.SavedValues, "已保存：不可用");
        }
    }
}
