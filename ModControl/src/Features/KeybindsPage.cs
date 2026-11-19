using System;
using System.Collections.Generic;
using System.Linq;
using PreLuna.ModControl.Adapters;
using PreLuna.ModControl.Core.Keybinds;
using PreLuna.ModControl.Core.Operations;
using PreLuna.ModControl.Runtime;
using TheArchive.Core.Attributes.Feature;
using TheArchive.Core.Attributes.Feature.Members;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI;
using TheArchive.Core.FeaturesAPI.Components;
using TheArchive.Core.FeaturesAPI.Settings;
using UnityEngine;

namespace PreLuna.ModControl.Features
{
    [EnableFeatureByDefault]
    public sealed class KeybindsPage : Feature
    {
        [FeatureConfig]
        public static KeybindSettings Settings { get; set; } = new KeybindSettings();

        static readonly KeybindAction[] Actions =
        {
            new("CameraToggle", "Camera", "key.toggle", "第三人称·切换第三人称"),
            new("CameraReset", "Camera", "key.reset", "第三人称·恢复镜头默认"),
            new("CameraSnapshot", "Camera", "key.snapshot", "第三人称·诊断快照"),
            new("CameraSwap", "Camera", "key.swapShoulder", "第三人称·左右换肩"),
            new("CameraPreset", "Camera", "key.cyclePreset", "第三人称·切换镜头风格"),
            new("CameraCapture", "Camera", "key.capture", "第三人称·性能采集"),
            new("CameraPointer", "Camera", "key.pointer", "第三人称·指向调试覆盖层"),
            new("CameraObserve", "Camera", "key.observe", "第三人称·站定观察"),
            new("HudVisibility", "Hud", "key.visibility", "第三人称 HUD·HUD显示／详情"),
            new("HudTeam", "Hud", "key.team", "第三人称 HUD·队伍信息"),
            new("HudStyle", "Hud", "key.style", "第三人称 HUD·循环HUD样式"),
            new("HudRestore", "Hud", "key.restore", "第三人称 HUD·恢复原版HUD"),
            new("HeadToggle", "Head", "key.toggle", "头戴摄像头·观察／接管共享入口"),
            new("HeadNext", "Head", "key.next", "头戴摄像头·下一位队友"),
            new("HeadPrevious", "Head", "key.previous", "头戴摄像头·上一位队友"),
            new("MicroEmergency", "Micro", "key.emergency", "微操大师·紧急退出接管"),
            new("MicroView", "Micro", "key.view", "微操大师·切换接管画面大小"),
            new("MicroReset", "Micro", "key.resetView", "微操大师·重置接管视角"),
        };
        readonly KeybindSession session = new(Actions, new Dictionary<string, Func<ITargetProvider>>
        {
            ["Camera"] = () => ThirdPersonCameraAdapterFactory.TryCreate(),
            ["Hud"] = () => ThirdPersonHudAdapterFactory.TryCreate(),
            ["Head"] = () => HeadCameraAdapterFactory.TryCreate(),
            ["Micro"] = () => MicroMasterAdapterFactory.TryCreate(),
        });
        float nextRefresh;
        bool wasOpen;
        string result = "";
        FeatureSettingsHelper helper;
        readonly List<LabelSetting> duplicateRows = new();

        public override string Name => "按键绑定";
        public override string Description => "集中管理已接入MOD的键盘／鼠标单键；点击键名后按新键，Esc清除绑定。";

        public override void Init() { base.Init(); RefreshPage(); }
        public override void OnEnable() { base.OnEnable(); RefreshPage(); }
        public override void OnGameDataInitialized() { base.OnGameDataInitialized(); RefreshPage(); }

        public override void Update()
        {
            base.Update();
            bool open = ModControlPageSupport.IsCurrentSubMenu("按键绑定") || ModControlPageSupport.IsCurrentSubMenu("重复占用详情");
            if (open && (!wasOpen || Time.unscaledTime >= nextRefresh))
            {
                nextRefresh = Time.unscaledTime + 2;
                RefreshPage();
            }
            wasOpen = open;
        }

