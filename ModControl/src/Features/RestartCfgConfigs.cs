using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class FlashlightRestartConfig : IRestartCfgSettings
    {
        static bool enabled = true, attach, perlin = true, rare = true, horror = true;
        static float angleStep = 30, helmetAngle = 85, helmetIntensity = 1, helmetRange = 17;

        public FLabel Status { get; set; } = new FLabel("手电筒大修：正在读取…", "flashlight.status");
        public FLabel Notice { get; set; } = new FLabel("重启后生效：只写本地CFG；不即时。", "flashlight.notice");
        public FLabel Result { get; set; } = new FLabel("", "flashlight.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前 CFG：…", "flashlight.current");
        public FLabel SavedValues { get; set; } = new FLabel("重启后读取该 CFG", "flashlight.saved");

        [FSDisplayName("总开关")] public bool Enabled { get => enabled; set => enabled = value; }
        [FSDisplayName("随手电模型")] public bool AttachLightToWeaponBone { get => attach; set => attach = value; }
        [FSDisplayName("核心光斑角度")] [FSSlider(5f, 120f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)] public float AngleStep { get => angleStep; set => angleStep = value; }
        [FSDisplayName("头盔光角度")] [FSSlider(5f, 180f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)] public float HelmetAngle { get => helmetAngle; set => helmetAngle = value; }
        [FSDisplayName("头盔亮度")] [FSSlider(0f, 10f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)] public float HelmetIntensity { get => helmetIntensity; set => helmetIntensity = value; }
        [FSDisplayName("头盔距离")] [FSSlider(1f, 100f, FSSlider.SliderStyle.FloatOneDecimal, FSSlider.RoundTo.OneDecimal)] public float HelmetRange { get => helmetRange; set => helmetRange = value; }
        [FSDisplayName("持续微闪")] public bool PerlinFlicker { get => perlin; set => perlin = value; }
        [FSDisplayName("偶发闪烁")] public bool RareFlicker { get => rare; set => rare = value; }
        [FSDisplayName("长暗停顿")] public bool HorrorFlicker { get => horror; set => horror = value; }

        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            Enabled = TargetValueReader.Bool(snapshot, "enabled", true);
            AttachLightToWeaponBone = TargetValueReader.Bool(snapshot, "attach", false);
            AngleStep = (float)TargetValueReader.Number(snapshot, "angleStep", 30);
            HelmetAngle = (float)TargetValueReader.Number(snapshot, "helmetAngle", 85);
            HelmetIntensity = (float)TargetValueReader.Number(snapshot, "helmetIntensity", 1);
            HelmetRange = (float)TargetValueReader.Number(snapshot, "helmetRange", 17);
            PerlinFlicker = TargetValueReader.Bool(snapshot, "perlin", true);
            RareFlicker = TargetValueReader.Bool(snapshot, "rare", true);
            HorrorFlicker = TargetValueReader.Bool(snapshot, "horror", true);
        }
    }

    public sealed class PrisonerMovementRestartConfig : IRestartCfgSettings
    {
        static bool recoil = true, superSlide = true, runReload = true;
        public FLabel Status { get; set; } = new FLabel("移动与生命：正在读取…", "movement.status");
        public FLabel Notice { get; set; } = new FLabel("重启后生效：只写本地CFG；不即时。", "movement.notice");
        public FLabel Result { get; set; } = new FLabel("", "movement.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前 CFG：…", "movement.current");
        public FLabel SavedValues { get; set; } = new FLabel("重启后读取该 CFG", "movement.saved");
        [FSDisplayName("允许后坐力修正")] public bool Recoil { get => recoil; set => recoil = value; }
        [FSDisplayName("允许超级滑铲")] public bool SuperSlide { get => superSlide; set => superSlide = value; }
        [FSDisplayName("允许奔跑换弹")] public bool RunReload { get => runReload; set => runReload = value; }
        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            Recoil = TargetValueReader.Bool(snapshot, "recoil", true);
            SuperSlide = TargetValueReader.Bool(snapshot, "superSlide", true);
            RunReload = TargetValueReader.Bool(snapshot, "runReload", true);
        }
    }

    public sealed class PrisonerHealthRestartConfig : IRestartCfgSettings
    {
        static float fallDamage = 1, minHeight = 1.5f, maxHeight = 1.5f, maxHealth = 2, regenRate = 2, regenCap = 2, regenDelay = 4;
        public FLabel Status { get; set; } = new FLabel("生命调整：正在读取…", "health.status");
        public FLabel Notice { get; set; } = new FLabel("重启后生效：只写本地CFG；不即时。", "health.notice");
        public FLabel Result { get; set; } = new FLabel("", "health.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前 CFG：…", "health.current");
        public FLabel SavedValues { get; set; } = new FLabel("重启后读取该 CFG", "health.saved");
        [FSDisplayName("坠落伤害倍率")] [FSSlider(.1f, 10f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)] public float FallDamage { get => fallDamage; set => fallDamage = value; }
        [FSDisplayName("最小伤害高度倍率")] [FSSlider(.1f, 10f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)] public float MinHeight { get => minHeight; set => minHeight = value; }
        [FSDisplayName("最大伤害高度倍率")] [FSSlider(.1f, 10f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)] public float MaxHeight { get => maxHeight; set => maxHeight = value; }
        [FSDisplayName("最大生命倍率")] [FSSlider(.1f, 10f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)] public float MaxHealth { get => maxHealth; set => maxHealth = value; }
        [FSDisplayName("恢复速率倍率")] [FSSlider(.1f, 10f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)] public float RegenRate { get => regenRate; set => regenRate = value; }
        [FSDisplayName("恢复上限倍率")] [FSSlider(.1f, 10f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)] public float RegenCap { get => regenCap; set => regenCap = value; }
        [FSDisplayName("恢复开始延迟（秒）")] [FSSlider(0f, 60f, FSSlider.SliderStyle.FloatOneDecimal, FSSlider.RoundTo.OneDecimal)] public float RegenDelay { get => regenDelay; set => regenDelay = value; }
        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            FallDamage = (float)TargetValueReader.Number(snapshot, "fallDamage", 1);
            MinHeight = (float)TargetValueReader.Number(snapshot, "minHeight", 1.5);
            MaxHeight = (float)TargetValueReader.Number(snapshot, "maxHeight", 1.5);
            MaxHealth = (float)TargetValueReader.Number(snapshot, "maxHealth", 2);
            RegenRate = (float)TargetValueReader.Number(snapshot, "regenRate", 2);
            RegenCap = (float)TargetValueReader.Number(snapshot, "regenCap", 2);
            RegenDelay = (float)TargetValueReader.Number(snapshot, "regenDelay", 4);
        }
    }

    public sealed class BioScannerRestartConfig : IRestartCfgSettings
    {
        static float offsetX = 10, offsetY = 10;
        public FLabel Status { get; set; } = new FLabel("生物扫描：正在读取…", "bio.status");
        public FLabel Notice { get; set; } = new FLabel("重启后生效：只写本地CFG；不即时。", "bio.notice");
        public FLabel Result { get; set; } = new FLabel("", "bio.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前 CFG：…", "bio.current");
        public FLabel SavedValues { get; set; } = new FLabel("重启后读取该 CFG", "bio.saved");
        [FSDisplayName("偏移 X")] [FSSlider(-1920f, 1920f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)] public float OffsetX { get => offsetX; set => offsetX = value; }
        [FSDisplayName("偏移 Y")] [FSSlider(-1080f, 1080f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)] public float OffsetY { get => offsetY; set => offsetY = value; }
        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            OffsetX = (float)TargetValueReader.Number(snapshot, "offsetX", 10);
            OffsetY = (float)TargetValueReader.Number(snapshot, "offsetY", 10);
        }
    }

    public sealed class CoopBotsRestartConfig : IRestartCfgSettings
    {
        static bool fastFollow = true, taskTarget = true, taskReturn = true, protectScans = true, weakpoint = true, containers = true, teleport = true, combatAwareness = true, combatDecisions = true;
        static float followDistance = 18;
        public FLabel Status { get; set; } = new FLabel("协同机器人：正在读取…", "coop.status");
        public FLabel Notice { get; set; } = new FLabel("重启后生效；bot效果仅主机装MOD生效。", "coop.notice");
        public FLabel Result { get; set; } = new FLabel("", "coop.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前 CFG：…", "coop.current");
        public FLabel SavedValues { get; set; } = new FLabel("重启后读取该 CFG", "coop.saved");
        [FSDisplayName("快速跟随传送")] public bool FastFollow { get => fastFollow; set => fastFollow = value; }
        [FSDisplayName("任务目标传送")] public bool TaskTarget { get => taskTarget; set => taskTarget = value; }
        [FSDisplayName("任务返回传送")] public bool TaskReturn { get => taskReturn; set => taskReturn = value; }
        [FSDisplayName("保护扫描")] public bool ProtectScans { get => protectScans; set => protectScans = value; }
        [FSDisplayName("弱点命中重定向")] public bool Weakpoint { get => weakpoint; set => weakpoint = value; }
        [FSDisplayName("机器人开箱")] public bool Containers { get => containers; set => containers = value; }
        [FSDisplayName("机器人传送")] public bool Teleport { get => teleport; set => teleport = value; }
        [FSDisplayName("战斗感知")] public bool CombatAwareness { get => combatAwareness; set => combatAwareness = value; }
        [FSDisplayName("战斗决策")] public bool CombatDecisions { get => combatDecisions; set => combatDecisions = value; }
        [FSDisplayName("跟随距离")] [FSSlider(2f, 50f, FSSlider.SliderStyle.FloatOneDecimal, FSSlider.RoundTo.OneDecimal)] public float FollowDistance { get => followDistance; set => followDistance = value; }
        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            FastFollow = TargetValueReader.Bool(snapshot, "fastFollow", true);
            TaskTarget = TargetValueReader.Bool(snapshot, "taskTarget", true);
            TaskReturn = TargetValueReader.Bool(snapshot, "taskReturn", true);
            ProtectScans = TargetValueReader.Bool(snapshot, "protectScans", true);
            Weakpoint = TargetValueReader.Bool(snapshot, "weakpoint", true);
            Containers = TargetValueReader.Bool(snapshot, "containers", true);
            Teleport = TargetValueReader.Bool(snapshot, "teleport", true);
            CombatAwareness = TargetValueReader.Bool(snapshot, "combatAwareness", true);
            CombatDecisions = TargetValueReader.Bool(snapshot, "combatDecisions", true);
            FollowDistance = (float)TargetValueReader.Number(snapshot, "followDistance", 18);
        }
    }
}
