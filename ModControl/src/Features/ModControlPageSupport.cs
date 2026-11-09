using System;
using System.Collections.Generic;
using System.Reflection;
using TheArchive.Core.FeaturesAPI;
using TheArchive.Core.FeaturesAPI.Components;
using TheArchive.Core.FeaturesAPI.Settings;

namespace PreLuna.ModControl.Features
{
    internal static class ModControlPageSupport
    {
        internal static string Line(IReadOnlyList<string> lines, int index)
            => index >= 0 && index < lines.Count ? lines[index] : string.Empty;

        internal static void SetLabel(FLabel label, string text)
        {
            if (label == null) return;
            label.LabelText = text;
            try
            {
                var primary = label.PrimaryText;
                if (primary != null) primary.text = text;
            }
            catch
            {
                // The label may not have been created by the settings page yet; LabelText
                // remains the value used when the page is generated.
            }
        }

        internal static void ClearDirty(FeatureSettingsHelper helper)
        {
            while (helper != null)
            {
                try
                {
                    var property = typeof(FeatureSettingsHelper).GetProperty(
                        "IsDirty",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    property?.SetValue(helper, false);
                }
                catch
                {
                    // The setter is private in the current TheArchive build.
                }

                helper = ParentOf(helper);
            }
        }

        static FeatureSettingsHelper ParentOf(FeatureSettingsHelper helper)
        {
            try
            {
                var property = typeof(FeatureSettingsHelper).GetProperty(
                    "ParentHelper",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                return property?.GetValue(helper) as FeatureSettingsHelper;
            }
            catch
            {
                return null;
            }
        }

        internal static bool IsCurrentSubMenu(string titlePart)
        {
            var submenu = CurrentSubMenu();
            if (submenu == null) return false;
            try
            {
                var title = submenu.GetType().GetProperty(
                    "Title",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(submenu) as string;
                return !string.IsNullOrEmpty(title) &&
                       title.IndexOf(titlePart, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch
            {
                return false;
            }
        }

        internal static bool IsMenuOpen() => CurrentSubMenu() != null;

        internal static bool RefreshCurrentSubMenu()
        {
            try
            {
                var submenu = CurrentSubMenu();
                if (submenu == null) return false;
                submenu.GetType().GetMethod(
                    "Refresh",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(submenu, null);
                return true;
            }
            catch
            {
                // Refreshing is cosmetic; never break the settings callback.
                return false;
            }
        }

        static object CurrentSubMenu()
        {
            try
            {
                var type = typeof(TheArchive.Core.FeaturesAPI.Feature).Assembly.GetType(
                    "TheArchive.Features.Dev.ModSettings+SubMenu",
                    throwOnError: false);
                if (type == null) return null;
                var stackField = type.GetField(
                    "openMenus",
                    BindingFlags.Static | BindingFlags.NonPublic);
                var stack = stackField?.GetValue(null);
                if (stack == null) return null;
                var count = stack.GetType().GetProperty("Count")?.GetValue(stack) is int n ? n : 0;
                if (count <= 0) return null;
                return stack.GetType().GetMethod("Peek")?.Invoke(stack, null);
            }
            catch
            {
                return null;
            }
        }

        internal static void HandleRefresh(ButtonSetting setting, Action refresh)
        {
            if (setting == null || !string.Equals(setting.ButtonID, Core.ModControlInfo.RefreshButtonId, StringComparison.Ordinal)) return;
            // The FButton that raised this setting has RefreshSubMenu=true, so TheArchive
            // rebuilds the current submenu after this callback. Do not regenerate the whole
            // ModSettings page from inside the click handler.
            refresh();
        }
    }
}
