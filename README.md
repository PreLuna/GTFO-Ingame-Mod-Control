# GTFO-Ingame-Mod-Control

PreLuna 游戏内统一 MOD 管理入口。通过 TheArchive 的扩展接口呈现页面，调用已接入 MOD 的配置能力，保留各 MOD 的独立安装与运行方式。

**当前阶段：2026-11-08 C4 已收尾；C5 已接入 `ThirdPersonHUD` 可写目标与 `PrisonerMovementCustom` 冻结只读目标，并加入通用文件修订保护与测试；HUD 页面显示与真实 CFG 写入/恢复探针已通过；UI 点击与只读页已由用户实机确认正常；C6 的滑块实际值显示子项已安装并经用户实机确认正常；阶段 A 的统一目标页面骨架与 C6 基础（中文状态、诊断分辨率/输入/性能）已实现并经用户实机确认（0.2.1）；阶段 B 已新增七个冻结只读页面；阶段 C 已把 ThirdPerson 5 字段、HeadCamera 6 字段、MicroMaster 6 字段转为可写，ModControl 0.3.3 已构建、安装，加载/真实写入探针通过，已由用户实机确认正常。当前生产路径只有即时自动保存：`SetImmediate → Validate/SetPersistent → 安全行级写盘`。C4 验证了目标 P 持久化、重启读回、注释/未知键/未知段保留、写入失败回滚，以及重启后页面控件按持久化值初始化且连续拖动正常。** 已删除 Draft、ApplyTemporary、SaveAsDefault、RestoreSaved、CommitDeferred、Deferred/Restart 策略及对应旧测试；C2 草稿模型只保留为历史设计记录。已知：恢复默认已删除；滑块归一化显示已在 C6 子项改为实际值；共享文件保护已在本轮以通用修订守卫和 HUD 目标端行级存储落地，Captain 多模块共享配置尚未接入。没有提交、远端或 Release。

