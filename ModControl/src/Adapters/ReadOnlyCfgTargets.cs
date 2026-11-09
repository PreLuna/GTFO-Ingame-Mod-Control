using System.Collections.Generic;

namespace PreLuna.ModControl.Adapters
{
    /// <summary>
    /// Selected, high-signal frozen settings. These are display-only; the full CFG remains
    /// owned by each target module and is never rewritten by ModControl.
    /// </summary>
    internal static class ReadOnlyCfgTargets
    {
        internal static readonly ReadOnlyCfgTarget Flashlight = new ReadOnlyCfgTarget(
            "PreLuna.FlashlightCustom",
            "手电筒大修",
            "1.0.1",
            "PreLuna.FlashlightCustom.cfg",
            "冻结模块只读：未提供运行控制接口；这里只显示 CFG 当前值。",
            new List<ReadOnlyCfgField>
            {
                new("01 - General", "Enabled", "总开关", ReadOnlyCfgFormat.Boolean),
                new("01 - General", "Mode", "手电模式", ReadOnlyCfgFormat.Text),
                new("01 - General", "AttachLightToWeaponBone", "随手电模型", ReadOnlyCfgFormat.Boolean),
                new("02 - Core Layer (RF_Core)", "AngleStep", "核心光斑角度", ReadOnlyCfgFormat.Number),
                new("05 - Helmet Synth (melee/tools) — all modes", "Angle", "头盔光角度", ReadOnlyCfgFormat.Number),
                new("05 - Helmet Synth (melee/tools) — all modes", "Intensity", "头盔亮度", ReadOnlyCfgFormat.Number),
                new("05 - Helmet Synth (melee/tools) — all modes", "Range", "头盔距离", ReadOnlyCfgFormat.Number),
                new("06 - Perlin Flicker (continuous tremor)", "Enabled", "持续微闪", ReadOnlyCfgFormat.Boolean),
                new("07 - Rare Flicker (bad-contact bursts)", "Enabled", "偶发闪烁", ReadOnlyCfgFormat.Boolean),
                new("08 - Horror Flicker (long dark pause)", "Enabled", "长暗停顿", ReadOnlyCfgFormat.Boolean)
            });

        internal static readonly ReadOnlyCfgTarget PrisonerHealth = new ReadOnlyCfgTarget(
            "PreLuna.PrisonerHealthCustom",
            "生命调整",
            "1.0.1",
            "PreLuna.PrisonerHealthCustom.cfg",
            "冻结模块只读：未提供运行控制接口；这里只显示 CFG 当前值。",
            new List<ReadOnlyCfgField>
            {
                new("Fall Damage", "Fall Damage Multiplier", "坠落伤害倍率", ReadOnlyCfgFormat.Number),
                new("Fall Damage", "Fall Damage Min Height Multiplier", "最小伤害高度倍率", ReadOnlyCfgFormat.Number),
                new("Fall Damage", "Fall Damage Max Height Multiplier", "最大伤害高度倍率", ReadOnlyCfgFormat.Number),
                new("Health", "Max Health Multiplier", "最大生命倍率", ReadOnlyCfgFormat.Number),
                new("Health", "Health Regen Rate Multiplier", "生命恢复速率倍率", ReadOnlyCfgFormat.Number),
                new("Health", "Health Regen Cap Multiplier", "恢复上限倍率", ReadOnlyCfgFormat.Number),
                new("Health", "Health Regen Start Delay Seconds", "恢复开始延迟（秒）", ReadOnlyCfgFormat.Number)
            });

        internal static readonly ReadOnlyCfgTarget BioScanner = new ReadOnlyCfgTarget(
            "PreLuna.BioScannerCustom",
            "生物扫描",
            "1.0.0",
            "PreLuna.BioScannerCustom.cfg",
            "冻结模块只读：未提供运行控制接口；这里只显示 CFG 当前值。",
            new List<ReadOnlyCfgField>
            {
                new("Positioning", "Pre-defined positions", "预设位置", ReadOnlyCfgFormat.Text),
                new("Positioning", "Anchor X", "锚点 X", ReadOnlyCfgFormat.Text),
                new("Positioning", "Anchor Y", "锚点 Y", ReadOnlyCfgFormat.Text),
                new("Positioning", "Offset X", "偏移 X", ReadOnlyCfgFormat.Number),
                new("Positioning", "Offset Y", "偏移 Y", ReadOnlyCfgFormat.Number)
            });

        internal static readonly ReadOnlyCfgTarget CoopBots = new ReadOnlyCfgTarget(
            "PreLuna.CoopBotsCustom",
            "协同机器人",
            "1.0.12",
            "PreLuna.CoopBotsCustom.cfg",
            "冻结模块只读：未提供运行控制接口；这里只显示 CFG 当前值。",
            new List<ReadOnlyCfgField>
            {
                new("Behavior", "Fast Follow Teleport", "快速跟随传送", ReadOnlyCfgFormat.Boolean),
                new("Behavior", "Task Target Teleport", "任务目标传送", ReadOnlyCfgFormat.Boolean),
                new("Behavior", "Task Return Teleport", "任务返回传送", ReadOnlyCfgFormat.Boolean),
                new("Behavior", "Protect Scans", "保护扫描", ReadOnlyCfgFormat.Boolean),
                new("Behavior", "Weakpoint Hit Redirect", "弱点命中重定向", ReadOnlyCfgFormat.Boolean),
                new("Behavior", "Follow Distance", "跟随距离", ReadOnlyCfgFormat.Number),
                new("Bots Open Containers", "Enabled", "机器人开箱", ReadOnlyCfgFormat.Boolean),
                new("Bots Teleport", "Enabled", "机器人传送", ReadOnlyCfgFormat.Boolean),
                new("Combat Awareness", "Enabled", "战斗感知", ReadOnlyCfgFormat.Boolean),
                new("Combat Decisions", "Enabled", "战斗决策", ReadOnlyCfgFormat.Boolean)
            });

        internal static readonly ReadOnlyCfgTarget Captain = new ReadOnlyCfgTarget(
            "PreLuna.Captain",
            "协同队长",
            "0.5.4",
            "PreLuna.Captain.cfg",
            "冻结模块只读：未提供运行控制接口；这里只显示 CFG 当前值。",
            new List<ReadOnlyCfgField>
            {
                new("机器人强化剂", "启用", "机器人强化剂", ReadOnlyCfgFormat.Boolean),
                new("机器人任务", "启用", "机器人任务", ReadOnlyCfgFormat.Boolean),
                new("机器人任务", "任务传送", "任务传送", ReadOnlyCfgFormat.Boolean),
                new("机器人资源使用", "默认策略", "资源默认策略", ReadOnlyCfgFormat.Text),
                new("行动指令", "驻守活动半径", "驻守活动半径", ReadOnlyCfgFormat.Number),
                new("指定击杀", "最大距离", "指定击杀距离", ReadOnlyCfgFormat.Number),
                new("中文显示", "通讯菜单", "通讯菜单中文", ReadOnlyCfgFormat.Boolean),
                new("自动拾取", "机器人收集距离", "机器人收集距离", ReadOnlyCfgFormat.Number)
            });
    }
}