        public override void OnFeatureSettingChanged(FeatureSetting setting)
        {
            base.OnFeatureSettingChanged(setting);
            ModControlPageSupport.ClearDirty(setting?.Helper);
            if (setting is not KeySetting key || !Actions.Any(a => a.Property == setting.Prop?.Name)) return;
            var requestedKey = key.GetCurrent();
            result = session.Set(setting.Prop.Name, (int)requestedKey);
            ModControlRuntime.RecordKeybindResult(setting.Prop.Name, requestedKey.ToString(), result);
            RefreshPage();
        }

        public override void OnButtonPressed(ButtonSetting setting)
        {
            base.OnButtonPressed(setting);
            ModControlPageSupport.HandleRefresh(setting, RefreshPage);
        }

        void RefreshPage()
        {
            session.Refresh();
            // The framework can reconstruct a settings instance; the shared key values
            // belong to neither its JSON config nor a particular menu instance.
            KeybindSettings.Sync(session.Values);
            ModControlPageSupport.SetLabel(Settings.Result, result);
            ModControlPageSupport.SetLabel(Settings.Coverage, "已接入 " + session.Values.Count + "/18 项；None表示不绑定，目标不可用时控件不保存。");
            ModControlPageSupport.SetLabel(Settings.CameraStatus, "第三人称：" + session.DescribeTarget("Camera"));
            ModControlPageSupport.SetLabel(Settings.HudStatus, "第三人称 HUD：" + session.DescribeTarget("Hud"));
            ModControlPageSupport.SetLabel(Settings.HeadStatus, "头戴摄像头：" + session.DescribeTarget("Head"));
            ModControlPageSupport.SetLabel(Settings.MicroStatus, "微操大师：" + session.DescribeTarget("Micro"));
            var duplicates = session.Duplicates();
            ModControlPageSupport.SetLabel(Settings.Duplicates, duplicates.Count == 0 ? "重复占用：当前已接入按键无重复" : "重复占用：" + duplicates.Count + " 个键（查看详情，并结合生效场景判断）");
            var labels = DuplicateSettings.Labels;
            var occupied = duplicates.SelectMany(d => d.Actions.Select(a => ((KeyCode)d.Code) + " 重复占用 · " + a.Name)).ToArray();
            for (int i = 0; i < labels.Length; i++)
                ModControlPageSupport.SetLabel(labels[i], i < occupied.Length ? occupied[i] : "");

            ModControlPageSupport.SetLabel(DuplicateSettings.SummaryLabel, occupied.Length == 0 ? "已接入按键当前无重复占用" : "重复允许保存；以下每行列出一个占用功能。");
            helper ??= SettingsHelpers?.FirstOrDefault();
            if (helper == null) return;
            var detailHelper = helper.Settings.OfType<SubmenuSetting>().FirstOrDefault(s => s.Prop?.Name == nameof(KeybindSettings.Conflicts))?.SettingsHelper;
            if (duplicateRows.Count == 0 && detailHelper != null)
                duplicateRows.AddRange(detailHelper.Settings.OfType<LabelSetting>().Where(s => s.Prop?.Name?.StartsWith("Duplicate", StringComparison.Ordinal) == true));
            foreach (var row in duplicateRows)
            {
                int index = int.Parse(row.Prop.Name.Substring("Duplicate".Length)) - 1;
                if (index < occupied.Length) detailHelper?.Settings.Add(row); else detailHelper?.Settings.Remove(row);
            }
            foreach (var key in helper.Settings.OfType<KeySetting>())
            {
                // Never overwrite the native "press a key" prompt during recording.
                var code = session.Values.TryGetValue(key.Prop.Name, out var current) ? current : 0;
                var text = ((KeyCode)code).ToString() + (duplicates.Any(d => d.Actions.Any(a => a.Property == key.Prop.Name)) ? " · 重复" : "");
                if (key.Key != text && !IsRecording(key)) key.UpdateKeyText(text);
            }
            ModControlPageSupport.ClearDirty(helper);
        }

        static bool IsRecording(KeySetting setting)
        {
            // Native recording text contains rich text; canonical KeyCode names do not.
            return setting.Key?.Contains("<") == true;
        }

        public sealed class KeybindSettings
        {
            static readonly Dictionary<string, KeyCode> values = new(StringComparer.Ordinal);
            internal static void Sync(IReadOnlyDictionary<string, int> source)
            {
                values.Clear();
                foreach (var pair in source) values[pair.Key] = (KeyCode)pair.Value;
            }
            static KeyCode Get(string property) => values.TryGetValue(property, out var value) ? value : KeyCode.None;
            static void Set(string property, KeyCode value) => values[property] = value;

