using System;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using PreLuna.ModControl.Core;
using PreLuna.ModControl.Runtime;
using TheArchive;

namespace PreLuna.ModControl
{
    [BepInPlugin(ModControlInfo.Guid, "PreLuna Mod Control", ModControlInfo.Version)]
    [BepInDependency(ModControlInfo.ArchiveGuid, ModControlInfo.ExpectedArchiveVersion)]
    [BepInDependency(ModControlInfo.EnemyHealthBarGuid, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModControlInfo.ThirdPersonHudGuid, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModControlInfo.ThirdPersonGuid, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModControlInfo.HeadCameraGuid, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModControlInfo.MicroMasterGuid, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModControlInfo.CaptainGuid, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModControlInfo.FoamGunGuid, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModControlInfo.DamageNumbersGuid, BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class Plugin : BasePlugin
    {
        public override void Load()
        {
            ModControlRuntime.AttachLog(Log);
            try
            {
                var setupAlreadyRan = ArchiveMod.RegisterArchiveModule(typeof(ModControlModule));
                ModControlRuntime.MarkRegistered(setupAlreadyRan);
                ModControlRuntime.RefreshFramework();
                ModControlRuntime.RefreshTargets();
                Log.LogInfo("PreLuna.ModControl " + ModControlInfo.Version + " ready; loaded PreLuna plugins=" + ModControlRuntime.Targets.Count + ".");
            }
            catch (Exception error)
            {
                ModControlRuntime.MarkRegistrationFailed(error);
                Log.LogError("PreLuna.ModControl registration failed: " + error);
            }
        }
    }
}
