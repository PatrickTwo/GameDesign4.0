using GameDesign4.Interaction.Contracts;
using GameDesign4.Infrastructure.Runtime.Pointer;
using GameDesign4.Infrastructure.Runtime;

namespace GameDesign4.Interaction.Runtime
{
    /// <summary>
    /// 交互模式处理器接口。
    /// 负责统一声明单个交互模式的输入处理能力。
    /// </summary>
    public interface IInteractionModeHandler
    {
        /// <summary>
        /// 当前处理器负责的交互模式。
        /// </summary>
        InteractionModeType ModeType { get; }

        #region 输入处理
        /// <summary>
        /// 处理当前模式下的主操作输入。
        /// </summary>
        InputHandleResult HandlePrimaryAction(PointerContext pointerContext, SceneSelectable hitSelectable);

        /// <summary>
        /// 处理当前模式下的次操作输入。
        /// </summary>
        InputHandleResult HandleSecondaryAction(PointerContext pointerContext, SceneSelectable hitSelectable);

        /// <summary>
        /// 处理当前模式下的取消输入。
        /// </summary>
        InputHandleResult HandleCancelAction();
        #endregion
    }
}
