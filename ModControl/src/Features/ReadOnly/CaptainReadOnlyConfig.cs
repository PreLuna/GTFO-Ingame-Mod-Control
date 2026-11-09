using System.Collections.Generic;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class CaptainReadOnlyConfig : IReadOnlyCfgSettings
    {
        public FLabel Status { get; set; } = new FLabel("协同队长（只读）：正在读取…", "captain.status");
        public FLabel Notice { get; set; } = new FLabel("权限：只读不写；bot/强化剂仅主机装MOD生效。", "captain.notice");
        public FLabel Line01 { get; set; } = new FLabel("机器人强化剂：…", "captain.line01");
        public FLabel Line02 { get; set; } = new FLabel("机器人任务：…", "captain.line02");
        public FLabel Line03 { get; set; } = new FLabel("任务传送：…", "captain.line03");
        public FLabel Line04 { get; set; } = new FLabel("资源默认策略：…", "captain.line04");
        public FLabel Line05 { get; set; } = new FLabel("驻守活动半径：…", "captain.line05");
        public FLabel Line06 { get; set; } = new FLabel("指定击杀距离：…", "captain.line06");
        public FLabel Line07 { get; set; } = new FLabel("通讯菜单中文：…", "captain.line07");
        public FLabel Line08 { get; set; } = new FLabel("机器人收集距离：…", "captain.line08");
        public FLabel Reason { get; set; } = new FLabel("冻结模块只读：…", "captain.reason");

        public void SetLines(IReadOnlyList<string> lines)
        {
            ModControlPageSupport.SetLabel(Line01, ReadOnlyCfgLine.At(lines, 0));
            ModControlPageSupport.SetLabel(Line02, ReadOnlyCfgLine.At(lines, 1));
            ModControlPageSupport.SetLabel(Line03, ReadOnlyCfgLine.At(lines, 2));
            ModControlPageSupport.SetLabel(Line04, ReadOnlyCfgLine.At(lines, 3));
            ModControlPageSupport.SetLabel(Line05, ReadOnlyCfgLine.At(lines, 4));
            ModControlPageSupport.SetLabel(Line06, ReadOnlyCfgLine.At(lines, 5));
            ModControlPageSupport.SetLabel(Line07, ReadOnlyCfgLine.At(lines, 6));
            ModControlPageSupport.SetLabel(Line08, ReadOnlyCfgLine.At(lines, 7));
        }
    }
}
