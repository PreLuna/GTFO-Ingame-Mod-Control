using TheArchive.Core;
using TheArchive.Core.Localization;
using TheArchive.Interfaces;

namespace PreLuna.ModControl.Runtime
{
    // Deliberately no [ArchiveModule] attribute.
    //
    // TheArchive's ArchiveModuleChainloader scans every assembly under BepInEx/plugins and
    // instantiates each type carrying [ArchiveModule]. This assembly is already loaded as a
    // BepInEx plugin, so the attribute would create a second module instance. The BepInEx
    // entry point registers this type explicitly through ArchiveMod.RegisterArchiveModule.
    public sealed class ModControlModule : IArchiveModule
    {
        public ILocalizationService LocalizationService { get; set; }
        public IArchiveLogger Logger { get; set; }

        public void Init()
        {
            ModControlRuntime.AttachArchiveLogger(Logger);
            ModControlRuntime.RefreshFramework();
            ModControlRuntime.RefreshTargets();
        }
    }
}
