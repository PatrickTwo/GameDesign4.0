using Cinemachine;
using GameDesign4.CameraControl.Contracts.Service;
using GameDesign4.CameraControl.Definition;
using GameDesign4.CameraControl.Runtime;
using GameDesign4.Command.Contracts;
using GameDesign4.Command.Runtime;
using GameDesign4.Combat.Contracts.Service;
using GameDesign4.Combat.Runtime;
using GameDesign4.Infrastructure.Runtime.Debug;
using GameDesign4.SceneInteract.Runtime;
using GameDesign4.Infrastructure.Contracts.Events;
using GameDesign4.UI.Runtime;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameDesign4.GameFlow
{
    /// <summary>
    /// 全局 DI 容器配置。
    /// 注册所有服务、调试输出源，并通过 VContainer 自动注入到场景中的 MonoBehaviour。
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private Transform uiRoot;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private CameraControlSettings cameraControlSettings = new CameraControlSettings();


        protected override void Configure(IContainerBuilder builder)
        {
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

            // 相机控制上下文：聚合相机目标、虚拟相机与控制参数。
            builder.RegisterInstance(new CameraControlSceneContext(cameraTarget, virtualCamera, cameraControlSettings));

            // RTS 相机控制器：负责平移、缩放、旋转，并对外暴露启停能力。
            builder.RegisterEntryPoint<CameraController>()
                .AsSelf()
                .As<ICameraControlService>();

            // 指针调试输出：为 DebugOverlay 提供实时鼠标命中上下文文本。
            builder.Register<PointerContextDebugOutput>(Lifetime.Singleton)
                .As<IDebugOutput>();


            builder.RegisterInstance(uiRoot);

            // UI 服务：根据目录配置加载并缓存全部面板。
            builder.Register<UIService>(Lifetime.Singleton)
                .AsSelf()
                .AsImplementedInterfaces();
        }
    }
}
