using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class MicroMasterConfig : IWritableTargetSettings
    {
        static bool enabled = true;
        static float doubleClickWindow = .3f;
        static float walkHaste = .25f;
        static float mouseSensitivity = 1f;
        static bool invertMouse;
        static bool nativeLight = true;

        public FLabel Status { get; set; } = new FLabel("微操大师：正在读取…", "micro.status");
        public FLabel Notice { get; set; } = new FLabel("主机提示：仅主机实际生效；客机改本地CFG不影响主机。", "micro.notice");
        public FLabel Result { get; set; } = new FLabel("", "micro.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前生效：", "micro.current");
        public FLabel SavedValues { get; set; } = new FLabel("已保存：", "micro.saved");

        [FSDisplayName("启用")]
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        [FSDisplayName("双击间隔（秒）")]
        [FSSlider(.18f, .6f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float DoubleClickWindow
        {
            get => doubleClickWindow;
            set => doubleClickWindow = value;
        }

        [FSDisplayName("步行速度档位")]
        [FSSlider(.05f, .5f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float WalkHaste
        {
            get => walkHaste;
            set => walkHaste = value;
        }

        [FSDisplayName("鼠标灵敏度")]
        [FSSlider(.05f, 10f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float MouseSensitivity
        {
            get => mouseSensitivity;
            set => mouseSensitivity = value;
        }

        [FSDisplayName("反转纵轴")]
        public bool InvertMouse
        {
            get => invertMouse;
            set => invertMouse = value;
        }

        [FSDisplayName("原生手电常亮")]
        public bool NativeLight
        {
            get => nativeLight;
            set => nativeLight = value;
        }

        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            if (snapshot == null) return;
            Enabled = TargetValueReader.Bool(snapshot, "enabled", true);
            DoubleClickWindow = (float)TargetValueReader.Number(snapshot, "doubleClickWindow", .3);
            WalkHaste = (float)TargetValueReader.Number(snapshot, "walkHaste", .25);
            MouseSensitivity = (float)TargetValueReader.Number(snapshot, "mouseSensitivity", 1);
            InvertMouse = TargetValueReader.Bool(snapshot, "invertMouse", false);
            NativeLight = TargetValueReader.Bool(snapshot, "nativeLight", true);
        }
    }
}
