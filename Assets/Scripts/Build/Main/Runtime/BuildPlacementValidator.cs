using GameDesign4.Build.Definition;
using GameDesign4.Grid.Contracts.Model;
using GameDesign4.Grid.Contracts.Service;
using GameDesign4.Infrastructure.Utilities;
using UnityEngine;

namespace GameDesign4.Build.Runtime
{
    /// <summary>
    /// 建造放置合法性判定器。
    /// 当前阶段负责处理网格越界与占用冲突校验。
    /// </summary>
    public sealed class BuildPlacementValidator
    {
        private readonly IGridQueryService gridQueryService;

        /// <summary>
        /// 构造建造放置合法性判定器。
        /// </summary>
        public BuildPlacementValidator(IGridQueryService gridQueryService)
        {
            this.gridQueryService = gridQueryService;
        }

        #region 合法性判定
        /// <summary>
        /// 判定指定蓝图是否可以放置到目标坐标。
        /// </summary>
        public bool CanPlace(BuildingBlueprintDef blueprint, GridFootprint footprint)
        {
            // 缺少蓝图或建筑定义时，直接视为非法放置。
            Guard.EnsureNotNull("非法参数", blueprint, blueprint.Building);

            // 先判定占地是否越界，再判定是否与已有占格冲突。
            bool isInBounds = gridQueryService.IsFootprintInBounds(footprint);
            bool isOccupied = gridQueryService.IsFootprintOccupied(footprint);
            return isInBounds && isOccupied == false;
        }
        #endregion
    }
}
