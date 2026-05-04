# SceneInteract 模块

## 目录结构

```text
SceneInteract
├─ SceneInteract.asmdef
├─ README.md
├─ Presentation
│  └─ Selection
│     └─ SceneSelectable.cs
└─ Runtime
   ├─ SceneCommandService.cs
   ├─ SceneInteractController.cs
   └─ SceneSelectionService.cs
```

## 文件职责

| 文件 | 职责 |
|---|---|
| `Presentation/Selection/SceneSelectable.cs` | 挂在可选中场景对象上的薄组件，负责暴露单位标识并桥接选中表现。 |
| `Runtime/SceneSelectionService.cs` | 维护当前单选目标并切换选中状态。 |
| `Runtime/SceneCommandService.cs` | 将左键、右键、取消输入翻译为选择、移动和攻击命令。 |
| `Runtime/SceneInteractController.cs` | 作为输入消费者接收 `Input` 模块转发的场景交互语义，再串起共享指针上下文与场景命令链路。 |

## 当前交互链路

```text
InputController
-> SceneInteractController
-> PointerContextService
-> SceneCommandService
-> SceneSelectionService / ICommandBus
-> Unit
```

## 共享依赖

| 文件 | 职责 |
|---|---|
| `Common/Utilities/Input/PointerContext.cs` | 一次共享指针检测结果数据。 |
| `Common/Utilities/Input/PointerContextService.cs` | 统一的共享指针射线检测服务与调试输出。 |

## 当前阶段能力

| 能力 | 说明 |
|---|---|
| 左键单选 | 左键命中 `SceneSelectable` 时切换当前选中对象。 |
| 左键空地取消选择 | 左键未命中可选中对象时清空当前选择。 |
| 右键移动 | 右键地面时，对当前选中单位派发 `UnitMoveCommand`。 |
| 右键攻击 | 右键其他可选中单位时，对当前选中单位派发 `UnitAttackCommand`。 |
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
