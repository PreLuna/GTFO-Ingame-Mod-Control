using System;
using TheArchive.Core.FeaturesAPI.Settings;

namespace PreLuna.ModControl.Features
{
    /// <summary>
    /// Optional contract for target page configurations. Keeping the labels in the
    /// settings object lets a target page render without knowing the target type.
    /// </summary>
    public interface ITargetPageSettings
    {
        TheArchive.Core.FeaturesAPI.Components.FLabel Status { get; }
        TheArchive.Core.FeaturesAPI.Components.FLabel Result { get; }
        TheArchive.Core.FeaturesAPI.Components.FLabel CurrentValues { get; }
        TheArchive.Core.FeaturesAPI.Components.FLabel SavedValues { get; }
    }

    /// <summary>
    /// Settings object that can both expose page labels and accept a target snapshot.
    /// A target adapter never touches this object during a save; it only provides data.
    /// </summary>
    public interface IWritableTargetSettings : ITargetPageSettings
    {
        void ApplySnapshot(Core.Operations.TargetSnapshot snapshot);
    }

    /// <summary>
    /// One page in the "已接入 MOD" feature. The page owns adapter/coordinator state;
    /// InstalledModsPage owns only discovery, open/close lifecycle, and label routing.
    /// </summary>
    internal interface ITargetPage
    {
        Type SettingsType { get; }
        void Refresh(object settings);
        void OnSettingChanged(FeatureSetting setting, object settings);
        void OnOpened();
        void OnClosed();
    }
}
