using GameDesign4.Unit.Contracts.Command;
using GameDesign4.Unit.Contracts.Model;
using UnityEngine;

namespace GameDesign4.Unit.Runtime
{
    /// <summary>
    /// 单位当前命令上下文。
    /// 负责保存当前命令目标和来源信息。
    /// </summary>
    public sealed class UnitCommandContext
    {
        private UnitCommandType commandType;

        /// <summary>
        /// 当前是否有待执行命令。
        /// </summary>
        public bool HasCommand { get; private set; }

        /// <summary>
        /// 当前命令是否来自手动输入。
        /// </summary>
        public bool IsManualCommand { get; private set; }

        /// <summary>
        /// 当前命令是否为移动命令。
        /// </summary>
        public bool IsMoveCommand => HasCommand && commandType == UnitCommandType.Move;

        /// <summary>
        /// 当前命令是否为攻击命令。
        /// </summary>
        public bool IsAttackCommand => HasCommand && commandType == UnitCommandType.Attack;

        /// <summary>
        /// 当前移动目标点。
        /// </summary>
        public Vector3 Destination { get; private set; }

        /// <summary>
        /// 当前攻击目标单位标识。
        /// </summary>
        public UnitId TargetUnitId { get; private set; }

        #region 命令写入
        /// <summary>
        /// 写入手动移动命令。
        /// </summary>
        public void SetMoveCommand(UnitMoveCommand command)
        {
            HasCommand = true;
            IsManualCommand = true;
            commandType = UnitCommandType.Move;
            Destination = command.Destination;
            TargetUnitId = null;
        }

        /// <summary>
        /// 写入手动攻击命令。
        /// </summary>
        public void SetAttackCommand(UnitAttackCommand command)
        {
            HasCommand = true;
            IsManualCommand = true;
            commandType = UnitCommandType.Attack;
            Destination = default;
            TargetUnitId = command.TargetUnitId;
        }

        /// <summary>
        /// 写入自动攻击目标。
        /// </summary>
        public void SetAutoAttackTarget(UnitId targetUnitId)
        {
            HasCommand = true;
            IsManualCommand = false;
            commandType = UnitCommandType.Attack;
            Destination = default;
            TargetUnitId = targetUnitId;
        }

        /// <summary>
        /// 清空当前命令上下文。
        /// </summary>
        public void Clear()
        {
            HasCommand = false;
            IsManualCommand = false;
            commandType = UnitCommandType.None;
            Destination = default;
            TargetUnitId = null;
        }
        #endregion

        /// <summary>
        /// 单位当前命令类型。
        /// </summary>
        private enum UnitCommandType
        {
            None = 0,
            Move = 1,
            Attack = 2
        }
    }
}