            [FSDisplayName("刷新绑定与占用")]
            public FButton Refresh { get; set; } = new FButton("刷新", "refresh", null, true);
            public FLabel Help { get; set; } = new FLabel("点击键名后按新键；Esc清除为None；每次修改立即保存。", "keys.help");
            public FLabel Boundary { get; set; } = new FLabel("仅检查下列18项；重复允许保存，不自动换键。原版、其他MOD及组合键未纳入检测。", "keys.boundary");
            public FLabel Coverage { get; set; } = new FLabel("正在读取…", "keys.coverage");
            public FLabel Result { get; set; } = new FLabel("", "keys.result");
            public FLabel Duplicates { get; set; } = new FLabel("", "keys.duplicates");
            [FSDisplayName("重复占用详情"), FSUseDynamicSubmenu]
            public DuplicateSettings Conflicts { get; set; } = new DuplicateSettings();

            [FSHeader("第三人称")]
            public FLabel CameraStatus { get; set; } = new FLabel("正在读取…", "keys.Camera");
            [FSDisplayName("切换第三人称"), FSDescription("按下切换；本体可操作时。设为None关闭此绑定。") ]
            public KeyCode CameraToggle { get => Get(nameof(CameraToggle)); set => Set(nameof(CameraToggle), value); }
            [FSDisplayName("恢复镜头默认"), FSDescription("按下恢复镜头参数并关闭第三人称；也会恢复部分镜头按键默认值。设为None关闭此绑定。") ]
            public KeyCode CameraReset { get => Get(nameof(CameraReset)); set => Set(nameof(CameraReset), value); }
            [FSDisplayName("诊断快照"), FSDescription("按下记录诊断；本体FPS输入有效时。设为None关闭此绑定。") ]
            public KeyCode CameraSnapshot { get => Get(nameof(CameraSnapshot)); set => Set(nameof(CameraSnapshot), value); }
            [FSDisplayName("左右换肩"), FSDescription("按下切换肩位；第三人称有效时。设为None关闭此绑定。") ]
            public KeyCode CameraSwap { get => Get(nameof(CameraSwap)); set => Set(nameof(CameraSwap), value); }
            [FSDisplayName("切换镜头风格"), FSDescription("按下循环预设；第三人称有效时。设为None关闭此绑定。") ]
            public KeyCode CameraPreset { get => Get(nameof(CameraPreset)); set => Set(nameof(CameraPreset), value); }
            [FSDisplayName("性能采集"), FSDescription("按下开启／结束采样；本体FPS输入有效时。设为None关闭此绑定。") ]
            public KeyCode CameraCapture { get => Get(nameof(CameraCapture)); set => Set(nameof(CameraCapture), value); }
            [FSDisplayName("指向调试覆盖层"), FSDescription("按下切换诊断覆盖层；FPS输入有效时。设为None关闭此绑定。") ]
            public KeyCode CameraPointer { get => Get(nameof(CameraPointer)); set => Set(nameof(CameraPointer), value); }
            [FSDisplayName("站定观察"), FSDescription("按住生效；第三人称且满足站定观察条件。设为None关闭此绑定。") ]
            public KeyCode CameraObserve { get => Get(nameof(CameraObserve)); set => Set(nameof(CameraObserve), value); }

            [FSHeader("第三人称 HUD")]
            public FLabel HudStatus { get; set; } = new FLabel("正在读取…", "keys.Hud");
            [FSDisplayName("HUD显示／详情"), FSDescription("按现有配置切换HUD，或按住／切换详情；第三人称有效时。设为None关闭此绑定。") ]
            public KeyCode HudVisibility { get => Get(nameof(HudVisibility)); set => Set(nameof(HudVisibility), value); }
            [FSDisplayName("队伍信息"), FSDescription("按下切换队伍信息；第三人称HUD有效时。设为None关闭此绑定。") ]
            public KeyCode HudTeam { get => Get(nameof(HudTeam)); set => Set(nameof(HudTeam), value); }
            [FSDisplayName("循环HUD样式"), FSDescription("按下循环样式；第三人称HUD有效时。设为None关闭此绑定。") ]
            public KeyCode HudStyle { get => Get(nameof(HudStyle)); set => Set(nameof(HudStyle), value); }
            [FSDisplayName("恢复原版HUD"), FSDescription("按下恢复／重新允许HUD替代；FPS输入有效时。设为None关闭此绑定。") ]
            public KeyCode HudRestore { get => Get(nameof(HudRestore)); set => Set(nameof(HudRestore), value); }

