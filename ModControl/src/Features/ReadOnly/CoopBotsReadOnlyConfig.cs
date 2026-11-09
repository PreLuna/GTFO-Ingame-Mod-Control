using System.Collections.Generic;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class CoopBotsReadOnlyConfig : IReadOnlyCfgSettings
    {
        public FLabel Status { get; set; } = new FLabel("协同机器人（只读）：正在读取…", "coopbots.status");
        public FLabel Line01 { get; set; } = new FLabel("快速跟随传送：…", "coopbots.line01");
        public FLabel Line02 { get; set; } = new FLabel("任务目标传送：…", "coopbots.line02");
        public FLabel Line03 { get; set; } = new FLabel("任务返回传送：…", "coopbots.line03");
        public FLabel Line04 { get; set; } = new FLabel("保护扫描：…", "coopbots.line04");
        public FLabel Line05 { get; set; } = new FLabel("弱点命中重定向：…", "coopbots.line05");
        public FLabel Line06 { get; set; } = new FLabel("跟随距离：…", "coopbots.line06");
        public FLabel Line07 { get; set; } = new FLabel("机器人开箱：…", "coopbots.line07");
        public FLabel Line08 { get; set; } = new FLabel("机器人传送：…", "coopbots.line08");
        public FLabel Line09 { get; set; } = new FLabel("战斗感知：…", "coopbots.line09");
        public FLabel Line10 { get; set; } = new FLabel("战斗决策：…", "coopbots.line10");
        public FLabel Reason { get; set; } = new FLabel("冻结模块只读：…", "coopbots.reason");

        public void SetLines(IReadOnlyList<string> lines)
        {
            ModControlPageSupport.SetLabel(Line01, Line(lines, 0));
            ModControlPageSupport.SetLabel(Line02, Line(lines, 1));
            ModControlPageSupport.SetLabel(Line03, Line(lines, 2));
            ModControlPageSupport.SetLabel(Line04, Line(lines, 3));
            ModControlPageSupport.SetLabel(Line05, Line(lines, 4));
            ModControlPageSupport.SetLabel(Line06, Line(lines, 5));
            ModControlPageSupport.SetLabel(Line07, Line(lines, 6));
            ModControlPageSupport.SetLabel(Line08, Line(lines, 7));
            ModControlPageSupport.SetLabel(Line09, Line(lines, 8));
            ModControlPageSupport.SetLabel(Line10, Line(lines, 9));
        }

        static string Line(IReadOnlyList<string> lines, int index)
            => lines != null && index < lines.Count ? lines[index] : string.Empty;
    }
}
