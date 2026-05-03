using System;
using GameDesign4.Combat.Contracts.Service;
using GameDesign4.Command.Contracts;
using GameDesign4.Unit.Contracts.Command;
using GameDesign4.Unit.Contracts.Identity;
using GameDesign4.Unit.Contracts.Model;
using GameDesign4.Unit.Presentation;
using GameDesign4.Unit.Registry;
using GameDesign4.Unit.Runtime;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace GameDesign4.Unit.Component
{
    /// <summary>
    /// 单位场景入口组件。
    /// 负责聚合单位组件、接收命令并驱动 UnitBrain。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class UnitEntity : MonoBehaviour, IUnitIdentity
    {
        // 组件引用
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject selectionIndicator;

        private readonly UnitId unitId = new(); // 单位唯一标识
        // 命令总线注入
        private ICommandBus commandBus;
        private IDisposable moveCommandSubscription;
        private IDisposable attackCommandSubscription;
        // 战斗规则服务
        private IUnitCombatRuleService combatRuleService;
        private UnitVisualController visualController;
        private UnitBrain unitBrain;

        /// <summary>
        /// 单位唯一标识。
        /// </summary>
        public UnitId UnitId => unitId;

        /// <summary>
        /// 当前世界坐标。
        /// </summary>
        public Vector3 Position => transform.position;

        /// <summary>
        /// 当前战斗规则服务。
        /// </summary>
        public IUnitCombatRuleService CombatRuleService => combatRuleService;

        #region 依赖注入
        /// <summary>
        /// 注入命令总线。
        /// 仅用于接收外部模块派发的命令。
        /// </summary>
        [Inject]
        public void Construct(ICommandBus commandBus, IUnitCombatRuleService combatRuleService)
        {
            this.commandBus = commandBus;
            this.combatRuleService = combatRuleService;
            TrySubscribeCommands();
            // 初始化功能组件
            visualController = new UnitVisualController(transform, animator, selectionIndicator);

            UnitMovementExecutor movementExecutor = new(navMeshAgent);
            UnitAttackExecutor attackExecutor = new(this, movementExecutor, visualController);
            UnitAutoCombatExecutor autoCombatExecutor = new(this);

            unitBrain = new UnitBrain(movementExecutor, attackExecutor, autoCombatExecutor, visualController);
        }
        #endregion

        #region 生命周期

        /// <summary>
        /// 注册单位并尝试订阅命令总线。
        /// </summary>
        private void OnEnable()
        {
            UnitRegistry.Register(this);
            TrySubscribeCommands();
        }

        /// <summary>
        /// 注销单位并释放命令订阅。
        /// </summary>
        private void OnDisable()
        {
            ReleaseCommandSubscriptions();
            UnitRegistry.Unregister(unitId);

            if (unitBrain != null)
            {
                unitBrain.StopCurrentAction();
            }
        }

        /// <summary>
        /// 每帧驱动单位逻辑。
        /// </summary>
        private void Update()
        {
            unitBrain.Tick();
        }
        #endregion

        #region 命令响应
        /// <summary>
        /// 外部直接下发移动命令。
        /// </summary>
        private void ApplyMoveCommand(UnitMoveCommand command)
        {
            if (command.UnitId != unitId)
            {
                // 非当前单位命令，直接返回
                // HACK 后续做实体单位中心，进行命令分发，不再每个单位都处理
                return;
            }

            unitBrain.ApplyMoveCommand(command);
        }

        /// <summary>
        /// 外部直接下发攻击命令。
        /// </summary>
        private void ApplyAttackCommand(UnitAttackCommand command)
        {
            if (command.UnitId != unitId)
            {
                // 非当前单位命令，直接返回
                // HACK 后续做实体单位中心，进行命令分发，不再每个单位都处理
                return;
            }

            unitBrain.ApplyAttackCommand(command);
        }

        #endregion

        #region 命令注册与销毁
        /// <summary>
        /// 在依赖就绪后按需订阅命令总线。
        /// </summary>
        private void TrySubscribeCommands()
        {
            if (commandBus == null)
            {
                return;
            }

            if (moveCommandSubscription == null)
            {
                moveCommandSubscription = commandBus.Subscribe<UnitMoveCommand>(ApplyMoveCommand);
            }

            if (attackCommandSubscription == null)
            {
                attackCommandSubscription = commandBus.Subscribe<UnitAttackCommand>(ApplyAttackCommand);
            }
        }

        /// <summary>
        /// 释放命令订阅。
        /// </summary>
        private void ReleaseCommandSubscriptions()
        {
            if (moveCommandSubscription != null)
            {
                moveCommandSubscription.Dispose();
                moveCommandSubscription = null;
            }

            if (attackCommandSubscription != null)
            {
                attackCommandSubscription.Dispose();
                attackCommandSubscription = null;
            }
        }
        #endregion

    }
}
