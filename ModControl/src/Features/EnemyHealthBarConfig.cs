using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class EnemyHealthBarConfig : IWritableTargetSettings
    {
        // TheArchive recreates the nested settings object when it reloads feature config.
        // Keep the values shared so every page instance reads the same synchronized state.
        static bool enabled = true;
        static float maxDistance = 60f;
        static float width = 200f;
        static float height = 5f;
        static float opacity = .55f;
        static bool showName = true;
        static bool showNumbers = true;
        static bool showStatusSlots = true;

        public FLabel Status { get; set; } = new FLabel("敌人血条：正在读取…", "ehb.status");
        public FLabel Result { get; set; } = new FLabel("", "ehb.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前生效：", "ehb.current");
        public FLabel SavedValues { get; set; } = new FLabel("已保存：", "ehb.saved");

        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            if (snapshot == null) return;
            Enabled = TargetValueReader.Bool(snapshot, "enabled", true);
            MaxDistance = (float)TargetValueReader.Number(snapshot, "maxDistance", 60);
            Width = (float)TargetValueReader.Number(snapshot, "width", 200);
            Height = (float)TargetValueReader.Number(snapshot, "height", 5);
            Opacity = (float)TargetValueReader.Number(snapshot, "opacity", .55);
            ShowName = TargetValueReader.Bool(snapshot, "showName", true);
            ShowNumbers = TargetValueReader.Bool(snapshot, "showNumbers", true);
            ShowStatusSlots = TargetValueReader.Bool(snapshot, "showStatusSlots", true);
        }

        [FSDisplayName("总开关")]
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        [FSDisplayName("最大距离（米）")]
        [FSSlider(5f, 300f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float MaxDistance
        {
            get => maxDistance;
            set => maxDistance = value;
        }

        [FSDisplayName("条宽（px）")]
        [FSSlider(90f, 420f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float Width
        {
            get => width;
            set => width = value;
        }

        [FSDisplayName("条高（px）")]
        [FSSlider(2f, 16f, FSSlider.SliderStyle.FloatOneDecimal, FSSlider.RoundTo.OneDecimal)]
        public float Height
        {
            get => height;
            set => height = value;
        }

        [FSDisplayName("透明度")]
        [FSSlider(.15f, 1f, FSSlider.SliderStyle.FloatPercent, FSSlider.RoundTo.TwoDecimal)]
        public float Opacity
        {
            get => opacity;
            set => opacity = value;
        }

        [FSDisplayName("显示名称")]
        public bool ShowName
        {
            get => showName;
            set => showName = value;
        }

        [FSDisplayName("显示数值")]
        public bool ShowNumbers
        {
            get => showNumbers;
            set => showNumbers = value;
        }

        [FSDisplayName("状态槽")]
        public bool ShowStatusSlots
        {
            get => showStatusSlots;
            set => showStatusSlots = value;
        }
    }
}
