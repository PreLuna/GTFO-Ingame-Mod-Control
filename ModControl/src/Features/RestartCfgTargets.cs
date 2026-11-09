using System.Collections.Generic;
using PreLuna.ModControl.Core.Operations;

namespace PreLuna.ModControl.Features
{
    internal static class RestartCfgTargets
    {
        internal static readonly RestartCfgTarget Flashlight = new RestartCfgTarget(
            "PreLuna.FlashlightCustom", "手电筒大修", "frozen", "PreLuna.FlashlightCustom.cfg",
            "重启后生效：只写本地 CFG；手电模式/键位等未列入的字段仍只读。",
            new List<RestartCfgField>
            {
                new("enabled", "01 - General", "Enabled", "总开关", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("attach", "01 - General", "AttachLightToWeaponBone", "随手电模型", SettingKind.Boolean, SettingValue.Boolean(false)),
                new("angleStep", "02 - Core Layer (RF_Core)", "AngleStep", "核心光斑角度", SettingKind.Number, SettingValue.Number(30), 5, 120),
                new("helmetAngle", "05 - Helmet Synth (melee/tools) — all modes", "Angle", "头盔光角度", SettingKind.Number, SettingValue.Number(85), 5, 180),
                new("helmetIntensity", "05 - Helmet Synth (melee/tools) — all modes", "Intensity", "头盔亮度", SettingKind.Number, SettingValue.Number(1), 0, 10),
                new("helmetRange", "05 - Helmet Synth (melee/tools) — all modes", "Range", "头盔距离", SettingKind.Number, SettingValue.Number(17), 1, 100),
                new("perlin", "06 - Perlin Flicker (continuous tremor)", "Enabled", "持续微闪", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("rare", "07 - Rare Flicker (bad-contact bursts)", "Enabled", "偶发闪烁", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("horror", "08 - Horror Flicker (long dark pause)", "Enabled", "长暗停顿", SettingKind.Boolean, SettingValue.Boolean(true))
            });

        internal static readonly RestartCfgTarget PrisonerMovement = new RestartCfgTarget(
            "PreLuna.PrisonerMovementCustom", "移动与生命", "frozen", "PreLuna.PrisonerMovementCustom.cfg",
            "重启后生效：只写本地 CFG；不改冻结 DLL。",
            new List<RestartCfgField>
            {
                new("recoil", "APEX: Prisoners", "Allow Recoil Reduce", "允许后坐力修正", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("superSlide", "APEX: Prisoners", "Allow Super Slide", "允许超级滑铲", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("runReload", "APEX: Prisoners", "Allow Run-Reload", "允许奔跑换弹", SettingKind.Boolean, SettingValue.Boolean(true))
            });

        internal static readonly RestartCfgTarget PrisonerHealth = new RestartCfgTarget(
            "PreLuna.PrisonerHealthCustom", "生命调整", "frozen", "PreLuna.PrisonerHealthCustom.cfg",
            "重启后生效：只写本地 CFG；不改冻结 DLL。",
            new List<RestartCfgField>
            {
                new("fallDamage", "Fall Damage", "Fall Damage Multiplier", "坠落伤害倍率", SettingKind.Number, SettingValue.Number(1), .1, 10),
                new("minHeight", "Fall Damage", "Fall Damage Min Height Multiplier", "最小伤害高度倍率", SettingKind.Number, SettingValue.Number(1.5), .1, 10),
                new("maxHeight", "Fall Damage", "Fall Damage Max Height Multiplier", "最大伤害高度倍率", SettingKind.Number, SettingValue.Number(1.5), .1, 10),
                new("maxHealth", "Health", "Max Health Multiplier", "最大生命倍率", SettingKind.Number, SettingValue.Number(2), .1, 10),
                new("regenRate", "Health", "Health Regen Rate Multiplier", "恢复速率倍率", SettingKind.Number, SettingValue.Number(2), .1, 10),
                new("regenCap", "Health", "Health Regen Cap Multiplier", "恢复上限倍率", SettingKind.Number, SettingValue.Number(2), .1, 10),
                new("regenDelay", "Health", "Health Regen Start Delay Seconds", "恢复开始延迟", SettingKind.Number, SettingValue.Number(4), 0, 60)
            });

        internal static readonly RestartCfgTarget BioScanner = new RestartCfgTarget(
            "PreLuna.BioScannerCustom", "生物扫描", "frozen", "PreLuna.BioScannerCustom.cfg",
            "重启后生效：只写本地 CFG；位置/锚点等文本字段仍只读。",
            new List<RestartCfgField>
            {
                new("offsetX", "Positioning", "Offset X", "偏移 X", SettingKind.Number, SettingValue.Number(10), -1920, 1920),
                new("offsetY", "Positioning", "Offset Y", "偏移 Y", SettingKind.Number, SettingValue.Number(10), -1080, 1080)
            });

        internal static readonly RestartCfgTarget CoopBots = new RestartCfgTarget(
            "PreLuna.CoopBotsCustom", "协同机器人", "frozen", "PreLuna.CoopBotsCustom.cfg",
            "重启后生效；bot 效果仅装 MOD 的主机实际生效。",
            new List<RestartCfgField>
            {
                new("fastFollow", "Behavior", "Fast Follow Teleport", "快速跟随传送", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("taskTarget", "Behavior", "Task Target Teleport", "任务目标传送", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("taskReturn", "Behavior", "Task Return Teleport", "任务返回传送", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("protectScans", "Behavior", "Protect Scans", "保护扫描", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("weakpoint", "Behavior", "Weakpoint Hit Redirect", "弱点命中重定向", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("containers", "Bots Open Containers", "Enabled", "机器人开箱", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("teleport", "Bots Teleport", "Enabled", "机器人传送", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("combatAwareness", "Combat Awareness", "Enabled", "战斗感知", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("combatDecisions", "Combat Decisions", "Enabled", "战斗决策", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("followDistance", "Behavior", "Follow Distance", "跟随距离", SettingKind.Number, SettingValue.Number(18), 2, 50)
            });

        internal static readonly RestartCfgTarget CaptainBot = new RestartCfgTarget(
            "PreLuna.Captain.Bot", "协同队长 bot/强化剂", "0.5.4", "PreLuna.Captain.cfg",
            "重启后生效；默认策略/中文仍只读；bot仅主机装MOD生效。",
            new List<RestartCfgField>
            {
                new("booster", "机器人强化剂", "启用", "机器人强化剂", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("tasks", "机器人任务", "启用", "机器人任务", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("teleport", "机器人任务", "任务传送", "任务传送", SettingKind.Boolean, SettingValue.Boolean(true)),
                new("stationRadius", "行动指令", "驻守活动半径", "驻守活动半径", SettingKind.Number, SettingValue.Number(3), 1.5, 8),
                new("killDistance", "指定击杀", "最大距离", "指定击杀距离", SettingKind.Number, SettingValue.Number(30), 3, 60),
                new("pickupDistance", "自动拾取", "机器人收集距离", "机器人收集距离", SettingKind.Number, SettingValue.Number(15), 3, 30)
            });
    }
}
