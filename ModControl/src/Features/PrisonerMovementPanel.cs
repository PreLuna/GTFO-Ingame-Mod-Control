using System;
using PreLuna.ModControl.Adapters;
using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.FeaturesAPI.Settings;

namespace PreLuna.ModControl.Features
{
    /// <summary>
    /// Frozen target page: read-only parsing of an existing CFG. It intentionally has no
    /// provider/coordinator and never calls a write API.
    /// </summary>
    internal sealed class PrisonerMovementPanel : ITargetPage
    {
        readonly PrisonerMovementReadOnlyAdapter adapter = new PrisonerMovementReadOnlyAdapter();

        public Type SettingsType => typeof(PrisonerMovementReadOnlyConfig);

        public void Refresh(object settings)
        {
            if (settings is not PrisonerMovementReadOnlyConfig typed) return;

            TargetSnapshot snapshot;
            try { snapshot = adapter.ReadSnapshot(); }
            catch { snapshot = null; }

            var descriptor = snapshot?.Descriptor;
            if (descriptor == null || descriptor.Status != TargetStatus.Available)
            {
                ModControlPageSupport.SetLabel(typed.Status, "移动与生命（只读）：未找到 PreLuna.PrisonerMovementCustom 配置");
                ModControlPageSupport.SetLabel(typed.Recoil, "允许后坐力修正：不可用");
                ModControlPageSupport.SetLabel(typed.SuperSlide, "允许超级滑铲：不可用");
                ModControlPageSupport.SetLabel(typed.RunReload, "允许奔跑换弹：不可用");
                ModControlPageSupport.SetLabel(typed.Reason, descriptor?.PermissionReason ?? "冻结模块只读：未提供运行控制接口。");
                return;
            }

            ModControlPageSupport.SetLabel(typed.Status, "移动与生命（只读）/ 冻结模块 / " + ModControlText.TargetStatusText(descriptor.Status));
            ModControlPageSupport.SetLabel(typed.Recoil, "允许后坐力修正：" + Format(snapshot, "recoil"));
            ModControlPageSupport.SetLabel(typed.SuperSlide, "允许超级滑铲：" + Format(snapshot, "superSlide"));
            ModControlPageSupport.SetLabel(typed.RunReload, "允许奔跑换弹：" + Format(snapshot, "runReload"));
            ModControlPageSupport.SetLabel(typed.Reason, descriptor.PermissionReason);
        }

        public void OnSettingChanged(FeatureSetting setting, object settings)
        {
            // The page exposes labels only; TheArchive has no editable control here.
        }

        public void OnOpened()
        {
        }

        public void OnClosed()
        {
        }

        static string Format(TargetSnapshot snapshot, string key)
        {
            if (snapshot != null && snapshot.TryGetEffective(key, out var value) && value.TryGetBoolean(out var boolean))
                return ModControlText.YesNo(boolean);
            return "未知";
        }
    }
}
