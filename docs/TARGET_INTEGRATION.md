# 新增目标接入清单（阶段 A 骨架）

本仓的“已接入 MOD”页面现在按 `TargetPageBinding + ITargetPage` 组织。TheArchive 仍要求每个目标在
`InstalledModsSettings` 中有一个静态 `[FeatureConfig]` 嵌套属性；除此之外，页面生命周期、打开/关闭刷新、
设置变化路由和只读/可写状态不再写死在 `InstalledModsPage.Update` 中。

## 1. 先定接入边界

- 读目标仓 `README.md`、`repo.json` 和 Git 状态，确认源码/冻结状态。
- 可写目标必须有目标端控制 API、安全保存后端、版本/能力/权限和失败回滚；不能在总控端直接改目标 DLL 内存
  或整文件覆盖。
- 冻结目标只读解析已有 CFG，`PermissionGranted=false`，不提供写入控件。
- 所有设置必须标明应用时机；没有延迟字段就不要伪造队列。HUD 首批均为即时本地视觉项。

## 2. 可写目标（Writable）

1. 目标仓新增控制 API 与安全行级存储，返回目标快照、修订和能力；保留注释/未知键，冲突时拒绝写入。
2. 在 `ModControl/src/Adapters` 新增隔离适配器（类型名 + JSON 桥或显式契约），实现
   `ITargetProvider`：`ReadSnapshot`、`Validate`、`SetPersistent`。
3. 新增配置类，实现 `IWritableTargetSettings`：
   - 继承所需 TheArchive 设置控件；
   - 提供 `FLabel Status/Result/CurrentValues/SavedValues`；
   - 在 `ApplySnapshot(TargetSnapshot)` 中只把目标值写回控件，不写盘。
4. 在 `InstalledModsSettings` 增加一个静态 `[FSDisplayName("中文名")]` 嵌套属性，并在
   `InstalledModsPage.EnsurePages()` 增加 `TargetPageBinding`：
   - 属性名、子菜单标题片段、配置类型、静态回退对象；
   - `WritableTargetPage<T>` + 适配器工厂 + “属性名 → 目标键”字典；
   - 如目标有额外打开/关闭动作（例如敌人血条的滑块登记），放进 binding 的 `opened/closed`。
5. 回归：适配器映射/校验/写入结果测试，更新 `IntegrationTests/recipes.json`；构建后检查
   `repo.json` 外部引用哈希和 `maintenance.lock.json`。

## 3. 冻结目标（Read-only）

1. 只读解析目标 CFG，生成 `TargetDescriptor(Capabilities = ReadOnly, PermissionGranted=false)`。
2. 配置类只放 `FLabel`；实现自定义 `ITargetPage`，在 `Refresh(object)` 中格式化标签，
   `OnSettingChanged` 空实现，`SettingsType` 返回配置类型。
3. 在 `InstalledModsPage.EnsurePages()` 增加只读 binding；不要传 `ITargetProvider`，不要提供
   `[FSSlider]`/`FSToggle` 等可编辑控件。
4. 测试：CFG 缺失/有效/字段缺失/只读权限原因；确认冻结 DLL/CFG 不被写。

## 4. C6 界面约定

- 状态/结果文本统一走 `ModControlText`：目标状态、操作结果和布尔/数值格式化使用中文。
- 不新增全局热键；菜单打开时输入焦点由 TheArchive 持有，管理页只读/写入控件不抢焦点。
- 诊断页新增显示分辨率、菜单输入状态和更新/刷新计数；这些是观测项，不改变渲染或输入行为。
- 页面最多 24 行目标列表；新增目标不得改变现有三项目录/布局顺序，不为对齐源码层级重排游戏目录。
- 构建、离线测试、实机分别记录；交付只列本次实际 DLL/资源，不附带整仓构建产物。
