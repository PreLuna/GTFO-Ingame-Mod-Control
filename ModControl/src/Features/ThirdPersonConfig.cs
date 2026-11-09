using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI.Components;

namespace PreLuna.ModControl.Features
{
    public sealed class ThirdPersonConfig : IWritableTargetSettings
    {
        // TheArchive recreates nested settings objects when reloading feature config.
        // Keep the values shared so page rebuilds read the same synchronized state.
        static float distance = 2.25f;
        static float shoulderOffset = .48f;
        static float pivotHeightOffset = .1f;
        static float shoulderDistance = 1.35f;
        static bool leftShoulder;

        public FLabel Status { get; set; } = new FLabel("第三人称镜头：正在读取…", "third.status");
        public FLabel Result { get; set; } = new FLabel("", "third.result");
        public FLabel CurrentValues { get; set; } = new FLabel("当前生效：", "third.current");
        public FLabel SavedValues { get; set; } = new FLabel("已保存：", "third.saved");

        [FSDisplayName("镜头距离（米）")]
        [FSSlider(.6f, 4f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float Distance
        {
            get => distance;
            set => distance = value;
        }

        [FSDisplayName("右肩偏移（米）")]
        [FSSlider(-1f, 1f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float ShoulderOffset
        {
            get => shoulderOffset;
            set => shoulderOffset = value;
        }

        [FSDisplayName("支点高度偏移（米）")]
        [FSSlider(-.35f, .6f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float PivotHeightOffset
        {
            get => pivotHeightOffset;
            set => pivotHeightOffset = value;
        }

        [FSDisplayName("肩射距离（米）")]
        [FSSlider(.6f, 3f, FSSlider.SliderStyle.FloatTwoDecimal, FSSlider.RoundTo.TwoDecimal)]
        public float ShoulderDistance
        {
            get => shoulderDistance;
            set => shoulderDistance = value;
        }

        [FSDisplayName("左肩")]
        public bool LeftShoulder
        {
            get => leftShoulder;
            set => leftShoulder = value;
        }

        public void ApplySnapshot(TargetSnapshot snapshot)
        {
            if (snapshot == null) return;
            Distance = (float)TargetValueReader.Number(snapshot, "distance", 2.25);
            ShoulderOffset = (float)TargetValueReader.Number(snapshot, "shoulder", .48);
            PivotHeightOffset = (float)TargetValueReader.Number(snapshot, "height", .1);
            ShoulderDistance = (float)TargetValueReader.Number(snapshot, "shoulderDistance", 1.35);
            LeftShoulder = TargetValueReader.Bool(snapshot, "leftShoulder", false);
        }
    }
}
