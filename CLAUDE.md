# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

Unity C# 项目（GameDesign4.0），命名空间统一为 `GameDesign4`。主场景：`Assets/Scenes/GameScene.unity`。

## 构建与测试

```bash
# 运行所有 EditMode 测试（需要在 Unity Editor 外通过 CLI）
# Unity CLI 路径根据安装位置不同，以下为通用写法
Unity -batchmode -projectPath . -runTests -testPlatform EditMode -testResults results.xml
```

测试文件位于 `Assets/Tests/`，使用 NUnit + UnityEngine.TestRunner。

## 核心依赖

| 包 | 用途 |
|---|---|
| VContainer | 依赖注入（LifetimeScope + IContainerBuilder） |
| UniTask | 异步编程 |
| Input System | 输入处理 |
| URP 14.0 | 渲染管线 |

VContainer 和 UniTask 通过 Git URL 引入，详见 `Packages/manifest.json`。

## 模块架构

项目通过 Assembly Definition（.asmdef）划分为独立模块，每个模块遵循相同的子目录约定：

```
Assets/Scripts/{Module}/
├── {Module}.asmdef          # 程序集定义，声明模块间依赖
├── Definitions/             # ScriptableObject 数据定义
├── Runtime/                 # 核心运行时逻辑（服务、状态、控制器）
├── Presentation/            # 表现层（MonoBehaviour、UI、输入绑定）
└── Editor/                  # 编辑器扩展
```

**模块依赖关系：**

```
Shared (基础层，无外部依赖)
  ↑
  ├── Combat
  ├── SceneInteract (依赖 InputSystem, VContainer)
  ├── ResourceNetwork
  ├── Production
  ├── UI
  └── GameFlow (依赖 SceneInteract, VContainer)
```

**完整模块列表：** Shared, Combat, SceneInteract, GameFlow, ResourceNetwork, Production, UI, BaseManagement, Defense, Deployment, Threat, UnitControl, Command, ReconVision, AICombat

## 关键架构模式

### 数据定义（ScriptableObject）

所有定义继承 `BaseDef`（`Assets/Scripts/Shared/Definitions/Core/BaseDef.cs`），提供统一的 Id / Name / Tags / Description 结构。实体定义包括 `UnitDef`, `BuildingDef`, `FactionDef`, `ResourceDef`, `BlueprintDef`, `SupplyDef`。

### 服务返回值

业务方法统一返回 `OperationResult` 或 `OperationResult<T>`（`Assets/Scripts/Shared/Runtime/OperationResult.cs`），不通过异常表达业务失败。

### 前置条件守卫

使用 `Guard` 工具类（`Assets/Scripts/Shared/Utilities/Guard.cs`）进行参数校验，非法参数直接抛出异常，与 `OperationResult` 的业务失败区分。

### 日志系统

统一使用 `GameLog` + `GameLogModule` 输出日志（`Assets/Scripts/Shared/Runtime/Logging/`），不直接使用 `Debug.Log`。格式：`[ModuleName] message`。

### 场景交互系统

`InteractionModeController` 是交互入口（`Assets/Scripts/SceneInteract/Runtime/Core/`），通过模式处理器（ModeHandler）派发输入命令。模式包括：SceneCommand, BuildingPlacement, UnitDeployment, Targeting, AreaCommand。

### 依赖注入

使用 VContainer 的 `LifetimeScope`（`Assets/Scripts/GameFlow/Presentation/GameLifetimeScope.cs`）配置 DI 容器。

## 编码约定

- 不使用 `var`，指明具体类型
- 生成代码时添加 XML 注释和语句级中文注释
- 使用有意义的 `#region` 标识分隔代码块（不在函数内使用 region）
- 不要生成 .meta 文件
- UI 脚本优先使用 Inspector 绑定，不生成运行时 UI 查找逻辑
