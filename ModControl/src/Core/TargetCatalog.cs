using System;
using System.Collections.Generic;

namespace PreLuna.ModControl.Core
{
    public static class TargetCatalog
    {
        public static IReadOnlyList<InstalledPlugin> SelectPreLunaTargets(IEnumerable<InstalledPlugin> plugins)
        {
            var byGuid = new Dictionary<string, InstalledPlugin>(StringComparer.OrdinalIgnoreCase);
            if (plugins != null)
            {
                foreach (var plugin in plugins)
                {
                    if (plugin == null) continue;
                    var guid = (plugin.Guid ?? string.Empty).Trim();
                    if (guid.Length == 0) continue;
                    if (!guid.StartsWith("PreLuna.", StringComparison.OrdinalIgnoreCase)) continue;
                    if (string.Equals(guid, ModControlInfo.Guid, StringComparison.OrdinalIgnoreCase)) continue;
                    if (!byGuid.ContainsKey(guid)) byGuid.Add(guid, plugin);
                }
            }

            var targets = new List<InstalledPlugin>(byGuid.Count);
            foreach (var pair in byGuid) targets.Add(pair.Value);
            targets.Sort(CompareByGuid);
            return targets;
        }

        static int CompareByGuid(InstalledPlugin left, InstalledPlugin right)
            => string.Compare(left.Guid, right.Guid, StringComparison.OrdinalIgnoreCase);
    }
}
