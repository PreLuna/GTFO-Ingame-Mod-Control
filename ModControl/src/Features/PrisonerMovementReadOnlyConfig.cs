using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class PrisonerMovementReadOnlyConfig
    {
        public FLabel Status { get; set; } = new FLabel("移动与生命（只读）：正在读取…", "pm.status");
        public FLabel Recoil { get; set; } = new FLabel("允许后坐力修正：…", "pm.recoil");
        public FLabel SuperSlide { get; set; } = new FLabel("允许超级滑铲：…", "pm.slide");
        public FLabel RunReload { get; set; } = new FLabel("允许奔跑换弹：…", "pm.reload");
        public FLabel Reason { get; set; } = new FLabel("冻结模块只读：未提供运行控制接口。", "pm.reason");
    }
}
