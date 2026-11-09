using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class HeadCameraConfig : IWritableTargetSettings
    {
        static bool enabled = true;
        static float renderWidth = 1280;
        static float fps = 30;
        static float fieldOfView = 80;
        static float viewDistance = 80;
        static float windowWidth = 480;

        public FLabel Status { get; set; } = new FLabel("头戴摄像头：正在读取…", "headcam.status");
        public FLabel Notice { get; set; } = new FLabel("本机提示：仅本机生效；客机修改不影响主机。", "headcam.notice");
        public FLabel Result { get; set; } = new FLabel("", "headcam.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前生效：", "headcam.current");
        public FLabel SavedValues { get; set; } = new FLabel("已保存：", "headcam.saved");

        [FSDisplayName("启用")]
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        [FSDisplayName("渲染宽度")]
        [FSSlider(160f, 1920f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float RenderWidth
        {
            get => renderWidth;
            set => renderWidth = value;
        }

        [FSDisplayName("刷新帧率")]
        [FSSlider(5f, 60f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float FramesPerSecond
        {
            get => fps;
            set => fps = value;
        }

        [FSDisplayName("视野角度")]
        [FSSlider(45f, 110f, FSSlider.SliderStyle.FloatOneDecimal, FSSlider.RoundTo.OneDecimal)]
        public float FieldOfView
        {
            get => fieldOfView;
            set => fieldOfView = value;
        }

        [FSDisplayName("最大绘制距离（米）")]
        [FSSlider(15f, 120f, FSSlider.SliderStyle.FloatOneDecimal, FSSlider.RoundTo.OneDecimal)]
        public float ViewDistance
        {
            get => viewDistance;
            set => viewDistance = value;
        }

        [FSDisplayName("小窗宽度")]
        [FSSlider(200f, 960f, FSSlider.SliderStyle.FloatNoDecimal, FSSlider.RoundTo.NoDecimal)]
        public float WindowWidth
        {
            get => windowWidth;
            set => windowWidth = value;
        }

        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            if (snapshot == null) return;
            Enabled = TargetValueReader.Bool(snapshot, "enabled", true);
            RenderWidth = (float)TargetValueReader.Number(snapshot, "width", 1280);
            FramesPerSecond = (float)TargetValueReader.Number(snapshot, "fps", 30);
            FieldOfView = (float)TargetValueReader.Number(snapshot, "fov", 80);
            ViewDistance = (float)TargetValueReader.Number(snapshot, "viewDistance", 80);
            WindowWidth = (float)TargetValueReader.Number(snapshot, "windowWidth", 480);
        }
    }
}
