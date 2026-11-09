using PreLuna.ModControl.Runtime;
using TheArchive.Core.Attributes.Feature;
using TheArchive.Core.Attributes.Feature.Members;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI;
using TheArchive.Core.FeaturesAPI.Components;
using TheArchive.Core.FeaturesAPI.Settings;

namespace PreLuna.ModControl.Features
{
    [EnableFeatureByDefault]
    public sealed class DiagnosticsPage : Feature
    {
        [FeatureConfig]
        public static DiagnosticsSettings Settings { get; set; } = new DiagnosticsSettings();

        public override string Name => "状态与诊断";
        public override string Description => "只读诊断在左侧“设置 → 进入”中查看；右侧只显示功能说明。";

        public override void Init()
        {
            base.Init();
            RefreshPage();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            RefreshPage();
        }

        public override void OnGameDataInitialized()
        {
            base.OnGameDataInitialized();
            RefreshPage();
        }

        public override void OnButtonPressed(ButtonSetting setting)
        {
            base.OnButtonPressed(setting);
            ModControlPageSupport.HandleRefresh(setting, RefreshPage);
        }

        internal void RefreshPage()
        {
            var lines = ModControlRuntime.CreateStatus().DiagnosticsLines();
            ModControlPageSupport.SetLabel(Settings.Assembly, ModControlPageSupport.Line(lines, 0));
            ModControlPageSupport.SetLabel(Settings.Archive, ModControlPageSupport.Line(lines, 1));
            ModControlPageSupport.SetLabel(Settings.Registration, ModControlPageSupport.Line(lines, 2));
            ModControlPageSupport.SetLabel(Settings.TargetCount, ModControlPageSupport.Line(lines, 3));
            ModControlPageSupport.SetLabel(Settings.LogSource, ModControlPageSupport.Line(lines, 4));
            ModControlPageSupport.SetLabel(Settings.ReadOnlyNotice, ModControlPageSupport.Line(lines, 5));
            ModControlPageSupport.SetLabel(Settings.LastError, ModControlPageSupport.Line(lines, 6));
            ModControlPageSupport.SetLabel(Settings.Display, "显示：" + ModControlEnvironment.ScreenSummary());
            ModControlPageSupport.SetLabel(Settings.InputFocus, ModControlEnvironment.InputSummary());
            ModControlPageSupport.SetLabel(Settings.Performance, ModControlPerformance.Summary());
        }

        public sealed class DiagnosticsSettings
        {
            [FSDisplayName("刷新诊断")]
            public FButton Refresh { get; set; } = new FButton("刷新诊断", "refresh", null, true);

            public FLabel Assembly { get; set; } = new FLabel("正在读取诊断…", "diagnostics.assembly");
            public FLabel Archive { get; set; } = new FLabel("", "diagnostics.archive");
            public FLabel Registration { get; set; } = new FLabel("", "diagnostics.registration");
            public FLabel TargetCount { get; set; } = new FLabel("", "diagnostics.targets");
            public FLabel LogSource { get; set; } = new FLabel("", "diagnostics.log");
            public FLabel ReadOnlyNotice { get; set; } = new FLabel("", "diagnostics.readonly");
            public FLabel LastError { get; set; } = new FLabel("", "diagnostics.error");
            public FLabel Display { get; set; } = new FLabel("显示：正在读取…", "diagnostics.display");
            public FLabel InputFocus { get; set; } = new FLabel("输入：正在读取…", "diagnostics.input");
            public FLabel Performance { get; set; } = new FLabel("运行：正在读取…", "diagnostics.performance");
        }
    }
}
