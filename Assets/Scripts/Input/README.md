# Input 模块说明

## 1. 模块职责

| 项 | 说明 |
| --- | --- |
| `GameInput` | 统一承载 Input System 输入定义 |
| `InputController` | 持有输入资产并将底层输入转发为业务语义 |
| `IInteractionInputConsumer` | 场景交互输入消费接口 |
| `IUIInputConsumer` | UI 快捷键输入消费接口 |

## 2. 当前输入优先级

```text
Global/Cancel
-> IUIInputConsumer.HandleCancelAction
-> IInteractionInputConsumer.HandleCancelAction
```

## 3. 当前快捷键语义

| ActionMap | Action | 说明 |
| --- | --- | --- |
| `Global` | `Cancel` | 全局取消输入 |
| `SceneInteract` | `PointerPosition` | 提供屏幕坐标 |
| `SceneInteract` | `LeftClick` | 主操作 |
| `SceneInteract` | `RightClick` | 次操作 |
| `UI` | `ToggleBuildPanel` | 切换建造面板 |
| `UI` | `ToggleInventoryPanel` | 切换仓库面板 |
| `UI` | `ToggleProductionPanel` | 切换生产面板 |
