using Cinemachine;
using GameDesign4.Build.Contracts;
using GameDesign4.Build.Definition;
using GameDesign4.Build.Runtime;
using GameDesign4.CameraControl.Contracts.Service;
using GameDesign4.CameraControl.Definition;
using GameDesign4.CameraControl.Runtime;
using GameDesign4.Command.Contracts;
using GameDesign4.Command.Runtime;
using GameDesign4.Combat.Contracts.Service;
using GameDesign4.Combat.Runtime;
using GameDesign4.Infrastructure.Runtime.Debug;
using GameDesign4.Infrastructure.Runtime.Logging;
using GameDesign4.Infrastructure.Runtime.Pointer;
using GameDesign4.Infrastructure.Contracts.Events;
using GameDesign4.SceneInteract.Runtime;
using GameDesign4.UI.Definitions;
using GameDesign4.UI.Presentation;
using GameDesign4.UI.Runtime;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using GameDesign4.Grid.Presentation;
using GameDesign4.Grid.Contracts.Service;
using GameDesign4.Grid.Contracts.Model;
using GameDesign4.Grid.Runtime;
using GameDesign4.Grid.Definition;

namespace GameDesign4.GameFlow
{
    /// <summary>
    /// 全局 DI 容器配置。
    /// 注册所有服务、调试输出源，并通过 VContainer 自动注入到场景中的 MonoBehaviour。
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("资源管理")]
        [SerializeField] private UiPanelCatalogDef uiPanelCatalog;
        [SerializeField] private BuildCatalogDef buildCatalog;
        [Header("UI根节点")]
        [SerializeField] private UIRoot uiRoot;
        [Header("相机控制")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private CameraControlSettings cameraControlSettings;
        [Header("网格配置")]
        [SerializeField] private GridDefinition gridDefinition;


        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PointerContextService>(Lifetime.Singleton)
                .AsSelf();

            // 网格系统
            builder.RegisterInstance(new GridState());
            builder.RegisterInstance(gridDefinition);
            builder.Register<GridController>(Lifetime.Singleton).AsSelf().As<IGridControlService>();
            builder.Register<GridQueryService>(Lifetime.Singleton).As<IGridQueryService>();

            // XXX 这里的EventSystem因与Infrastructure.Runtime.Events.EventSystem名称冲突，已经产生过bug，因此这里用显示命名空间，防止再次混淆
            builder.Register<Infrastructure.Runtime.Events.EventSystem>(Lifetime.Singleton)
                .AsSelf()
                .As<IEventSystem>();

            // 命令总线：提供强类型命令发布与订阅能力。
            // XXX: 组合根当前仍需直接注册 Command.Main 的具体实现；若要完全对齐“外部模块只依赖契约程序集”，后续需要补模块 Installer 入口。
            builder.Register<CommandBus>(Lifetime.Singleton)
                .AsSelf()
                .As<ICommandBus>();

            // 战斗规则服务：提供单位自动索敌、攻击范围判断与伤害结算。
            builder.Register<CombatRuleService>(Lifetime.Singleton)
                .AsSelf()
                .As<IUnitCombatRuleService>();


            // 场景交互控制器：由容器驱动输入生命周期并派发场景交互命令。
            builder.RegisterEntryPoint<SceneInteractController>();

            // 相机控制运行依赖：直接注册场景节点与配置，避免无意义的上下文包装。
            builder.RegisterInstance(cameraTarget);
            builder.RegisterInstance(virtualCamera);
            builder.RegisterInstance(cameraControlSettings);

            // RTS 相机控制器：负责平移、缩放、旋转，并对外暴露启停能力。
            builder.RegisterEntryPoint<CameraController>()
                .AsSelf()
                .As<ICameraControlService>();

            // 指针调试输出：为 DebugOverlay 提供实时鼠标命中上下文文本。
            builder.Register<PointerContextDebugOutput>(Lifetime.Singleton)
                .As<IDebugOutput>();


            builder.RegisterInstance(uiRoot);
            UiPanelCatalogDef resolvedUiPanelCatalog = uiPanelCatalog;
            if (resolvedUiPanelCatalog == null)
            {
                // XXX: 当前场景若未绑定 UI 目录资产，UIService 将无法初始化，因此这里回退为空目录并输出中文警告，避免容器装配直接失败。
                resolvedUiPanelCatalog = ScriptableObject.CreateInstance<UiPanelCatalogDef>();
                GameLog.Warning(GameLogModule.UI, "未绑定 UiPanelCatalogDef，UI 面板目录将为空。");
            }

            builder.RegisterInstance(resolvedUiPanelCatalog);

            // 建造运行时上下文：承载当前场景使用的建造目录资产。
            if (buildCatalog == null)
            {
                GameLog.Warning(GameLogModule.Build, "未绑定 BuildCatalogDef，建造面板将为空。");
            }

            builder.RegisterInstance(buildCatalog);

            // 建造放置服务：负责建造模式、预览、放置、取消与每帧预览跟随。
            builder.RegisterEntryPoint<BuildPlacementService>()
                .AsSelf()
                .As<IBuildPlacementService>();

            // UI 服务：根据目录配置加载并缓存全部面板。
            builder.RegisterEntryPoint<UIService>(Lifetime.Singleton);
        }
    }
}
