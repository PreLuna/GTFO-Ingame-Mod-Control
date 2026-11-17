using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI.Components;
using TheArchive.Core.Models;
using TheArchive.Utilities;

namespace PreLuna.ModControl.Features
{
    public sealed class BfvFeedbackConfig : IWritableTargetSettings, ITargetValueSummary
    {
        static bool enabled = true;
        static bool followHudVisibility = true;
        static bool soundEnabled = true;
        static float styleIndex = 1;
        static float volume = 1f;
        static float gain = 1.8f;
        static float killGain = 1.6f;
        static float killHighPassHz = 350f;
        static bool killChimeEnabled = true;
        static float killChimeVolume = .9f;
        static float killPriorityMs = 150f;
        static bool muteVanillaIndicator = true;
        static bool hitMarkerEnabled = true;
        static bool replaceVanilla = true;
        static float hitMarkerSize = 64f;
        static bool armorFromNoDamage = false;
        static SColor colorNormal = ParseColor("#FFFFFF", "#FFFFFF");
        static SColor colorWeakspot = ParseColor("#FF8C00", "#FF8C00");
        static SColor colorArmor = ParseColor("#49ADC9", "#49ADC9");
        static SColor colorKill = ParseColor("#B92828", "#B92828");
        static bool killIconEnabled = true;
        static float killIconSize = 46f;
        static float killIconOpacity = .85f;
        static float killIconLifetime = 2.2f;
        static bool healthTiering = false;
        static float killIconGap = .35f;

        public FLabel Status { get; set; } = new FLabel("BFV 反馈：正在读取…", "bfv.status");
        public FLabel Notice { get; set; } = new FLabel("即时生效；颜色点 Pick Color，色相/饱和度/明度粗略调，十六进制可留空。", "bfv.notice");
        public FLabel Result { get; set; } = new FLabel("", "bfv.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前生效：", "bfv.current");
        public FLabel SavedValues { get; set; } = new FLabel("已保存：", "bfv.saved");

        bool ITargetValueSummary.ShowValueSummaries => false;

        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            if (snapshot == null) return;
            Enabled = TargetValueReader.Bool(snapshot, "enabled", true);
            FollowHudVisibility = TargetValueReader.Bool(snapshot, "followHudVisibility", true);
            SoundEnabled = TargetValueReader.Bool(snapshot, "soundEnabled", true);
            StyleIndex = (float)TargetValueReader.Number(snapshot, "styleIndex", 1);
            Volume = (float)TargetValueReader.Number(snapshot, "volume", 1);
            Gain = (float)TargetValueReader.Number(snapshot, "gain", 1.8);
            KillGain = (float)TargetValueReader.Number(snapshot, "killGain", 1.6);
            KillHighPassHz = (float)TargetValueReader.Number(snapshot, "killHighPassHz", 350);
            KillChimeEnabled = TargetValueReader.Bool(snapshot, "killChimeEnabled", true);
            KillChimeVolume = (float)TargetValueReader.Number(snapshot, "killChimeVolume", .9);
            KillPriorityMs = (float)TargetValueReader.Number(snapshot, "killPriorityMs", 150);
            MuteVanillaIndicator = TargetValueReader.Bool(snapshot, "muteVanillaIndicator", true);
            HitMarkerEnabled = TargetValueReader.Bool(snapshot, "hitMarkerEnabled", true);
            ReplaceVanilla = TargetValueReader.Bool(snapshot, "replaceVanilla", true);
            HitMarkerSize = (float)TargetValueReader.Number(snapshot, "hitMarkerSize", 64);
            ArmorFromNoDamage = TargetValueReader.Bool(snapshot, "armorFromNoDamage", false);
            ColorNormal = ParseColor(TargetValueReader.String(snapshot, "colorNormal", "#FFFFFF"), "#FFFFFF");
            ColorWeakspot = ParseColor(TargetValueReader.String(snapshot, "colorWeakspot", "#FF8C00"), "#FF8C00");
            ColorArmor = ParseColor(TargetValueReader.String(snapshot, "colorArmor", "#49ADC9"), "#49ADC9");
            ColorKill = ParseColor(TargetValueReader.String(snapshot, "colorKill", "#B92828"), "#B92828");
            KillIconEnabled = TargetValueReader.Bool(snapshot, "killIconEnabled", true);
            KillIconSize = (float)TargetValueReader.Number(snapshot, "killIconSize", 46);
            KillIconOpacity = (float)TargetValueReader.Number(snapshot, "killIconOpacity", .85);
            KillIconLifetime = (float)TargetValueReader.Number(snapshot, "killIconLifetime", 2.2);
            HealthTiering = TargetValueReader.Bool(snapshot, "healthTiering", false);
            KillIconGap = (float)TargetValueReader.Number(snapshot, "killIconGap", .35);
        }

