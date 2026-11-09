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
    public sealed class OverviewPage : Feature
    {
        [FeatureConfig]
        public static OverviewSettings Settings { get; set; } = new OverviewSettings();

        public override string Name => "概览";
        public override string Description => "只读状态在左侧“设置 → 进入”中查看；右侧只显示功能说明。";

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
            var lines = ModControlRuntime.CreateStatus().OverviewLines();
            ModControlPageSupport.SetLabel(Settings.ModuleVersion, ModControlPageSupport.Line(lines, 0));
            ModControlPageSupport.SetLabel(Settings.FrameworkVersion, ModControlPageSupport.Line(lines, 1));
            ModControlPageSupport.SetLabel(Settings.FrameworkState, ModControlPageSupport.Line(lines, 2));
            ModControlPageSupport.SetLabel(Settings.Registration, ModControlPageSupport.Line(lines, 3));
            ModControlPageSupport.SetLabel(Settings.TargetCount, ModControlPageSupport.Line(lines, 4));
            ModControlPageSupport.SetLabel(Settings.TargetMode, ModControlPageSupport.Line(lines, 5));
            ModControlPageSupport.SetLabel(Settings.ReadOnlyNotice, ModControlPageSupport.Line(lines, 6));
            ModControlPageSupport.SetLabel(Settings.LastError, ModControlPageSupport.Line(lines, 7));
        }

        public sealed class OverviewSettings
        {
            [FSDisplayName("刷新状态")]
            public FButton Refresh { get; set; } = new FButton("刷新状态", "refresh", null, true);

            public FLabel ModuleVersion { get; set; } = new FLabel("正在读取状态…", "overview.module");
            public FLabel FrameworkVersion { get; set; } = new FLabel("", "overview.framework");
            public FLabel FrameworkState { get; set; } = new FLabel("", "overview.state");
            public FLabel Registration { get; set; } = new FLabel("", "overview.registration");
            public FLabel TargetCount { get; set; } = new FLabel("", "overview.targets");
            public FLabel TargetMode { get; set; } = new FLabel("", "overview.mode");
            public FLabel ReadOnlyNotice { get; set; } = new FLabel("", "overview.readonly");
            public FLabel LastError { get; set; } = new FLabel("", "overview.error");
        }
    }
}
