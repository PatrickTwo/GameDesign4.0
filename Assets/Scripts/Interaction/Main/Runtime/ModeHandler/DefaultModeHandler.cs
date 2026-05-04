using GameDesign4.Command.Contracts;
using GameDesign4.Infrastructure.Runtime;
using GameDesign4.Infrastructure.Runtime.Pointer;
using GameDesign4.Interaction.Contracts;
using GameDesign4.Unit.Contracts.Command;
using GameDesign4.Unit.Contracts.Model;

namespace GameDesign4.Interaction.Runtime
{
    /// <summary>
    /// 默认模式控制器。
    /// 负责处理正常选择、移动与攻击逻辑。
    /// </summary>
    public sealed class DefaultModeHandler : IInteractionModeHandler
    {
        private readonly ICommandBus commandBus;
        private SceneSelectable currentSelection;

        /// <summary>
        /// 当前处理器负责的交互模式。
        /// </summary>
        public InteractionModeType ModeType => InteractionModeType.Default;

        /// <summary>
        /// 当前是否存在有效选中对象。
        /// </summary>
        private bool HasSelection => currentSelection != null;

        /// <summary>
        /// 构造默认模式控制器。
        /// </summary>
        public DefaultModeHandler(ICommandBus commandBus)
        {
            this.commandBus = commandBus;
        }

        #region 接口实现
        /// <summary>
        /// 处理默认模式下的主操作输入。
        /// </summary>
        public InputHandleResult HandlePrimaryAction(PointerContext pointerContext, SceneSelectable hitSelectable)
        {
            // 鼠标位于 UI 上时，不响应默认模式的场景选择。
            if (pointerContext.IsOverUI)
            {
                return InputHandleResult.Continue;
            }

            // 点击到可选中对象时，切换当前选中目标。
            if (hitSelectable != null)
            {
                Select(hitSelectable);
                return InputHandleResult.Continue;
            }

            // 点击空地时清空当前选择。
            ClearSelection();
            return InputHandleResult.Continue;
        }

        /// <summary>
        /// 处理默认模式下的次操作输入。
        /// </summary>
        public InputHandleResult HandleSecondaryAction(PointerContext pointerContext, SceneSelectable hitSelectable)
        {
            // 鼠标位于 UI 上或当前没有选中对象时，不下发任何命令。
            if (pointerContext.IsOverUI || HasSelection == false)
            {
                return InputHandleResult.Continue;
            }

            SceneSelectable selectedUnit = currentSelection;
            if (selectedUnit == null || selectedUnit.TryGetUnitId(out UnitId sourceUnitId) == false)
            {
                return InputHandleResult.Continue;
            }

            // 命中其他单位时，优先尝试下发攻击命令。
            if (hitSelectable != null && hitSelectable != selectedUnit)
            {
                if (hitSelectable.TryGetUnitId(out UnitId targetUnitId))
                {
                    commandBus.Publish(new UnitAttackCommand(sourceUnitId, targetUnitId));
                }

                return InputHandleResult.Continue;
            }

            // 命中地面时，下发移动命令。
            if (pointerContext.HasGroundHit)
            {
                commandBus.Publish(new UnitMoveCommand(sourceUnitId, pointerContext.GroundHitPoint));
            }

            return InputHandleResult.Continue;
        }

        /// <summary>
        /// 处理默认模式下的取消输入。
        /// </summary>
        public InputHandleResult HandleCancelAction()
        {
            // 默认模式下取消输入只负责清空当前选择。
            ClearSelection();
            return InputHandleResult.Continue;
        }
        #endregion

        #region 选择状态管理
        /// <summary>
        /// 选中指定对象。
        /// </summary>
        private void Select(SceneSelectable selectable)
        {
            if (selectable == currentSelection)
            {
                return;
            }

            ClearSelection();
            currentSelection = selectable;
            if (currentSelection != null)
            {
                currentSelection.SetSelected(true);
            }
        }

        /// <summary>
        /// 清空当前选中对象。
        /// </summary>
        private void ClearSelection()
        {
            if (currentSelection == null)
            {
                return;
            }

            currentSelection.SetSelected(false);
            currentSelection = null;
        }
        #endregion
    }
}
