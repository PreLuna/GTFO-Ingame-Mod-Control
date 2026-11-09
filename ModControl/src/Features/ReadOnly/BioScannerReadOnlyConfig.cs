using System.Collections.Generic;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class BioScannerReadOnlyConfig : IReadOnlyCfgSettings
    {
        public FLabel Status { get; set; } = new FLabel("生物扫描（只读）：正在读取…", "bio.status");
        public FLabel Line01 { get; set; } = new FLabel("预设位置：…", "bio.line01");
        public FLabel Line02 { get; set; } = new FLabel("锚点 X：…", "bio.line02");
        public FLabel Line03 { get; set; } = new FLabel("锚点 Y：…", "bio.line03");
        public FLabel Line04 { get; set; } = new FLabel("偏移 X：…", "bio.line04");
        public FLabel Line05 { get; set; } = new FLabel("偏移 Y：…", "bio.line05");
        public FLabel Reason { get; set; } = new FLabel("冻结模块只读：…", "bio.reason");

        public void SetLines(IReadOnlyList<string> lines)
        {
            ModControlPageSupport.SetLabel(Line01, Line(lines, 0));
            ModControlPageSupport.SetLabel(Line02, Line(lines, 1));
            ModControlPageSupport.SetLabel(Line03, Line(lines, 2));
            ModControlPageSupport.SetLabel(Line04, Line(lines, 3));
            ModControlPageSupport.SetLabel(Line05, Line(lines, 4));
        }

        static string Line(IReadOnlyList<string> lines, int index)
            => lines != null && index < lines.Count ? lines[index] : string.Empty;
    }
}
