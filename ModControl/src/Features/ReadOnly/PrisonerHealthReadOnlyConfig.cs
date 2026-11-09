using System.Collections.Generic;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class PrisonerHealthReadOnlyConfig : IReadOnlyCfgSettings
    {
        public FLabel Status { get; set; } = new FLabel("生命调整（只读）：正在读取…", "health.status");
        public FLabel Line01 { get; set; } = new FLabel("坠落伤害倍率：…", "health.line01");
        public FLabel Line02 { get; set; } = new FLabel("最小伤害高度倍率：…", "health.line02");
        public FLabel Line03 { get; set; } = new FLabel("最大伤害高度倍率：…", "health.line03");
        public FLabel Line04 { get; set; } = new FLabel("最大生命倍率：…", "health.line04");
        public FLabel Line05 { get; set; } = new FLabel("生命恢复速率倍率：…", "health.line05");
        public FLabel Line06 { get; set; } = new FLabel("恢复上限倍率：…", "health.line06");
        public FLabel Line07 { get; set; } = new FLabel("恢复开始延迟（秒）：…", "health.line07");
        public FLabel Reason { get; set; } = new FLabel("冻结模块只读：…", "health.reason");

        public void SetLines(IReadOnlyList<string> lines)
        {
            ModControlPageSupport.SetLabel(Line01, Line(lines, 0));
            ModControlPageSupport.SetLabel(Line02, Line(lines, 1));
            ModControlPageSupport.SetLabel(Line03, Line(lines, 2));
            ModControlPageSupport.SetLabel(Line04, Line(lines, 3));
            ModControlPageSupport.SetLabel(Line05, Line(lines, 4));
            ModControlPageSupport.SetLabel(Line06, Line(lines, 5));
            ModControlPageSupport.SetLabel(Line07, Line(lines, 6));
        }

        static string Line(IReadOnlyList<string> lines, int index)
            => lines != null && index < lines.Count ? lines[index] : string.Empty;
    }
}
