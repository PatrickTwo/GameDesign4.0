using GameDesign4.Command.Contracts;
using GameDesign4.Unit.Contracts.Model;
using UnityEngine;

namespace GameDesign4.Unit.Contracts.Command
{
    /// <summary>
    /// 单位移动命令。
    /// 由外部模块下发给指定单位。
    /// </summary>
    public readonly struct UnitMoveCommand : ICommand
    {
        /// <summary>
        /// 构造移动命令。
        /// </summary>
        public UnitMoveCommand(UnitId unitId, Vector3 destination)
        {
            UnitId = unitId;
            Destination = destination;
        }

        /// <summary>
        /// 接收命令的单位标识。
        /// </summary>
        public UnitId UnitId { get; }

        /// <summary>
        /// 目标移动位置。
        /// </summary>
        public Vector3 Destination { get; }
    }
}
