using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class ThirdPersonHudConfig : IWritableTargetSettings
    {
        static bool panel = true;
        static bool personal = true;
        static bool team = true;
        static bool scene = true;
        static bool alerts = true;

        public FLabel Status { get; set; } = new FLabel("第三人称 HUD：正在读取…", "tph.status");
        public FLabel Result { get; set; } = new FLabel("", "tph.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前生效：", "tph.current");
        public FLabel SavedValues { get; set; } = new FLabel("已保存：", "tph.saved");

        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            if (snapshot == null) return;
            Panel = TargetValueReader.Bool(snapshot, "panel", true);
            Personal = TargetValueReader.Bool(snapshot, "personal", true);
            Team = TargetValueReader.Bool(snapshot, "team", true);
            Scene = TargetValueReader.Bool(snapshot, "scene", true);
            Alerts = TargetValueReader.Bool(snapshot, "alerts", true);
        }

        [FSDisplayName("角色旁面板")]
        public bool Panel
        {
            get => panel;
            set => panel = value;
        }

        [FSDisplayName("个人读数")]
        public bool Personal
        {
            get => personal;
            set => personal = value;
        }

        [FSDisplayName("队伍摘要")]
        public bool Team
        {
            get => team;
            set => team = value;
        }

        [FSDisplayName("场景提示")]
        public bool Scene
        {
            get => scene;
            set => scene = value;
        }

        [FSDisplayName("提示区")]
        public bool Alerts
        {
            get => alerts;
            set => alerts = value;
        }
    }
}
