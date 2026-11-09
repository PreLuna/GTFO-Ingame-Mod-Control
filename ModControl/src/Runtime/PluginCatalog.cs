using System;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP;
using PreLuna.ModControl.Core;

namespace PreLuna.ModControl.Runtime
{
    internal static class PluginCatalog
    {
        internal static IReadOnlyList<InstalledPlugin> ReadInstalledPreLuna()
        {
            var source = new List<InstalledPlugin>();
            var plugins = IL2CPPChainloader.Instance?.Plugins;
            if (plugins == null) return Array.Empty<InstalledPlugin>();

            foreach (var pair in plugins)
            {
                var info = pair.Value;
                var metadata = info?.Metadata;
                if (metadata == null) continue;
                source.Add(new InstalledPlugin(metadata.GUID, metadata.Name, metadata.Version?.ToString(), info.Location));
            }

            return TargetCatalog.SelectPreLunaTargets(source);
        }

        internal static bool IsLoaded(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return false;
            var plugins = IL2CPPChainloader.Instance?.Plugins;
            return plugins != null && plugins.ContainsKey(guid);
        }
    }
}
