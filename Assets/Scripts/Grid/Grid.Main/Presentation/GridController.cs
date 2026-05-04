using GameDesign4.Grid.Contracts.Model;
using GameDesign4.Grid.Contracts.Service;

namespace GameDesign4.Grid.Presentation
{
    /// <summary>
    /// 网格显示控制器。
    /// 负责承接外部控制语义，并将状态同步到最底层的 GridView。
    /// </summary>
    public sealed class GridController : IGridControlService
    {
        private readonly GridState gridState;

        /// <summary>
        /// 构造网格显示控制器。
        /// </summary>
        public GridController(GridState gridState)
        {
            this.gridState = gridState;
        }

        #region IGridControlService

        public void ShowGrid()
        {
            gridState.SetGridVisible(true);
        }

        public void HideGrid()
        {
            gridState.SetGridVisible(false);
            gridState.SetHoverCoord(null);
            gridState.ClearPreviewFootprint();
        }

        public void SetHoverCoord(GridCoord? coord)
        {
            gridState.SetHoverCoord(coord);
        }

        public void SetPreviewFootprint(GridFootprint footprint, bool isValid)
        {
            gridState.SetPreviewFootprint(footprint, isValid);
        }

        public void ClearPreviewFootprint()
        {
            gridState.ClearPreviewFootprint();
        }

        public void AddOccupiedFootprint(GridFootprint footprint)
        {
            gridState.AddFootprint(footprint);
        }

        public void RemoveOccupiedFootprint(GridFootprint footprint)
        {
            gridState.RemoveFootprint(footprint);
        }

        #endregion
    }
}
