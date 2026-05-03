using System;

namespace GameDesign4.Command.Contracts
{
    /// <summary>
    /// 命令总线接口。
    /// 提供强类型命令发布与订阅能力。
    /// </summary>
    public interface ICommandBus
    {
        #region 命令发布
        /// <summary>
        /// 发布命令。
        /// </summary>
        void Publish<TCommand>(TCommand command) where TCommand : struct, ICommand;
        #endregion

        #region 命令订阅
        /// <summary>
        /// 订阅命令。
        /// </summary>
        IDisposable Subscribe<TCommand>(Action<TCommand> handler) where TCommand : struct, ICommand;
        #endregion
    }
}
