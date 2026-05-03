using GameDesign4.Command.Contracts;
using GameDesign4.Unit.Contracts.Model;

namespace GameDesign4.Unit.Contracts.Command
{
    /// <summary>
    /// 单位攻击命令。
    /// 由外部模块下发给指定单位。
    /// </summary>
    public readonly struct UnitAttackCommand : ICommand
    {
        /// <summary>
        /// 构造攻击命令。
        /// </summary>
        public UnitAttackCommand(UnitId unitId, UnitId targetUnitId)
        {
            UnitId = unitId;
            TargetUnitId = targetUnitId;
        }

        /// <summary>
        /// 接收命令的单位标识。
        /// </summary>
        public UnitId UnitId { get; }

        /// <summary>
        /// 攻击目标单位标识。
        /// </summary>
        public UnitId TargetUnitId { get; }
    }
}
