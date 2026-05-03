# UI 模块说明

## 1. 模块交互

### 对外接口

| 接口 | 说明 |
| --- | --- |
| `IUiPanelService.IsPanelOpen` | 提供面板打开状态查询 |
| `IUiPanelService.OpenPanel` | 提供面板打开入口 |
| `IUiPanelService.ClosePanel` | 提供面板关闭入口 |
| `IUiPanelService.CloseLastOpenedPanel` | 提供按最近打开顺序关闭面板的入口 |
| `IUiPanelService.TogglePanel` | 提供面板显隐切换入口 |

### 外部模块依赖

#### `Build`

| 模块接口 | 描述说明 |
| --- | --- |
| 无 | 建造面板已归属 `Build` 模块，UI 模块不再直接承载建造业务入口 |

#### `UI.Definitions`

| 模块接口 | 描述说明 |
| --- | --- |
| `UiPanelCatalogDef.PanelEntries` | 提供全部 UI 面板目录条目 |
| `UiPanelEntryDef.PanelId` | 读取面板唯一标识 |
| `UiPanelEntryDef.AddressKey` | 读取面板 Addressable 地址键 |
| `UiPanelEntryDef.Layer` | 读取面板挂载层级 |
| `UiPanelEntryDef.OpenOnStartup` | 读取面板是否启动默认打开 |
| `UiPanelEntryDef.ExclusivePanelIds` | 读取面板互斥配置 |

#### `UI.Presentation`

| 模块接口 | 描述说明 |
| --- | --- |
| `UIRoot.GetLayerRoot` | 按层级枚举返回已通过 Inspector 绑定的层级根节点 |
| `BasePanel.PanelId` | 读取运行时面板唯一标识 |
| `BasePanel.Open` | 打开面板实例 |
| `BasePanel.Close` | 关闭面板实例 |
| `BasePanel.IsOpen` | 读取面板当前打开状态 |

#### `VContainer`

| 模块接口 | 描述说明 |
| --- | --- |
| `IObjectResolver.InjectGameObject` | 对运行时实例化的面板对象执行依赖注入 |

#### `Unity Addressables`

| 模块接口 | 描述说明 |
| --- | --- |
| `Addressables.InstantiateAsync` | 按目录配置异步实例化面板预制体 |
| `Addressables.ReleaseInstance` | 释放已缓存的面板实例 |

#### `UnityEngine`

| 模块接口 | 描述说明 |
| --- | --- |
| `Transform.SetAsLastSibling` | 调整面板显示顺序，保证后打开面板显示在前 |

## 2. 模块流程图

```mermaid
flowchart TD
    A["GameFlow 注入 UiPanelCatalogDef / UIRoot"] --> B["UIService"]
    B --> C["Addressables.InstantiateAsync 加载 BasePanel"]
    C --> D["IObjectResolver.InjectGameObject 注入依赖"]
    D --> E["缓存面板并暴露 IUiPanelService"]
    F["外部模块调用 IUiPanelService"] --> E
    E --> G["BasePanel.Open / Close / Toggle"]
```

## 3. 当前实现备注

| 项 | 说明 |
| --- | --- |
| 当前风险 | 装配层对 UI 仍是硬依赖；UI 面板初始化仍是异步 fire-and-forget |
| 当前限制 | 当前层级节点依赖场景中的 `UIRoot` 组件显式绑定 |
| 后续建议 | 先补 `NullUiPanelService` 与条件注册，再补 UI 模块测试，并补一个初始化完成信号 |
