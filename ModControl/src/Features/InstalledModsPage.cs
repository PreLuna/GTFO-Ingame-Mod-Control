using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CellMenu;
using PreLuna.ModControl.Adapters;
using PreLuna.ModControl.Core;
using PreLuna.ModControl.Runtime;
using TheArchive.Core.Attributes.Feature;
using TheArchive.Core.Attributes.Feature.Members;
using TheArchive.Core.Attributes.Feature.Patches;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI;
using TheArchive.Core.FeaturesAPI.Components;
using TheArchive.Core.FeaturesAPI.Settings;

namespace PreLuna.ModControl.Features
{
    [EnableFeatureByDefault]
    public sealed class InstalledModsPage : Feature
    {
        const int MaxRows = 24;

        static readonly HashSet<string> AdaptedTargetGuids = new HashSet<string>(StringComparer.Ordinal)
        {
            "PreLuna.EnemyHealthBar", "PreLuna.ThirdPersonHUD", "PreLuna.ThirdPerson",
            "PreLuna.HeadCamera", "PreLuna.MicroMaster", "PreLuna.Captain",
            "PreLuna.FoamGunCustom", "PreLuna.EnemyHealthBar.DamageNumbers",
            "PreLuna.FlashlightCustom", "PreLuna.PrisonerMovementCustom", "PreLuna.PrisonerHealthCustom",
            "PreLuna.BioScannerCustom", "PreLuna.CoopBotsCustom"
        };

        static readonly IReadOnlyDictionary<string, string> EnemyHealthBarKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Enabled"] = "enabled",
                ["MaxDistance"] = "maxDistance",
                ["Width"] = "width",
                ["Height"] = "height",
                ["Opacity"] = "opacity",
                ["ShowName"] = "showName",
                ["ShowNumbers"] = "showNumbers",
                ["ShowStatusSlots"] = "showStatusSlots"
            };

