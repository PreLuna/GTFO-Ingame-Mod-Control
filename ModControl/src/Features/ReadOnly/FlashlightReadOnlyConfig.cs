using System.Collections.Generic;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class FlashlightReadOnlyConfig : IReadOnlyCfgSettings
    {
        public FLabel Status { get; set; } = new FLabel("手电筒大修（只读）：正在读取…", "flashlight.status");
        public FLabel Line01 { get; set; } = new FLabel("总开关：…", "flashlight.line01");
        public FLabel Line02 { get; set; } = new FLabel("手电模式：…", "flashlight.line02");
        public FLabel Line03 { get; set; } = new FLabel("随手电模型：…", "flashlight.line03");
        public FLabel Line04 { get; set; } = new FLabel("核心光斑角度：…", "flashlight.line04");
        public FLabel Line05 { get; set; } = new FLabel("头盔光角度：…", "flashlight.line05");
        public FLabel Line06 { get; set; } = new FLabel("头盔亮度：…", "flashlight.line06");
        public FLabel Line07 { get; set; } = new FLabel("头盔距离：…", "flashlight.line07");
        public FLabel Line08 { get; set; } = new FLabel("持续微闪：…", "flashlight.line08");
        public FLabel Line09 { get; set; } = new FLabel("偶发闪烁：…", "flashlight.line09");
        public FLabel Line10 { get; set; } = new FLabel("长暗停顿：…", "flashlight.line10");
        public FLabel Reason { get; set; } = new FLabel("冻结模块只读：…", "flashlight.reason");

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
