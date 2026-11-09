using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class FoamGunConfig : IWritableTargetSettings
    {
        static bool enabled = true;
        static float shotInterval = .08f;
        static float launchForce = 32f;
        static float pelletSpread = 1f;
        static float ammoCost = .5f;

        public FLabel Status { get; set; } = new FLabel("泡沫枪调整：正在读取…", "foam.status");
        public FLabel Notice { get; set; } = new FLabel("启用需重启；其他参数即时。", "foam.notice");
        public FLabel Result { get; set; } = new FLabel("", "foam.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前生效：", "foam.current");
        public FLabel SavedValues { get; set; } = new FLabel("已保存：", "foam.saved");

        [FSDisplayName("启用自动泡沫枪")]
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        [FSDisplayName("发射间隔（秒）")]
        [FSSlider(.02f, 2f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float ShotInterval
        {
            get => shotInterval;
            set => shotInterval = value;
        }

        [FSDisplayName("发射力度")]
        [FSSlider(1f, 100f, FSSlider.SliderStyle.FloatOneDecimal, FSSlider.RoundTo.OneDecimal)]
        public float LaunchForce
        {
            get => launchForce;
            set => launchForce = value;
        }

        [FSDisplayName("散布")]
        [FSSlider(1f, 45f, FSSlider.SliderStyle.FloatOneDecimal, FSSlider.RoundTo.OneDecimal)]
        public float PelletSpread
        {
            get => pelletSpread;
            set => pelletSpread = value;
        }

        [FSDisplayName("每发资源消耗")]
        [FSSlider(.01f, 50f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float AmmoCost
        {
            get => ammoCost;
            set => ammoCost = value;
        }

        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            if (snapshot == null) return;
            Enabled = TargetValueReader.Bool(snapshot, "enabled", true);
            ShotInterval = (float)TargetValueReader.Number(snapshot, "shotInterval", .08);
            LaunchForce = (float)TargetValueReader.Number(snapshot, "launchForce", 32);
            PelletSpread = (float)TargetValueReader.Number(snapshot, "pelletSpread", 1);
            AmmoCost = (float)TargetValueReader.Number(snapshot, "ammoCost", .5);
        }
    }
}
