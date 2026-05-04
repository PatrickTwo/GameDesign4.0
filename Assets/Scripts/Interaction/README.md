# Interaction 模块

## 目录结构

```text
Interaction
├─ Contracts
│  ├─ Interaction.Contracts.asmdef
│  ├─ InteractionModeType.cs
│  └─ IInteractionModeController.cs
├─ Main
│  ├─ Interaction.Main.asmdef
│  ├─ Presentation
│  │  └─ Selection
│  │     └─ SceneSelectable.cs
│  └─ Runtime
│     ├─ InteractionModeController.cs
│     └─ ModeHandler
│        ├─ IInteractionModeHandler.cs
│        ├─ DefaultModeHandler.cs
│        ├─ BuildModeHandler.cs
│        └─ DeploymentModeHandler.cs
├─ README.md
```

## 文件职责

| 文件 | 职责 |
|---|---|
| `Contracts/IInteractionModeController.cs` | 对外暴露交互模式切换与模式入口契约。 |
| `Contracts/InteractionModeType.cs` | 对外暴露交互模式枚举。 |
| `Main/Presentation/Selection/SceneSelectable.cs` | 挂在可选中场景对象上的薄组件，负责暴露单位标识并桥接选中表现。 |
| `Main/Runtime/ModeHandler/IInteractionModeHandler.cs` | 声明单个交互模式处理器的统一输入接口，仅模块内部使用。 |
| `Main/Runtime/InteractionModeController.cs` | 同时承担输入入口与模式状态维护，并按当前处理器分发输入。 |
| `Main/Runtime/ModeHandler/DefaultModeHandler.cs` | 负责默认模式下的选中状态维护、移动与攻击命令翻译。 |
| `Main/Runtime/ModeHandler/BuildModeHandler.cs` | 负责建造模式下的输入转发。 |
| `Main/Runtime/ModeHandler/DeploymentModeHandler.cs` | 负责部署模式下的输入转发。 |

## 当前交互链路

```text
InputController
-> InteractionModeController
-> PointerContextService
-> currentHandler
-> BuildPlacementService / DeploymentService / ICommandBus
-> InputHandleResult
-> InteractionModeController 统一决定是否回到 Default
-> Unit / Build / Deployment
```

## 共享依赖

| 文件 | 职责 |
|---|---|
| `Common/Utilities/Input/PointerContext.cs` | 一次共享指针检测结果数据。 |
| `Common/Utilities/Input/PointerContextService.cs` | 统一的共享指针射线检测服务与调试输出。 |

## 当前阶段能力

| 能力 | 说明 |
|---|---|
| 模式切换 | 当前支持 `Default`、`Build`、`Deployment` 三种交互模式。 |
| 模式入口 | `InteractionModeController` 对外提供 `EnterBuildMode`、`EnterDeploymentMode` 统一进入模式。 |
| 左键单选 | 左键命中 `SceneSelectable` 时切换当前选中对象。 |
| 左键空地取消选择 | 左键未命中可选中对象时清空当前选择。 |
| 右键移动 | 右键地面时，对当前选中单位派发 `UnitMoveCommand`。 |
| 右键攻击 | 右键其他可选中单位时，对当前选中单位派发 `UnitAttackCommand`。 |
| 建造模式输入接管 | 建造模式下由建造系统独占处理放置、右键取消与 ESC 取消，并返回 `InputHandleResult`。 |
| 部署模式输入接管 | 部署模式下由部署系统独占处理落点部署、右键取消与 ESC 取消，并返回 `InputHandleResult`。 |
| UI 点击拦截 | 指针悬停在 UI 上时，不处理场景点击命令。 |

## 默认运行时配置

| 项目 | 要求 |
|---|---|
| 场景相机 | 默认使用 `Camera.main`。 |
| UI 事件系统 | 默认使用 `EventSystem.current`。 |
| 地面层 | 默认使用 `Ground` Layer。 |
| 场景检测层 | 默认使用 `Physics.DefaultRaycastLayers`，并自动排除 `Ground` 层。 |
| `SceneSelectable` | 挂在可选中对象根节点上；子碰撞体点击会通过 `GetComponentInParent` 映射回根节点。 |
| `selectionVisual` | 可选，用于显示选中圈、描边等基础选中表现。 |

## 当前边界

| 不负责内容 | 说明 |
|---|---|
| 单位移动执行 | 由 `Unit` 模块处理。 |
| 攻击追击与伤害结算 | 由 `Combat` 与 `Unit` 模块处理。 |
| 自动索敌 | 由 `Unit` 模块驱动 `Combat` 服务完成。 |
| 阵营判定 | 当前阶段右键命中其他单位即派发攻击命令，不在本模块判断敌我。 |
