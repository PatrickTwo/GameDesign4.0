using System;
using GameDesign4.Command.Contracts;
using GameDesign4.Shared.Contracts.Events;
using GameDesign4.Shared.Runtime.Logging;
using GameDesign4.Shared.Utilities;

namespace GameDesign4.Command.Runtime
{
    /// <summary>
    /// 强类型命令总线。
    /// 负责发布和订阅命令消息，不承载具体业务语义。
    /// </summary>
    public sealed class CommandBus : ICommandBus
    {
        private readonly IEventSystem eventDispatcher;

        #region 构造
        /// <summary>
        /// 构造命令总线。
        /// </summary>
        public CommandBus(IEventSystem eventDispatcher)
        {
            Guard.EnsureNotNull(eventDispatcher, nameof(eventDispatcher));
            this.eventDispatcher = eventDispatcher;
        }
        #endregion

        #region 命令发布
        /// <summary>
        /// 发布命令。
        /// </summary>
        public void Publish<TCommand>(TCommand command) where TCommand : struct, ICommand
        {
            GameLog.Log(GameLogModule.Command, "派发命令：" + typeof(TCommand).Name);
            eventDispatcher.Publish(command);
        }
        #endregion

        #region 命令订阅
        /// <summary>
        /// 订阅命令。
        /// </summary>
        public IDisposable Subscribe<TCommand>(Action<TCommand> handler) where TCommand : struct, ICommand
        {
            Guard.EnsureNotNull(handler, nameof(handler));
            return eventDispatcher.Subscribe(handler);
        }
        #endregion
    }
}