        static readonly IReadOnlyDictionary<string, string> ThirdPersonHudKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Panel"] = "panel",
                ["Personal"] = "personal",
                ["Team"] = "team",
                ["Scene"] = "scene",
                ["Alerts"] = "alerts"
            };

        static readonly IReadOnlyDictionary<string, string> ThirdPersonKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Distance"] = "distance",
                ["ShoulderOffset"] = "shoulder",
                ["PivotHeightOffset"] = "height",
                ["ShoulderDistance"] = "shoulderDistance",
                ["LeftShoulder"] = "leftShoulder"
            };

        static readonly IReadOnlyDictionary<string, string> CaptainChineseKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ChineseMenus"] = "chineseMenus",
                ["ChineseItems"] = "chineseItems"
            };

        static readonly IReadOnlyDictionary<string, string> HeadCameraKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Enabled"] = "enabled",
                ["RenderWidth"] = "width",
                ["FramesPerSecond"] = "fps",
                ["FieldOfView"] = "fov",
                ["ViewDistance"] = "viewDistance",
                ["WindowWidth"] = "windowWidth"
            };

        static readonly IReadOnlyDictionary<string, string> FoamGunKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Enabled"] = "enabled",
                ["ShotInterval"] = "shotInterval",
                ["LaunchForce"] = "launchForce",
                ["PelletSpread"] = "pelletSpread",
                ["AmmoCost"] = "ammoCost"
            };

        static readonly IReadOnlyDictionary<string, string> DamageNumbersKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Enabled"] = "enabled",
                ["MaxRows"] = "maxRows",
                ["Lifetime"] = "lifetime",
                ["FontSize"] = "fontSize",
                ["MaxDistance"] = "maxDistance",
                ["ShowArmor"] = "showArmor"
            };

        static readonly IReadOnlyDictionary<string, string> FlashlightRestartKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Enabled"] = "enabled", ["AttachLightToWeaponBone"] = "attach", ["AngleStep"] = "angleStep",
                ["HelmetAngle"] = "helmetAngle", ["HelmetIntensity"] = "helmetIntensity", ["HelmetRange"] = "helmetRange",
                ["PerlinFlicker"] = "perlin", ["RareFlicker"] = "rare", ["HorrorFlicker"] = "horror"
            };

        static readonly IReadOnlyDictionary<string, string> PrisonerMovementRestartKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Recoil"] = "recoil", ["SuperSlide"] = "superSlide", ["RunReload"] = "runReload"
            };

        static readonly IReadOnlyDictionary<string, string> PrisonerHealthRestartKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["FallDamage"] = "fallDamage", ["MinHeight"] = "minHeight", ["MaxHeight"] = "maxHeight",
                ["MaxHealth"] = "maxHealth", ["RegenRate"] = "regenRate", ["RegenCap"] = "regenCap", ["RegenDelay"] = "regenDelay"
            };

        static readonly IReadOnlyDictionary<string, string> BioScannerRestartKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["OffsetX"] = "offsetX", ["OffsetY"] = "offsetY"
            };

        static readonly IReadOnlyDictionary<string, string> CoopBotsRestartKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["FastFollow"] = "fastFollow", ["TaskTarget"] = "taskTarget", ["TaskReturn"] = "taskReturn",
                ["ProtectScans"] = "protectScans", ["Weakpoint"] = "weakpoint", ["Containers"] = "containers",
                ["Teleport"] = "teleport", ["CombatAwareness"] = "combatAwareness", ["CombatDecisions"] = "combatDecisions",
                ["FollowDistance"] = "followDistance"
            };

        static readonly IReadOnlyDictionary<string, string> CaptainBotRestartKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Booster"] = "booster", ["Tasks"] = "tasks", ["TaskTeleport"] = "teleport",
                ["StationRadius"] = "stationRadius", ["KillDistance"] = "killDistance", ["PickupDistance"] = "pickupDistance"
            };

        static readonly IReadOnlyDictionary<string, string> MicroMasterKeys =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Enabled"] = "enabled",
                ["DoubleClickWindow"] = "doubleClickWindow",
                ["WalkHaste"] = "walkHaste",
                ["MouseSensitivity"] = "mouseSensitivity",
                ["InvertMouse"] = "invertMouse",
                ["NativeLight"] = "nativeLight"
            };

        [FeatureConfig]
        public static InstalledModsSettings Settings { get; set; } = new InstalledModsSettings();

        FeatureSettingsHelper settingsHelper;
        readonly Dictionary<string, FeatureSetting> allSettings = new();
        readonly List<TargetPageBinding> targetPages = new();
        bool pagesBuilt;
        bool pendingSliderRegistration;
        float nextPageStateCheck;
        float nextPageRefresh;
        float nextListRefresh;

        public override string Name => "已接入 MOD";
        public override string Description => "已加载 PreLuna 插件列表；已适配目标从列表上方的配置子菜单进入，未适配目标只显示信息。";

        public override void Init()
        {
            base.Init();
            EnsurePages();
            RefreshPage();
        }

        public override void Update()
        {
            base.Update();
            var started = UnityEngine.Time.realtimeSinceStartup;
            EnsurePages();

            if (started >= nextPageStateCheck)
            {
                nextPageStateCheck = started + .25f;
                UpdatePageLifecycle();
            }

            if (started >= nextPageRefresh)
            {
                nextPageRefresh = started + 2f;
                foreach (var binding in targetPages)
                    if (binding.WasOpen) RefreshPage(binding);
            }

            if (pendingSliderRegistration && SliderActualValueDisplay.Register(FindSubmenuHelper("EnemyHealthBar")))
                pendingSliderRegistration = false;

            if (started >= nextListRefresh)
            {
                nextListRefresh = started + 2f;
                RefreshList();
            }

            ModControlPerformance.RecordUpdate((UnityEngine.Time.realtimeSinceStartup - started) * 1000f);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            EnsurePages();
            RefreshPage();
        }

        public override void OnDisable()
        {
            foreach (var binding in targetPages)
            {
                if (binding.WasOpen)
                {
                    binding.WasOpen = false;
                    binding.Page.OnClosed();
                    binding.Closed?.Invoke();
                }
            }

            SliderActualValueDisplay.Clear();
            pendingSliderRegistration = false;
            base.OnDisable();
        }

        public override void OnGameDataInitialized()
        {
            base.OnGameDataInitialized();
            RefreshPage();
        }

        public override void OnFeatureSettingChanged(FeatureSetting setting)
        {
            base.OnFeatureSettingChanged(setting);
            if (setting == null) return;
            ModControlPageSupport.ClearDirty(setting.Helper);

            var declaringType = setting.Prop?.DeclaringType;
            if (declaringType == null) return;
            foreach (var binding in targetPages)
            {
                if (binding.SettingsType != declaringType) continue;
                var settings = ResolveSettings(binding);
                if (settings != null) binding.Page.OnSettingChanged(setting, settings);
                return;
            }
        }

        public override void OnButtonPressed(ButtonSetting setting)
        {
            base.OnButtonPressed(setting);
            ModControlPageSupport.HandleRefresh(setting, RefreshPage);
        }

        internal void RefreshPage()
        {
            EnsurePages();
            RefreshList();
            foreach (var binding in targetPages) RefreshPage(binding);
        }

        void RefreshPage(TargetPageBinding binding)
        {
            var settings = ResolveSettings(binding);
            if (settings == null) return;
            binding.Page.Refresh(settings);
            binding.Refreshed?.Invoke();
            ModControlPerformance.RecordPageRefresh();
        }

        void UpdatePageLifecycle()
        {
            foreach (var binding in targetPages)
            {
                var open = ModControlPageSupport.IsCurrentSubMenu(binding.SubmenuTitleFragment);
                if (open)
                {
                    if (!binding.Initialized)
                    {
                        binding.Initialized = true;
                        binding.Opened?.Invoke();
                        binding.Page.OnOpened();
                        RefreshPage(binding);
                        ModControlPageSupport.RefreshCurrentSubMenu();
                    }
                    else if (!binding.WasOpen)
                    {
                        binding.Opened?.Invoke();
                        binding.Page.OnOpened();
                        RefreshPage(binding);
                    }

                    binding.WasOpen = true;
                }
                else if (binding.WasOpen)
                {
                    binding.WasOpen = false;
                    binding.Page.OnClosed();
                    binding.Closed?.Invoke();
                }
            }
        }

        void EnsurePages()
        {
            if (pagesBuilt) return;
            pagesBuilt = true;

            targetPages.Add(new TargetPageBinding(
                "EnemyHealthBar",
                "敌人血条",
                typeof(EnemyHealthBarConfig),
                () => Settings.EnemyHealthBar,
                new WritableTargetPage<EnemyHealthBarConfig>(
                    "敌人血条",
                    ModControlInfo.EnemyHealthBarVersion,
                    EnemyHealthBarAdapterFactory.TryCreate,
                    EnemyHealthBarKeys),
                () => pendingSliderRegistration = true,
                () =>
                {
                    pendingSliderRegistration = false;
                    SliderActualValueDisplay.Clear();
                },
                () => SliderActualValueDisplay.RefreshText()));

            targetPages.Add(new TargetPageBinding(
                "ThirdPersonHud",
                "第三人称HUD",
                typeof(ThirdPersonHudConfig),
                () => Settings.ThirdPersonHud,
                new WritableTargetPage<ThirdPersonHudConfig>(
                    "第三人称 HUD",
                    ModControlInfo.ThirdPersonHudVersion,
                    ThirdPersonHudAdapterFactory.TryCreate,
                    ThirdPersonHudKeys)));

            targetPages.Add(new TargetPageBinding(
                "PrisonerMovement",
                "移动与生命",
                typeof(PrisonerMovementRestartConfig),
                () => Settings.PrisonerMovement,
                new RestartCfgPage<PrisonerMovementRestartConfig>(RestartCfgTargets.PrisonerMovement, PrisonerMovementRestartKeys)));

            targetPages.Add(new TargetPageBinding(
                "Flashlight",
                "手电筒大修",
                typeof(FlashlightRestartConfig),
                () => Settings.Flashlight,
                new RestartCfgPage<FlashlightRestartConfig>(RestartCfgTargets.Flashlight, FlashlightRestartKeys)));

            targetPages.Add(new TargetPageBinding(
                "PrisonerHealth",
                "生命调整",
                typeof(PrisonerHealthRestartConfig),
                () => Settings.PrisonerHealth,
                new RestartCfgPage<PrisonerHealthRestartConfig>(RestartCfgTargets.PrisonerHealth, PrisonerHealthRestartKeys)));

            targetPages.Add(new TargetPageBinding(
                "BioScanner",
                "生物扫描",
                typeof(BioScannerRestartConfig),
                () => Settings.BioScanner,
                new RestartCfgPage<BioScannerRestartConfig>(RestartCfgTargets.BioScanner, BioScannerRestartKeys)));

            targetPages.Add(new TargetPageBinding(
                "CoopBots",
                "协同机器人",
                typeof(CoopBotsRestartConfig),
                () => Settings.CoopBots,
                new RestartCfgPage<CoopBotsRestartConfig>(RestartCfgTargets.CoopBots, CoopBotsRestartKeys)));

            targetPages.Add(new TargetPageBinding(
                "Captain",
                "协同队长",
                typeof(CaptainReadOnlyConfig),
                () => Settings.Captain,
                new ReadOnlyCfgPage<CaptainReadOnlyConfig>(ReadOnlyCfgTargets.Captain)));

            targetPages.Add(new TargetPageBinding(
                "CaptainBot",
                "协同队长·bot/强化剂（重启生效）",
                typeof(CaptainBotRestartConfig),
                () => Settings.CaptainBot,
                new RestartCfgPage<CaptainBotRestartConfig>(RestartCfgTargets.CaptainBot, CaptainBotRestartKeys)));

            targetPages.Add(new TargetPageBinding(
                "CaptainChinese",
                "协同队长·中文显示（重启生效）",
                typeof(CaptainChineseConfig),
                () => Settings.CaptainChinese,
                new WritableTargetPage<CaptainChineseConfig>(
                    "协同队长·中文显示",
                    ModControlInfo.CaptainVersion,
                    CaptainChineseAdapterFactory.TryCreate,
                    CaptainChineseKeys)));

            targetPages.Add(new TargetPageBinding(
                "HeadCamera",
                "头戴摄像头",
                typeof(HeadCameraConfig),
                () => Settings.HeadCamera,
                new WritableTargetPage<HeadCameraConfig>(
                    "头戴摄像头",
                    ModControlInfo.HeadCameraVersion,
                    HeadCameraAdapterFactory.TryCreate,
                    HeadCameraKeys)));

            targetPages.Add(new TargetPageBinding(
                "MicroMaster",
                "微操大师",
                typeof(MicroMasterConfig),
                () => Settings.MicroMaster,
                new WritableTargetPage<MicroMasterConfig>(
                    "微操大师",
                    ModControlInfo.MicroMasterVersion,
                    MicroMasterAdapterFactory.TryCreate,
                    MicroMasterKeys)));

            targetPages.Add(new TargetPageBinding(
                "ThirdPerson",
                "第三人称镜头",
                typeof(ThirdPersonConfig),
                () => Settings.ThirdPerson,
                new WritableTargetPage<ThirdPersonConfig>(
                    "第三人称镜头",
                    ModControlInfo.ThirdPersonVersion,
                    ThirdPersonCameraAdapterFactory.TryCreate,
                    ThirdPersonKeys)));

            targetPages.Add(new TargetPageBinding(
                "FoamGun",
                "泡沫枪调整",
                typeof(FoamGunConfig),
                () => Settings.FoamGun,
                new WritableTargetPage<FoamGunConfig>(
                    "泡沫枪调整",
                    ModControlInfo.FoamGunVersion,
                    FoamGunAdapterFactory.TryCreate,
                    FoamGunKeys)));

            targetPages.Add(new TargetPageBinding(
                "DamageNumbers",
                "伤害数字",
                typeof(DamageNumbersConfig),
                () => Settings.DamageNumbers,
                new WritableTargetPage<DamageNumbersConfig>(
                    "伤害数字",
                    ModControlInfo.DamageNumbersVersion,
                    DamageNumbersAdapterFactory.TryCreate,
                    DamageNumbersKeys)));
        }

        object ResolveSettings(TargetPageBinding binding)
        {
            var live = FindSubmenuConfig(binding.SubmenuPropertyName, binding.SettingsType);
            return live ?? binding.DefaultSettings?.Invoke();
        }

        void RefreshList()
        {
            var status = ModControlRuntime.CreateStatus();
            var targets = status.Targets;
            EnsureSettings();
            var adaptedCount = targets.Count(target => AdaptedTargetGuids.Contains(target.Guid));
            ModControlPageSupport.SetLabel(Settings.Summary, "已加载 PreLuna 插件：" + targets.Count + " 个；其中 " + adaptedCount + " 个已在上方提供配置");

            for (var index = 0; index < MaxRows; index++)
            {
                ModControlPageSupport.SetLabel(
                    Settings.TargetAt(index),
                    index < targets.Count ? FormatTarget(targets[index], AdaptedTargetGuids.Contains(targets[index].Guid)) : string.Empty);
            }

            ModControlPageSupport.SetLabel(
                Settings.Hint,
                targets.Count > MaxRows
                    ? "最多显示 " + MaxRows + " 个；另有 " + (targets.Count - MaxRows) + " 个未展开。"
                    : "上方是配置入口；下方是插件清单，[已适配] 仅表示同一插件可在上方配置，不是重复安装。");
            ModControlPageSupport.SetLabel(
                Settings.LastError,
                "最近错误：" + (string.IsNullOrWhiteSpace(status.LastError) ? "无" : status.LastError));

            OrderRows(targets.Count);
        }

        FeatureSettingsHelper FindSubmenuHelper(string propertyName)
        {
            EnsureSettings();
            if (settingsHelper != null)
            {
                foreach (var setting in settingsHelper.Settings)
                {
                    if (setting is SubmenuSetting submenu &&
                        string.Equals(submenu.Prop?.Name, propertyName, StringComparison.Ordinal))
                    {
                        return submenu.SettingsHelper;
                    }
                }
            }

            return null;
        }

        object FindSubmenuConfig(string propertyName, Type expectedType)
        {
            var instance = HelperInstance(FindSubmenuHelper(propertyName));
            return instance != null && expectedType.IsInstanceOfType(instance) ? instance : null;
        }

        static object HelperInstance(FeatureSettingsHelper helper)
        {
            if (helper == null) return null;
            try
            {
                var property = typeof(FeatureSettingsHelper).GetProperty(
                    "Instance",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                return property?.GetValue(helper);
            }
            catch
            {
                return null;
            }
        }

        void EnsureSettings()
        {
            settingsHelper ??= SettingsHelpers?.FirstOrDefault();
            if (settingsHelper == null) return;
            foreach (var setting in settingsHelper.Settings) allSettings[setting.Prop.Name] = setting;
        }

        void OrderRows(int targetCount)
        {
            if (settingsHelper == null) return;
            for (var index = 0; index < MaxRows; index++)
            {
                var name = "Target" + (index + 1).ToString("00", CultureInfo.InvariantCulture);
                if (!allSettings.TryGetValue(name, out var setting)) continue;
                if (index < targetCount) settingsHelper.Settings.Add(setting);
                else settingsHelper.Settings.Remove(setting);
            }
        }

        static string FormatTarget(InstalledPlugin target, bool adapted)
        {
            var version = string.IsNullOrWhiteSpace(target.Version) ? "未知版本" : target.Version;
            var name = string.IsNullOrWhiteSpace(target.Name) ? string.Empty : "  " + target.Name;
            return "- " + (adapted ? "[已适配] " : "") + target.Guid + "  " + version + name;
        }

        [ArchivePatch(typeof(CM_SettingScrollReceiver), nameof(CM_SettingScrollReceiver.UpdateText))]
        internal static class SliderTextPatch
        {
            public static void Postfix(CM_SettingScrollReceiver __instance)
            {
                SliderActualValueDisplay.Apply(__instance);
            }
        }

        public sealed class InstalledModsSettings
        {
            [FSDisplayName("刷新列表")]
            public FButton Refresh { get; set; } = new FButton("刷新列表", "refresh", null, true);

            public FLabel Summary { get; set; } = new FLabel("正在读取插件列表…", "installed.summary");

            [FSDisplayName("敌人血条配置")]
            public EnemyHealthBarConfig EnemyHealthBar { get; set; } = new EnemyHealthBarConfig();

            [FSDisplayName("第三人称HUD配置")]
            public ThirdPersonHudConfig ThirdPersonHud { get; set; } = new ThirdPersonHudConfig();

            [FSDisplayName("移动与生命（重启生效）")]
            public PrisonerMovementRestartConfig PrisonerMovement { get; set; } = new PrisonerMovementRestartConfig();

            [FSDisplayName("手电筒大修（重启生效）")]
            public FlashlightRestartConfig Flashlight { get; set; } = new FlashlightRestartConfig();

            [FSDisplayName("生命调整（重启生效）")]
            public PrisonerHealthRestartConfig PrisonerHealth { get; set; } = new PrisonerHealthRestartConfig();

            [FSDisplayName("生物扫描（重启生效）")]
            public BioScannerRestartConfig BioScanner { get; set; } = new BioScannerRestartConfig();

            [FSDisplayName("协同机器人（重启生效）")]
            public CoopBotsRestartConfig CoopBots { get; set; } = new CoopBotsRestartConfig();

            [FSDisplayName("协同队长（只读）")]
            public CaptainReadOnlyConfig Captain { get; set; } = new CaptainReadOnlyConfig();

            [FSDisplayName("协同队长·bot/强化剂（重启生效）")]
            public CaptainBotRestartConfig CaptainBot { get; set; } = new CaptainBotRestartConfig();

            [FSDisplayName("协同队长·中文显示（重启生效）")]
            public CaptainChineseConfig CaptainChinese { get; set; } = new CaptainChineseConfig();

            [FSDisplayName("头戴摄像头")]
            public HeadCameraConfig HeadCamera { get; set; } = new HeadCameraConfig();

            [FSDisplayName("微操大师")]
            public MicroMasterConfig MicroMaster { get; set; } = new MicroMasterConfig();

            [FSDisplayName("第三人称镜头")]
            public ThirdPersonConfig ThirdPerson { get; set; } = new ThirdPersonConfig();

            [FSDisplayName("泡沫枪调整")]
            public FoamGunConfig FoamGun { get; set; } = new FoamGunConfig();

            [FSDisplayName("伤害数字")]
            public DamageNumbersConfig DamageNumbers { get; set; } = new DamageNumbersConfig();

            public FLabel Target01 { get; set; } = new FLabel("", "installed.target01");
            public FLabel Target02 { get; set; } = new FLabel("", "installed.target02");
            public FLabel Target03 { get; set; } = new FLabel("", "installed.target03");
            public FLabel Target04 { get; set; } = new FLabel("", "installed.target04");
            public FLabel Target05 { get; set; } = new FLabel("", "installed.target05");
            public FLabel Target06 { get; set; } = new FLabel("", "installed.target06");
            public FLabel Target07 { get; set; } = new FLabel("", "installed.target07");
            public FLabel Target08 { get; set; } = new FLabel("", "installed.target08");
            public FLabel Target09 { get; set; } = new FLabel("", "installed.target09");
            public FLabel Target10 { get; set; } = new FLabel("", "installed.target10");
            public FLabel Target11 { get; set; } = new FLabel("", "installed.target11");
            public FLabel Target12 { get; set; } = new FLabel("", "installed.target12");
            public FLabel Target13 { get; set; } = new FLabel("", "installed.target13");
            public FLabel Target14 { get; set; } = new FLabel("", "installed.target14");
            public FLabel Target15 { get; set; } = new FLabel("", "installed.target15");
            public FLabel Target16 { get; set; } = new FLabel("", "installed.target16");
            public FLabel Target17 { get; set; } = new FLabel("", "installed.target17");
            public FLabel Target18 { get; set; } = new FLabel("", "installed.target18");
            public FLabel Target19 { get; set; } = new FLabel("", "installed.target19");
            public FLabel Target20 { get; set; } = new FLabel("", "installed.target20");
            public FLabel Target21 { get; set; } = new FLabel("", "installed.target21");
            public FLabel Target22 { get; set; } = new FLabel("", "installed.target22");
            public FLabel Target23 { get; set; } = new FLabel("", "installed.target23");
            public FLabel Target24 { get; set; } = new FLabel("", "installed.target24");
            public FLabel Hint { get; set; } = new FLabel("C1 只显示 BepInEx 已加载插件。", "installed.hint");
            public FLabel LastError { get; set; } = new FLabel("最近错误：无", "installed.error");

            internal FLabel TargetAt(int index)
                => index switch
                {
                    0 => Target01,
                    1 => Target02,
                    2 => Target03,
                    3 => Target04,
                    4 => Target05,
                    5 => Target06,
                    6 => Target07,
                    7 => Target08,
                    8 => Target09,
                    9 => Target10,
                    10 => Target11,
                    11 => Target12,
                    12 => Target13,
                    13 => Target14,
                    14 => Target15,
                    15 => Target16,
                    16 => Target17,
                    17 => Target18,
                    18 => Target19,
                    19 => Target20,
                    20 => Target21,
                    21 => Target22,
                    22 => Target23,
                    23 => Target24,
                    _ => Target01
                };
        }
    }
}