            [FSHeader("头戴摄像头")]
            public FLabel HeadStatus { get; set; } = new FLabel("正在读取…", "keys.Head");
            [FSDisplayName("观察／接管共享入口"), FSDescription("单击观察、双击请求接管、接管中按下退出；由头戴摄像头统一识别，微操不另设入口。设为None关闭此绑定。") ]
            public KeyCode HeadToggle { get => Get(nameof(HeadToggle)); set => Set(nameof(HeadToggle), value); }
            [FSDisplayName("下一位队友"), FSDescription("按下切换；观察小窗打开且未接管时。设为None关闭此绑定。") ]
            public KeyCode HeadNext { get => Get(nameof(HeadNext)); set => Set(nameof(HeadNext), value); }
            [FSDisplayName("上一位队友"), FSDescription("按下切换；观察小窗打开且未接管时。设为None关闭此绑定。") ]
            public KeyCode HeadPrevious { get => Get(nameof(HeadPrevious)); set => Set(nameof(HeadPrevious), value); }

            [FSHeader("微操大师")]
            public FLabel MicroStatus { get; set; } = new FLabel("正在读取…", "keys.Micro");
            [FSDisplayName("紧急退出接管"), FSDescription("按下退出；等待或已接管且FPS输入有效时；Esc保留原有退出能力。设为None关闭此绑定。") ]
            public KeyCode MicroEmergency { get => Get(nameof(MicroEmergency)); set => Set(nameof(MicroEmergency), value); }
            [FSDisplayName("切换接管画面大小"), FSDescription("按下切换；已接管时。设为None关闭此绑定。") ]
            public KeyCode MicroView { get => Get(nameof(MicroView)); set => Set(nameof(MicroView), value); }
            [FSDisplayName("重置接管视角"), FSDescription("按下回正；已接管时。设为None关闭此绑定。") ]
            public KeyCode MicroReset { get => Get(nameof(MicroReset)); set => Set(nameof(MicroReset), value); }

        }

        public sealed class DuplicateSettings
        {
            internal static FLabel SummaryLabel = new FLabel("", "keys.detailSummary");
            internal static readonly FLabel[] Labels = Enumerable.Range(1, 18).Select(i => new FLabel("", "keys.duplicate" + i)).ToArray();
            public FLabel Summary { get => SummaryLabel; set => SummaryLabel = value; }
            public FLabel Duplicate01 { get => Labels[0]; set => Labels[0] = value; }
            public FLabel Duplicate02 { get => Labels[1]; set => Labels[1] = value; }
            public FLabel Duplicate03 { get => Labels[2]; set => Labels[2] = value; }
            public FLabel Duplicate04 { get => Labels[3]; set => Labels[3] = value; }
            public FLabel Duplicate05 { get => Labels[4]; set => Labels[4] = value; }
            public FLabel Duplicate06 { get => Labels[5]; set => Labels[5] = value; }
            public FLabel Duplicate07 { get => Labels[6]; set => Labels[6] = value; }
            public FLabel Duplicate08 { get => Labels[7]; set => Labels[7] = value; }
            public FLabel Duplicate09 { get => Labels[8]; set => Labels[8] = value; }
            public FLabel Duplicate10 { get => Labels[9]; set => Labels[9] = value; }
            public FLabel Duplicate11 { get => Labels[10]; set => Labels[10] = value; }
            public FLabel Duplicate12 { get => Labels[11]; set => Labels[11] = value; }
            public FLabel Duplicate13 { get => Labels[12]; set => Labels[12] = value; }
            public FLabel Duplicate14 { get => Labels[13]; set => Labels[13] = value; }
            public FLabel Duplicate15 { get => Labels[14]; set => Labels[14] = value; }
            public FLabel Duplicate16 { get => Labels[15]; set => Labels[15] = value; }
            public FLabel Duplicate17 { get => Labels[16]; set => Labels[16] = value; }
            public FLabel Duplicate18 { get => Labels[17]; set => Labels[17] = value; }

        }
    }
}
