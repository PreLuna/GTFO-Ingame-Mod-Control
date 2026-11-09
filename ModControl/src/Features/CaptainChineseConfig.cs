using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class CaptainChineseConfig : IWritableTargetSettings
    {
        static bool chineseMenus = true;
        static bool chineseItems = true;

        public FLabel Status { get; set; } = new FLabel("协同队长·中文显示：正在读取…", "captain.chinese.status");
        public FLabel Notice { get; set; } = new FLabel("重启后生效：只写本地CFG；bot/强化剂仍只读。", "captain.chinese.notice");
        public FLabel Result { get; set; } = new FLabel("", "captain.chinese.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前生效：", "captain.chinese.current");
        public FLabel SavedValues { get; set; } = new FLabel("已保存：", "captain.chinese.saved");

        [FSDisplayName("通讯菜单")]
        public bool ChineseMenus
        {
            get => chineseMenus;
            set => chineseMenus = value;
        }

        [FSDisplayName("携带物品")]
        public bool ChineseItems
        {
            get => chineseItems;
            set => chineseItems = value;
        }

        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            if (snapshot == null) return;
            ChineseMenus = TargetValueReader.Bool(snapshot, "chineseMenus", true);
            ChineseItems = TargetValueReader.Bool(snapshot, "chineseItems", true);
        }
    }
}
