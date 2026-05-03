using GameDesign4.Shared.Contracts.Events;

namespace GameDesign4.Command.Contracts
{
    /// <summary>
    /// 命令消息标记接口。
    /// 用于约束所有通过命令总线传递的强类型命令。
    /// 
    /// 目前仅作语义隔离，并承载命令发布时的类型约束。
    /// </summary>
    public interface ICommand : IGameEvent
    {
    }
}
