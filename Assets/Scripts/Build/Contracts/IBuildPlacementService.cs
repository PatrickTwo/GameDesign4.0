using System.Collections.Generic;
using GameDesign4.Build.Contracts.ViewData;
using UnityEngine;

namespace GameDesign4.Build.Contracts
{
    /// <summary>
    /// 建造放置服务接口。
    /// 负责向外暴露建造面板数据、建造模式切换、预览更新与放置输入处理能力。
    /// </summary>
    public interface IBuildPlacementService
    {
        /// <summary>
        /// 当前是否处于建造模式。
        /// </summary>
        bool IsPlacementActive { get; }

        #region 面板数据
        /// <summary>
        /// 获取建造面板展示条目。
        /// </summary>
        IReadOnlyList<BuildPanelEntry> GetPanelEntries();

        /// <summary>
        /// 基于蓝图标识开始一次建造放置。
        /// </summary>
        void StartPlacement(string blueprintId);
        #endregion

        #region 输入处理
        /// <summary>
        /// 处理建造模式下的左键输入。
        /// </summary>
        bool HandlePrimaryAction(Vector3 worldPosition, bool hasGroundHit, bool isOverUi);

        /// <summary>
        /// 处理建造模式下的右键输入。
        /// </summary>
        bool HandleSecondaryAction();

        /// <summary>
        /// 处理建造模式下的取消输入。
        /// </summary>
        bool HandleCancelAction();
        #endregion
    }
}
