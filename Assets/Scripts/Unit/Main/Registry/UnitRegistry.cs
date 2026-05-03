using System.Collections.Generic;
using GameDesign4.Unit.Component;
using GameDesign4.Unit.Contracts.Model;

namespace GameDesign4.Unit.Registry
{
    /// <summary>
    /// 单位运行时注册表。
    /// 提供 UnitId 到 UnitEntity 的静态查询能力。
    /// </summary>
    public static class UnitRegistry
    {
        private static readonly Dictionary<UnitId, UnitEntity> UnitsById = new Dictionary<UnitId, UnitEntity>();

        #region 注册管理
        /// <summary>
        /// 注册单位。
        /// </summary>
        public static void Register(UnitEntity unitEntity)
        {
            if (unitEntity == null || unitEntity.UnitId == null || unitEntity.UnitId.IsValid == false)
            {
                return;
            }

            UnitsById[unitEntity.UnitId] = unitEntity;
        }

        /// <summary>
        /// 注销单位。
        /// </summary>
        public static void Unregister(UnitId unitId)
        {
            if (unitId == null || unitId.IsValid == false)
            {
                return;
            }

            UnitsById.Remove(unitId);
        }
        #endregion

        #region 查询
        /// <summary>
        /// 查询指定单位。
        /// </summary>
        public static bool TryGetUnit(UnitId unitId, out UnitEntity unitEntity)
        {
            if (unitId == null)
            {
                unitEntity = null;
                return false;
            }

            return UnitsById.TryGetValue(unitId, out unitEntity);
        }
        #endregion
    }
}
