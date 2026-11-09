using System;
using PreLuna.ModControl.Core;
using PreLuna.ModControl.Core.Operations;
using PreLuna.ModControl.Runtime;

namespace PreLuna.ModControl.Adapters
{
    internal static class EnemyHealthBarAdapterFactory
    {
        internal static ITargetProvider TryCreate()
        {
            try
            {
                if (!PluginCatalog.IsLoaded(ModControlInfo.EnemyHealthBarGuid)) return null;
                var type = typeof(EnemyHealthBarAdapterFactory).Assembly.GetType(
                    "PreLuna.ModControl.Adapters.EnemyHealthBarAdapter",
                    throwOnError: false);
                if (type == null) return null;
                return Activator.CreateInstance(type) as ITargetProvider;
            }
            catch
            {
                // A missing or older EnemyHealthBar assembly must not take down ModControl.
                return null;
            }
        }
    }
}
