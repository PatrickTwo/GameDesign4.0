using GameDesign4.Command.Contracts;
using GameDesign4.Command.Runtime;
using GameDesign4.SceneInteract.Runtime;
using GameDesign4.Shared.Contracts.Events;
using GameDesign4.UI.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
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

        protected override void Configure(IContainerBuilder builder)
        {
            // 共享事件分发器：承载跨模块事实通知，不承担命令职责。
            builder.Register<EventSystem>(Lifetime.Singleton)
                .AsSelf()
                .As<IEventSystem>();

            // 命令总线：提供强类型命令发布与订阅能力。
            // XXX: 组合根当前仍需直接注册 Command.Main 的具体实现；若要完全对齐“外部模块只依赖契约程序集”，后续需要补模块 Installer 入口。
            builder.Register<CommandBus>(Lifetime.Singleton)
                .AsSelf()
                .As<ICommandBus>();


            // 指针上下文服务：负责射线查询，生成指针上下文。
            builder.Register<PointerContextService>(Lifetime.Singleton);


            builder.RegisterInstance(uiRoot);

            // UI 服务：根据目录配置加载并缓存全部面板。
            builder.Register<UIService>(Lifetime.Singleton)
                .AsSelf()
                .AsImplementedInterfaces();


            // HACK UIService 默认是懒加载，这里在容器构建后主动解析一次，确保启动阶段完成 UI 缓存初始化。
            // 后续当 UIService 被依赖时，则不需要再显式解析。
            builder.RegisterBuildCallback(
                delegate(IObjectResolver container)
                {
                    container.Resolve<IUiPanelService>();
                });
        }
    }
}
