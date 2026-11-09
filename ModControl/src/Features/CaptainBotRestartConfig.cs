using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class CaptainBotRestartConfig : IRestartCfgSettings
    {
        static bool booster = true, tasks = true, teleport = true;
        static float stationRadius = 3, killDistance = 30, pickupDistance = 15;

        public FLabel Status { get; set; } = new FLabel("协同队长 bot/强化剂：正在读取…", "captain.bot.status");
        public FLabel Notice { get; set; } = new FLabel("重启后生效；默认策略/中文仍只读；bot仅主机装MOD生效。", "captain.bot.notice");
        public FLabel Result { get; set; } = new FLabel("", "captain.bot.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前 CFG：…", "captain.bot.current");
        public FLabel SavedValues { get; set; } = new FLabel("重启后读取该 CFG", "captain.bot.saved");

        [FSDisplayName("机器人强化剂")] public bool Booster { get => booster; set => booster = value; }
        [FSDisplayName("机器人任务")] public bool Tasks { get => tasks; set => tasks = value; }
        [FSDisplayName("任务传送")] public bool TaskTeleport { get => teleport; set => teleport = value; }
        [FSDisplayName("驻守活动半径")] [FSSlider(1.5f, 8f, FSSlider.SliderStyle.FloatOneDecimal, FSSlider.RoundTo.OneDecimal)] public float StationRadius { get => stationRadius; set => stationRadius = value; }
        [FSDisplayName("指定击杀距离")] [FSSlider(3f, 60f, FSSlider.SliderStyle.FloatOneDecimal, FSSlider.RoundTo.OneDecimal)] public float KillDistance { get => killDistance; set => killDistance = value; }
        [FSDisplayName("机器人收集距离")] [FSSlider(3f, 30f, FSSlider.SliderStyle.FloatOneDecimal, FSSlider.RoundTo.OneDecimal)] public float PickupDistance { get => pickupDistance; set => pickupDistance = value; }

        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            Booster = TargetValueReader.Bool(snapshot, "booster", true);
            Tasks = TargetValueReader.Bool(snapshot, "tasks", true);
            TaskTeleport = TargetValueReader.Bool(snapshot, "teleport", true);
            StationRadius = (float)TargetValueReader.Number(snapshot, "stationRadius", 3);
            KillDistance = (float)TargetValueReader.Number(snapshot, "killDistance", 30);
            PickupDistance = (float)TargetValueReader.Number(snapshot, "pickupDistance", 15);
        }
    }
}
