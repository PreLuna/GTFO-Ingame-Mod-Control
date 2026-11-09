using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class DamageNumbersConfig : IWritableTargetSettings
    {
        static bool enabled = true;
        static float maxRows = 12;
        static float lifetime = .95f;
        static float fontSize = 24f;
        static float maxDistance = 100f;
        static bool showArmor = true;

        public FLabel Status { get; set; } = new FLabel("伤害数字：正在读取…", "damage.status");
        public FLabel Notice { get; set; } = new FLabel("本地视觉设置；父血条隐藏时一起隐藏。", "damage.notice");
        public FLabel Result { get; set; } = new FLabel("", "damage.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前生效：", "damage.current");
        public FLabel SavedValues { get; set; } = new FLabel("已保存：", "damage.saved");

        [FSDisplayName("启用")]
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        [FSDisplayName("最大行数")]
        [FSSlider(1f, 12f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float MaxRows
        {
            get => maxRows;
            set => maxRows = value;
        }

        [FSDisplayName("生命周期（秒）")]
        [FSSlider(.3f, 3f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float Lifetime
        {
            get => lifetime;
            set => lifetime = value;
        }

        [FSDisplayName("字号")]
        [FSSlider(12f, 48f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float FontSize
        {
            get => fontSize;
            set => fontSize = value;
        }

        [FSDisplayName("最大距离（米）")]
        [FSSlider(0f, 300f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float MaxDistance
        {
            get => maxDistance;
            set => maxDistance = value;
        }

        [FSDisplayName("显示护甲命中")]
        public bool ShowArmor
        {
            get => showArmor;
            set => showArmor = value;
        }

        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            if (snapshot == null) return;
            Enabled = TargetValueReader.Bool(snapshot, "enabled", true);
            MaxRows = (float)TargetValueReader.Number(snapshot, "maxRows", 12);
            Lifetime = (float)TargetValueReader.Number(snapshot, "lifetime", .95);
            FontSize = (float)TargetValueReader.Number(snapshot, "fontSize", 24);
            MaxDistance = (float)TargetValueReader.Number(snapshot, "maxDistance", 100);
            ShowArmor = TargetValueReader.Bool(snapshot, "showArmor", true);
        }
    }
}