[定位入口](#modules) · [页面与操作](#usage) · [依赖与兼容](#relations) · [开发与验收](#development) · [来源说明](#sources)

<a id="modules"></a>
## 定位入口

| 对象 | 入口 | 当前状态 |
| --- | --- | --- |
| 机器登记 | [repo.json](repo.json) | `ModControl` 活动模块＋`ModControl.Resources` 数据模块；版本 0.3.3（阶段 B） |
| 运行源码 | [ModControl/src](ModControl/src) | BepInEx 入口＋手动 ArchiveModule 注册＋三个只读 Feature 页面（静态 `[FeatureConfig]` 设置属性） |
| 操作模型源码 | [ModControl/src/Core/Operations](ModControl/src/Core/Operations) | 即时保存、P／R、校验、能力/权限、修订检查与协调器；无草稿/临时应用/恢复路径 |
| 离线测试 | [ModControl/tests](ModControl/tests) | 9 组 42 项纯逻辑/资源/存储/适配器测试；不加载游戏、不写目标配置 |
| 模块分组定义 | [Definitions](ModControl/Resources/Definitions/Module_PreLuna.ModControl_Group_Definition.json) | 随 DLL 安装的 TheArchive 原生数据文件 |
| 页面组织、状态模型、配置归属与操作协议 | [设计细节](docs/DESIGN.md) | 设计基线 v1；C1～C4 已落地，C5 目标扩展完成并经用户验收，C6 滑块显示子项已落地 |
| 原型、施工阶段、验收矩阵与回退 | [施工计划](docs/IMPLEMENTATION_PLAN.md) | C1～C4 完成；C5 目标扩展已验收；阶段 A 0.2.1、阶段 B 0.2.2 已验收；阶段 C 14 个可写目标（含 5 个冻结 CFG 重启项）+ 1 只读页 0.3.3（17组64项）已安装，待用户确认 |

模块名 `ModControl`，程序集与插件 GUID `PreLuna.ModControl`，版本 `0.2.1`。构建输出为 `PreLuna.ModControl.dll` 与 `Definitions/Module_PreLuna.ModControl_Group_Definition.json`；游戏目标目录沿用 `BepInEx/plugins/本地定制/PreLuna.ModControl/`。尚无远端、提交或 Release；`out` 仅为可重建临时输出。

<a id="usage"></a>
## 页面与操作

沿用 `设置 → 模组设置 → PreLuna` 入口，使用 TheArchive 的原生分组、子菜单、标签和按钮。C1 一级页面为：

- **概览**：管理模块版本、TheArchive 版本／初始化状态、已加载 PreLuna 目标数量、Archive 模块注册结果。
- **已接入 MOD**：显示 BepInEx 已加载的 PreLuna 插件 GUID 与版本；列表自动刷新，也可手动刷新。首个已适配目标从列表上方的“敌人血条配置”子菜单进入。
- **状态与诊断**：框架、注册、最近错误与只读说明；刷新按钮重新读取框架和插件状态。

三个 C1 页面的刷新按钮仍在设置行最上方；`已接入 MOD` 列表每 2 秒刷新标签，不触碰目标配置控件。

C1 页面通过静态 `[FeatureConfig]` 设置属性注册，只使用 `FLabel`／`FButton`，不创建或改写目标 MOD 配置。

## C3 敌人血条自动保存

C3 把首个真实目标接入 `已接入 MOD`，不再新增同级功能行：

- 入口：`已接入 MOD → 敌人血条配置`。
- 8 个设置项：总开关、最大距离、条宽、条高、透明度、显示名称、显示数值、状态槽。
- 修改控件后立即更新目标 R 并写入 P（BepInEx 配置）；页面不显示 P／R／T 或草稿，也没有“应用到本次游戏／保存为默认／丢弃草稿”。
- 页面级“恢复全部设置”按钮经实机测试无法稳定触发，已**删除按钮和专用 ResetDefaults 功能**；C3 不包含恢复默认。
- 通用模型保留 `ImmediateAutoSave`／`DeferredSave`／`RestartRequired` 策略字段；敌人血条当前全部是即时自动保存，没有需要批量保存或重启的字段。
- 自动保存会主动改变目标 P；这是 C3 交互的有意结果。持久化完整性、未知键与共享文件保护、冲突检测和重启验证归 C4。

C4 才处理保存后端、共享文件、冲突/失败恢复与完整重启矩阵；C5 扩展更多目标，C6 做界面与交付验收。

## C4 持久化与端到端原型

C4 已完成首个真实目标的持久化闭环：

- 目标侧关闭 BepInEx `ConfigFile.SaveOnConfigSet`，不再整文件重写；
- 只更新 8 个受管键所在行，保留注释、未知键、未知段和其他行；
- 同目录临时文件＋`File.Replace`，失败保留原文件；保存失败回滚 P／R；
- 页面控件在重启后从目标 P 初始化，不要求先修改一次；连续拖动不再触发子菜单重建；
- 已保存／当前生效两行显示实际值；滑块右侧已在 C6 滑块实际值显示子项改为实际值。

C4 实机结论：CFG 为 300/420/16/1，重启日志读回 300，页面初始化与连续拖动通过。共享文件保护对独立 ConfigFile 不适用，明确留给 C5；恢复默认保持删除。

### C4 清理版

C4 验收后按“代码只保留最终方案”清理：

- 保留 `OperationCoordinator.SetImmediate`、`TargetSnapshot` P／R、`Validate`、`SetPersistent`、`EnemyHudConfigFileStore`；
- 删除 `Draft`、`ApplyToSession`、`SaveAsDefault`、`RestoreSaved`、`CommitDeferred`、`DiscardDraft`、`SettingEditPolicy`、`ApplyTiming`、T／overrides；
- 目标侧删除 `ApplyTemporary`、`SavePersistent`、`RestoreSaved` 与 Deferred/Restart 分支；
- 删除旧草稿/应用/保存/恢复测试；页面文案改为即时自动保存；
- 当前 ModControl 9 组 42 项、EnemyHealthBar 5 组 29 项；两个 DLL 均缩小。清理版已构建/安装，C4 待最小游戏冒烟确认。

### C5 目标扩展（ThirdPersonHUD / PrisonerMovementCustom）

C5 按用户选择先接两个目标；Captain 中文显示后续通过 Captain 目标侧 API 接入，不直接改共享 CFG：

- **`ThirdPersonHUD` 0.13.1（可写，本地视觉）**：目标仓新增 `ThirdPersonHudControlApi`，暴露角色旁面板、个人读数、队伍摘要、场景提示、提示区五个开关及面板缩放/宽度；控制状态使用 `HudConfigFileStore` 按文件修订和受管行写入，保留注释、未知键和其他设置。ModControl 通过 `ThirdPersonHudAdapterFactory` 按类型名隔离解析，使用目标提供的 `GetSnapshotJson/ValidateJson/SetPersistentJson` JSON 桥，不静态引用 HUD 程序集；页面入口为 `已接入 MOD → 第三人称HUD配置`。
- **`PrisonerMovementCustom`（冻结只读）**：不修改冻结 DLL，只读取 `PreLuna.PrisonerMovementCustom.cfg` 的 `[APEX: Prisoners]` 三个开关，展示为只读页 `已接入 MOD → 移动与生命（只读）`；descriptor 标记 `ReadOnly` 且 `PermissionGranted=false`，不提供写入。
- **通用共享文件保护**：`ConfigFileRevisionStore` 提供文件指纹、期望修订、冲突拒绝、行级更新、注释/未知键保留和原子替换，离线覆盖双写冲突、外部修改、缺失文件创建；Captain 中文显示由目标侧 API 接入，仍不直接改共享文件。
- **权限/联机边界**：HUD/伤害为本地视觉；冻结模块只写本地 CFG、重启生效；Captain bot/强化剂为重启CFG页；默认策略只读；没有新增网络同步或客机写权限。
- **时限**：本轮 HUD 五个开关均为运行中即时生效，没有需要延迟到重启/下次应用的字段；通用安全时机队列未产生运行路径，待后续目标暴露延迟字段时再接。

目标、适配器、存储测试已离线通过；HUD 页面已在实机打开，显示 `0.13.1 / AVAILABLE`、五个开关和实际 `PANELWIDTH=304`（非静态默认），说明反射适配器、JSON 快照和面板初始化链路已通。开关写入、重开和 `移动与生命（只读）` 页已由用户 2026-11-08 实机确认正常。

### 阶段 A：目标接入骨架与 C6 基础

- **统一页面骨架**：`InstalledModsPage` 不再为每个目标写一套打开/关闭/刷新/设置变化分支，改为 `TargetPageBinding + ITargetPage`；可写目标复用 `WritableTargetPage<TSettings>`，只读目标实现 `ITargetPage`。TheArchive 仍要求 `InstalledModsSettings` 中每个目标有一个静态 `[FeatureConfig]` 嵌套属性和 `[FSDisplayName]`，这是框架限制，不能改成完全动态目标发现。
- **配置契约**：可写配置实现 `IWritableTargetSettings`，`ApplySnapshot` 只同步页面控件；适配器/协调器仍留在页面对象，`InstalledModsPage` 只负责发现、生命周期和路由。敌人血条和第三人称 HUD 已迁移到该骨架，旧 `EnemyHealthBarPanel`/`ThirdPersonHudPanel` 硬编码文件已删除。
- **C6 文本/诊断**：目标状态、操作结果、布尔/数值显示统一走 `ModControlText`，不再直接显示 `Available` 或英文枚举；诊断页新增显示分辨率、菜单输入状态和更新/刷新计数，用于焦点/输入/性能验收。
- **接入清单**：见 [TARGET_INTEGRATION.md](docs/TARGET_INTEGRATION.md)，后续目标按该清单接入。
- **测试**：新增 `TargetPageTextTests` 4 项；阶段 A 时 ModControl 10 组 46 项通过（HUD 目标侧 33 组不变）。版本 0.2.1 已经用户实机确认。

### 阶段 B：冻结只读批次

阶段 B 原有 7 个只读 CFG 页面中 FoamGun/DamageNumbers 已转可写，剩余 5 个；加上 PrisonerMovement 与 Captain 只读页共 6 个，复用 `ReadOnlyCfgPage<TSettings>` 与 `ReadOnlyCfgReader`；只解析已选键，不写冻结 DLL/CFG，`Capabilities=ReadOnly`、`PermissionGranted=false`：
  - `手电筒大修（只读）`：10 行；`生命调整（只读）`：7 行；`生物扫描（只读）`：5 行；`协同机器人（只读）`：10 行。
  - `协同队长（只读）`：8 行，机器人强化/任务传送/资源策略/驻守/指定击杀/中文菜单/收集距离。
  - `泡沫枪调整`：已转可写。
  - `伤害数字`：已转可写。
- 仅显示 CFG 当前值；值缺失显示“未知”，文件缺失显示“不可用”。`WeaponAttachments`、`MaxConsumables` 在游戏配置目录没有独立运行 CFG，本轮未强行造页，后续按其实际数据入口再评估。
- 测试新增 `ReadOnlyCfgReaderTests` 5 项（含中文键与跨段同名键）；ModControl 现 17 组 64 项通过。

### 阶段 C：本地控制少量可写

- `ThirdPerson` 0.6.3：5 个可写字段（镜头距离/右肩偏移/支点高度偏移/肩射距离/左肩），安全行级写入。
- `HeadCamera` 0.3.16：6 个可写字段（启用/渲染宽度/刷新帧率/视野/最大绘制距离/小窗宽度）；仅影响装 MOD 的本机玩家。
- `MicroMaster` 0.5.18：6 个可写字段（启用/双击间隔/步行速度档位/鼠标灵敏度/反转纵轴/原生手电常亮）；bot 相关效果仅装 MOD 的主机玩家实际生效，非主机只改本地配置。页面增加“主机提示”，说明客机修改只保存本地 CFG，不影响主机或其他玩家。
- `HeadCamera` 页面增加“本机提示”，说明只影响安装 MOD 的本机玩家；编辑仍允许。
- `泡沫枪调整` 1.0.2：启用、发射间隔、力度、散布、每发资源；启用需重启，其他即时。
- `伤害数字` 0.1.1：启用、最大行数、生命周期、字号、最大距离、显示护甲；本地视觉设置。
- Captain 旧只读参考页保留默认策略等未列入字段；bot/强化剂已移到独立重启页。
- 新增独立页面 `协同队长·中文显示（重启生效）`：通讯菜单、携带物品可写；只写本地 CFG，重启游戏后生效。
- 新增独立页面 `协同队长·bot/强化剂（重启生效）`：强化剂/机器人任务/任务传送/驻守半径/指定击杀/收集距离可写；默认策略仍只读。
- 目标侧写入期间关闭 `Config.SaveOnConfigSet`，安全行级替换后恢复；热键内部保存会被状态吸收，外部改动返回冲突。
- 冻结模块 Flashlight/PrisonerMovement/PrisonerHealth/BioScanner/CoopBots 增加“CFG 写入 + 重启生效”页；只写本地 CFG，不即时、不新增 API。
- 只读参考页仍保留 Captain 默认策略/旧只读页。
- 下方插件清单给已配置插件加 `[已适配]`，避免与上方配置入口混淆。版本 0.3.3，已安装，待用户确认。

### C6 滑块实际值显示（子项）

TheArchive 的 `FSSlider` 把 `(实际值 - Min) / (Max - Min)` 归一化后交给 `CellMenu.CM_SettingScrollReceiver`，红字显示的是这个 0～1 比例（百分比或 0/1/2 位小数）。TheArchive 自带的“显示实际值”补丁在 BepInEx 构建中被 `#if IL2CPP && !BepInEx` 排除，不能靠改 `[FSSlider]` 样式解决。

当前实现（仅 ModControl，不改 TheArchive／目标 DLL）：

- `InstalledModsPage` 内嵌 `[ArchivePatch]`，对 `CellMenu.CM_SettingScrollReceiver.UpdateText` 做 postfix；
- 子菜单打开/重建后把 `EnemyHealthBarConfig` 的四个 `NumberSetting` 与其 `CM_SettingScrollReceiver` 登记；
- postfix 只处理已登记接收器，按 `FSSlider.SliderStyle` 把实际值格式化为整数/一位小数/百分比并覆盖 `m_progress.text`，不影响原版和其他 MOD 的滑块；
- 显示名增加单位：`最大距离（米）`、`条宽（px）`、`条高（px）`；透明度保持 `59%`。

探针结果：TheArchive 日志确认 `Patching InstalledModsPage : CellMenu.CM_SettingScrollReceiver.UpdateText()`，登记 `4/4` 并命中四个滑块（归一化文本 `1/1/0.5/52%`），未因补丁崩溃。正式覆盖已安装；用户 2026-11-08 实机确认四个滑块显示实际值、拖动和重开子菜单正常。离线新增 `SliderValueFormatterTests` 5 项；不改目标配置格式和安装布局。


<a id="relations"></a>
## 依赖与兼容

| 关系 | 规则 |
| --- | --- |
| 管理模块 → TheArchive.Core | 运行硬前置；本机固定引用程序集版本 `0.0.838.0`，SHA256 见 `repo.json` |
| 管理模块 → GTFO／BepInEx IL2CPP | BepInEx 插件加载；`[BepInDependency]` 保证 TheArchive 先加载 |
| 管理模块 → 各功能 MOD | 显式适配、版本核验、可选接入；HUD 通过软依赖 + 反射/JSON 隔离，不静态引用目标程序集 |
| 各功能 MOD → 管理模块 | 不新增反向硬依赖；移除管理模块后保留原配置和原操作方式 |
| 各功能 MOD → 原有前置 | 沿用各自 README／repo.json，不因统一管理给全部 MOD 新增 TheArchive 依赖 |

本机研究基线是 Hikaria 分支 TheArchive.Core，程序集版本 `0.0.838.0`。注册接线采用 **BepInEx 插件入口 + 不带 `[ArchiveModule]` 的 `IArchiveModule` 手动注册**：TheArchive 的 Chainloader 会递归扫描 `BepInEx/plugins` 并实例化带 `[ArchiveModule]` 的类型，本程序集已由 BepInEx 加载，若再加该特性会产生第二个模块实例。`Plugin.Load()` 通过 `ArchiveMod.RegisterArchiveModule` 注册 `ModControlModule`；`Build.ps1` 对成品执行“无 `[ArchiveModule]`”守卫。模块分组显示名由随 DLL 安装的 `Definitions/Module_PreLuna.ModControl_Group_Definition.json` 提供。

框架升级先核验接口与行为，必要时仅适配本仓；不兼容时停止管理能力并输出诊断，独立功能 MOD 的正常运行需实测验收。C1 只读页面对框架版本不匹配只标记“未验证”，不开放任何写操作。

编译引用不等于玩家硬前置。可选目标适配必须隔离类型解析和初始化，不能把所有目标程序集静态加载后才判断是否安装。初期不新增要求全部 MOD 引用的公共管理契约 DLL。

每项配置的权威来源仍属于原 MOD。TheArchive 仅承担展示；C1 不建立目标配置镜像，也不持久化草稿。原 MOD 必要的运行状态刷新与持久化接口放回原仓，本仓负责页面和适配。

<a id="development"></a>
## 开发与验收

共用工具为旁边的 [GTFO-Mod-Build-and-Verification-Tools](../GTFO-Mod-Build-and-Verification-Tools/README.md)。开发底座沿用 PowerShell 7、支持 C# 12 的编译环境、.NET 6 与游戏本机 BepInEx／interop。

```powershell
./Build.ps1 -GameRoot 'D:/Program/Game/Steam/Steam/steamapps/common/GTFO' -RuntimeRoot 'C:/Program Files/dotnet/shared/Microsoft.NETCore.App/6.0.16'
./Test.ps1 -RuntimeRoot 'C:/Program Files/dotnet/shared/Microsoft.NETCore.App/6.0.16'
```

`Build.ps1` 调用共享工具，核验 `maintenance.lock.json`、TheArchive 固定 SHA256、`[BepInPlugin]`／`[BepInDependency]` 身份，并额外检查成品不含 `[ArchiveModule]`、不存在会被 TheArchive 实例化的抽象 Feature 基类，且每个页面都有静态 `[FeatureConfig]` 设置属性（缺少该特性时 TheArchive 不会生成“设置 → 进入”子菜单）。`Test.ps1` 编译并运行 9 组 42 项：`TargetCatalogTests` 4、`StatusModelTests` 3、`SliderValueFormatterTests` 5、`ResourcesJsonTests` 1、`ValidationTests` 4、`OperationCoordinatorTests` 16、`ConfigFileRevisionStoreTests` 4、`ThirdPersonHudAdapterTests` 3、`PrisonerMovementReadOnlyAdapterTests` 2。覆盖合法/非法值、越界、未知键、provider 校验拒绝、目标缺失/销毁、读取异常、能力不足、权限拒绝、写入冲突、重入、保存失败保留 P／R、异常和连续即时更新；JSON 适配器另用同程序集假目标验证快照/校验/保存映射。失败注入与 fake provider 只在测试中使用，不写目标配置。HUD 目标仓另有 33 组测试（新增 `HudConfigFileStoreTests` 4、`HudControlStateTests` 5），包含安全行级存储、修订冲突和保存失败回滚。文件存在不表示测试已执行；离线通过不表示游戏实测。

本仓遵循“审查并暂存输入 → 显式更新锁 → 暂存锁 → 核验”的维护流程。安装本机钩子后，维护检查可从本仓运行：

```powershell
../GTFO-Mod-Build-and-Verification-Tools/tools/Check-Commit.ps1 -RepositoryRoot .
```

只验证文档、路径、输入锁及 Git 索引，不表示构建或游戏实测通过。新克隆依工具仓 `Enable-Hooks.ps1` 流程安装本机钩子。源码改动按实际接入范围做关联回归；C1 的只读打开／关闭、焦点恢复、页面刷新与实机加载需要单独游戏验收。

游戏引用、第三方框架 DLL、个人 CFG 和本地日志不进入源码 Git。`out` 仅为可重建输出；本轮增量交付只包含 `PreLuna.ModControl.dll` 与 `Definitions/Module_PreLuna.ModControl_Group_Definition.json`，按工作区根的[本地交付/当前/替换清单.md](../../本地交付/当前/替换清单.md)复制到游戏目标目录。提交、远端、推送、安装与正式发布按当次授权分别执行。

整合仓的精确源码组合目前未包含本仓；本仓没有提交和远端，不填造 Commit、URL 或发布哈希。后续联仓核验先按实际范围登记 LocalWorkspace，提交／推送后再登记真实提交；历史 PublishedBaseline 不随本阶段改写。

<a id="sources"></a>
## 来源说明

本仓由 PreLuna 维护独立管理模块的设计与后续实现。TheArchive 为外部框架，来源为 [AuriRex/GTFO_TheArchive](https://github.com/AuriRex/GTFO_TheArchive)，本机研究使用 Hikaria 分支安装版本；外部模块接入参考为已安装 BoosterTweaker 的接口形态。本轮只引用本机已安装的 TheArchive.Core 程序集，未复制第三方实现代码或 DLL；研究结论不等于代码再分发授权；后续复用源码时保留作者、许可证和来源。
