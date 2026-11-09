using System;

namespace PreLuna.ModControl.Features
{
    /// <summary>
    /// Static registration of one target submenu. TheArchive still requires a static
    /// [FeatureConfig] property per target, so this binding points at that property by
    /// name and type instead of making the page controller hard-code each target.
    /// </summary>
    internal sealed class TargetPageBinding
    {
        internal TargetPageBinding(
            string submenuPropertyName,
            string submenuTitleFragment,
            Type settingsType,
            Func<object> defaultSettings,
            ITargetPage page,
            Action opened = null,
            Action closed = null,
            Action refreshed = null)
        {
            SubmenuPropertyName = submenuPropertyName ?? throw new ArgumentNullException(nameof(submenuPropertyName));
            SubmenuTitleFragment = submenuTitleFragment ?? submenuPropertyName;
            SettingsType = settingsType ?? throw new ArgumentNullException(nameof(settingsType));
            DefaultSettings = defaultSettings;
            Page = page ?? throw new ArgumentNullException(nameof(page));
            Opened = opened;
            Closed = closed;
            Refreshed = refreshed;
        }

        internal string SubmenuPropertyName { get; }
        internal string SubmenuTitleFragment { get; }
        internal Type SettingsType { get; }
        internal Func<object> DefaultSettings { get; }
        internal ITargetPage Page { get; }
        internal Action Opened { get; }
        internal Action Closed { get; }
        internal Action Refreshed { get; }

        internal bool WasOpen { get; set; }
        internal bool Initialized { get; set; }
    }
}