        [FSDisplayName("启用 BFV 反馈")]
        public bool Enabled { get => enabled; set => enabled = value; }

        [FSDisplayName("跟随血条显隐")]
        public bool FollowHudVisibility { get => followHudVisibility; set => followHudVisibility = value; }

        [FSDisplayName("音效总开关")]
        public bool SoundEnabled { get => soundEnabled; set => soundEnabled = value; }

        [FSDisplayName("音效风格（0=BF1 1=BF5 2=BF6）")]
        [FSSlider(0f, 2f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float StyleIndex { get => styleIndex; set => styleIndex = value; }

        [FSDisplayName("主音量")]
        [FSSlider(0f, 1f, FSSlider.SliderStyle.FloatPercent, FSSlider.RoundTo.TwoDecimal)]
        public float Volume { get => volume; set => volume = value; }

        [FSDisplayName("总增益")]
        [FSSlider(.5f, 3f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float Gain { get => gain; set => gain = value; }

        [FSDisplayName("击杀增益")]
        [FSSlider(.5f, 3f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float KillGain { get => killGain; set => killGain = value; }

        [FSDisplayName("击杀高通（Hz，0=关）")]
        [FSSlider(0f, 2000f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float KillHighPassHz { get => killHighPassHz; set => killHighPassHz = value; }

        [FSDisplayName("击杀确认音")]
        public bool KillChimeEnabled { get => killChimeEnabled; set => killChimeEnabled = value; }

        [FSDisplayName("确认音音量")]
        [FSSlider(0f, 1f, FSSlider.SliderStyle.FloatPercent, FSSlider.RoundTo.TwoDecimal)]
        public float KillChimeVolume { get => killChimeVolume; set => killChimeVolume = value; }

        [FSDisplayName("击杀优先级（ms）")]
        [FSSlider(0f, 500f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float KillPriorityMs { get => killPriorityMs; set => killPriorityMs = value; }

        [FSDisplayName("静音原版提示")]
        public bool MuteVanillaIndicator { get => muteVanillaIndicator; set => muteVanillaIndicator = value; }

        [FSDisplayName("命中线开关")]
        public bool HitMarkerEnabled { get => hitMarkerEnabled; set => hitMarkerEnabled = value; }

        [FSDisplayName("替换原版标记")]
        public bool ReplaceVanilla { get => replaceVanilla; set => replaceVanilla = value; }

        [FSDisplayName("命中线尺寸")]
        [FSSlider(16f, 200f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float HitMarkerSize { get => hitMarkerSize; set => hitMarkerSize = value; }

        [FSDisplayName("把 noDamage 当护甲")]
        public bool ArmorFromNoDamage { get => armorFromNoDamage; set => armorFromNoDamage = value; }

        [FSDisplayName("普通命中颜色")]
        public SColor ColorNormal { get => colorNormal; set => colorNormal = value; }

        [FSDisplayName("弱点命中颜色")]
        public SColor ColorWeakspot { get => colorWeakspot; set => colorWeakspot = value; }

        [FSDisplayName("护甲颜色")]
        public SColor ColorArmor { get => colorArmor; set => colorArmor = value; }

        [FSDisplayName("击杀线颜色")]
        public SColor ColorKill { get => colorKill; set => colorKill = value; }

        [FSDisplayName("击杀图标开关")]
        public bool KillIconEnabled { get => killIconEnabled; set => killIconEnabled = value; }

        [FSDisplayName("图标尺寸")]
        [FSSlider(26f, 76f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float KillIconSize { get => killIconSize; set => killIconSize = value; }

        [FSDisplayName("图标透明度")]
        [FSSlider(0f, 1f, FSSlider.SliderStyle.FloatPercent, FSSlider.RoundTo.TwoDecimal)]
        public float KillIconOpacity { get => killIconOpacity; set => killIconOpacity = value; }

        [FSDisplayName("图标寿命（秒）")]
        [FSSlider(.4f, 6f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float KillIconLifetime { get => killIconLifetime; set => killIconLifetime = value; }

        [FSDisplayName("按血量分档（关=按敌人类型）")]
        public bool HealthTiering { get => healthTiering; set => healthTiering = value; }

        [FSDisplayName("头部间距（米）")]
        [FSSlider(0f, 2f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float KillIconGap { get => killIconGap; set => killIconGap = value; }

        static SColor ParseColor(string text, string fallback)
        {
            try
            {
                return SColorExtensions.FromHexString(string.IsNullOrWhiteSpace(text) ? fallback : text);
            }
            catch
            {
                return SColorExtensions.FromHexString(fallback);
            }
        }
    }
}
