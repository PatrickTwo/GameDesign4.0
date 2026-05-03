using GameDesign4.Unit.Component;
using GameDesign4.Unit.Contracts.Command;
using GameDesign4.Unit.Contracts.Model;
using GameDesign4.Unit.Presentation;

namespace GameDesign4.Unit.Runtime
{
    /// <summary>
    /// 单位大脑。
    /// 负责统一调度手动命令、自动索敌和攻击编排。
    /// </summary>
    public sealed class UnitBrain
    {
        private readonly UnitMovementExecutor movementExecutor;
        private readonly UnitAttackExecutor attackExecutor;
        private readonly UnitAutoCombatExecutor autoCombatExecutor;
        private readonly UnitVisualController visualController;
        private readonly UnitCommandContext commandContext = new UnitCommandContext();

        private UnitActionState currentState;

        /// <summary>
        /// 构造单位大脑。
        /// </summary>
        public UnitBrain(
            UnitMovementExecutor movementExecutor,
            UnitAttackExecutor attackExecutor,
            UnitAutoCombatExecutor autoCombatExecutor,
            UnitVisualController visualController)
        {
            this.movementExecutor = movementExecutor;
            this.attackExecutor = attackExecutor;
            this.autoCombatExecutor = autoCombatExecutor;
            this.visualController = visualController;

            SetState(UnitActionState.Idle);
        }

        /// <summary>
        /// 当前行为状态。
        /// </summary>
        public UnitActionState CurrentState => currentState;

        #region 命令执行
        /// <summary>
        /// 应用移动命令。
        /// </summary>
        public void ApplyMoveCommand(UnitMoveCommand command)
        {
            commandContext.SetMoveCommand(command);
            movementExecutor.Stop();
            SetState(UnitActionState.Moving);
        }

        /// <summary>
        /// 应用攻击命令。
        /// </summary>
        public void ApplyAttackCommand(UnitAttackCommand command)
        {
            commandContext.SetAttackCommand(command);
            movementExecutor.Stop();
            SetState(UnitActionState.ChasingTarget);
        }

        /// <summary>
        /// 停止当前行为并回到待机。
        /// </summary>
        public void StopCurrentAction()
        {
            commandContext.Clear();
            movementExecutor.Stop();
            SetState(UnitActionState.Idle);
        }
        #endregion

        #region Tick 调度
        /// <summary>
        /// 每帧驱动单位行为。
        /// </summary>
        public void Tick()
        {
            if (commandContext.HasCommand == false)
            {
                TryStartAutoCombat();
            }

            if (commandContext.HasCommand == false)
            {
                movementExecutor.Stop();
                SetState(UnitActionState.Idle);
                return;
            }

            if (commandContext.IsMoveCommand)
            {
                TickMoveCommand();
                return;
            }

            if (commandContext.IsAttackCommand)
            {
                TickAttackCommand();
            }
        }

        /// <summary>
        /// 执行移动命令 Tick。
        /// </summary>
        private void TickMoveCommand()
        {
            bool hasReachedDestination = movementExecutor.TickMove(commandContext.Destination);
            SetState(UnitActionState.Moving);

            if (hasReachedDestination)
            {
                commandContext.Clear();
                SetState(UnitActionState.Idle);
            }
        }

        /// <summary>
        /// 执行攻击命令 Tick。
        /// </summary>
        private void TickAttackCommand()
        {
            bool isAttackFlowActive = attackExecutor.TickAttack(commandContext.TargetUnitId, out UnitActionState nextState);

            if (isAttackFlowActive == false)
            {
                commandContext.Clear();
                SetState(UnitActionState.Idle);
                return;
            }

            SetState(nextState);
        }

        /// <summary>
        /// 尝试在空闲时开启自动索敌。
        /// </summary>
        private void TryStartAutoCombat()
        {
            UnitId autoTargetUnitId;
            bool hasAutoTarget = autoCombatExecutor.TryAcquireAutoAttackTarget(out autoTargetUnitId);
            if (hasAutoTarget == false)
            {
                return;
            }

            commandContext.SetAutoAttackTarget(autoTargetUnitId);
        }
        #endregion

        #region 状态同步
        /// <summary>
        /// 切换当前状态并同步表现层。
        /// </summary>
        private void SetState(UnitActionState nextState)
        {
            currentState = nextState;
            visualController.ApplyActionState(currentState);
        }
        #endregion
    }
}
