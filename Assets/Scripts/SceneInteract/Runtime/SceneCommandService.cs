using GameDesign4.Build.Contracts;
using GameDesign4.Command.Contracts;
using GameDesign4.Infrastructure.Runtime.Pointer;
using GameDesign4.Unit.Contracts.Command;
using GameDesign4.Unit.Contracts.Model;

namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 场景命令服务。
    /// 负责将交互输入翻译为选择、移动与攻击命令。
    /// </summary>
    public sealed class SceneCommandService
    {
        private readonly IBuildPlacementService buildPlacementService;
        private readonly ICommandBus commandBus;
        private readonly SceneSelectionService selectionService;

        /// <summary>
        /// 构造场景命令服务。
        /// </summary>
        public SceneCommandService(ICommandBus commandBus, IBuildPlacementService buildPlacementService, SceneSelectionService selectionService)
        {
            this.buildPlacementService = buildPlacementService;
            this.commandBus = commandBus;
            this.selectionService = selectionService;
        }

        #region 输入处理
        /// <summary>
        /// 处理左键点击。
        /// </summary>
        public void HandlePrimaryClick(PointerContext pointerContext, SceneSelectable hitSelectable)
        {
            if (buildPlacementService.HandlePrimaryAction(pointerContext.GroundHitPoint, pointerContext.HasGroundHit, pointerContext.IsOverUI))
            {
                return;
            }

            if (pointerContext.IsOverUI)
            {
                return;
            }

            if (hitSelectable != null)
            {
                selectionService.Select(hitSelectable);
                return;
            }

            selectionService.ClearSelection();
        }

        /// <summary>
        /// 处理右键点击。
        /// </summary>
        public void HandleSecondaryClick(PointerContext pointerContext, SceneSelectable hitSelectable)
        {
            if (buildPlacementService.HandleSecondaryAction())
            {
                return;
            }

            if (pointerContext.IsOverUI || selectionService.HasSelection == false)
            {
                return;
            }

            SceneSelectable selectedUnit = selectionService.CurrentSelection;
            if (selectedUnit == null || selectedUnit.TryGetUnitId(out UnitId sourceUnitId) == false)
            {
                return;
            }

            if (hitSelectable != null && hitSelectable != selectedUnit)
            {
                if (hitSelectable.TryGetUnitId(out UnitId targetUnitId))
                {
                    commandBus.Publish(new UnitAttackCommand(sourceUnitId, targetUnitId));
                }

                return;
            }

            if (pointerContext.HasGroundHit)
            {
                commandBus.Publish(new UnitMoveCommand(sourceUnitId, pointerContext.GroundHitPoint));
            }
        }

        /// <summary>
        /// 处理取消输入。
        /// </summary>
        public void HandleCancel()
        {
            if (buildPlacementService.HandleCancelAction())
            {
                return;
            }

            selectionService.ClearSelection();
        }
        #endregion
    }
}
