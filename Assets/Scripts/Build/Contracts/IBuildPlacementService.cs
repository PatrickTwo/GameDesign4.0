using GameDesign4.Infrastructure.Runtime;
using UnityEngine;

namespace GameDesign4.Build.Contracts
{
    /// <summary>
     /// 建造放置服务接口。
    /// 负责向外暴露建造放置初始化与输入处理能力。
    /// </summary>
    public interface IBuildPlacementService
    {
        /// <summary>
        /// 当前是否处于建造模式。
        /// </summary>
        bool IsPlacementActive { get; }

        #region 建造模式切换
        /// <summary>
        /// 基于蓝图标识开始一次建造放置。
        /// </summary>
        void StartPlacement(string blueprintId);
        #endregion

        #region 输入处理
        /// <summary>
        /// 处理建造模式下的左键输入。
        /// </summary>
        InputHandleResult HandlePrimaryAction(Vector3 worldPosition, bool hasGroundHit, bool isOverUi);

        /// <summary>
        /// 处理建造模式下的右键输入。
        /// </summary>
        InputHandleResult HandleSecondaryAction();

        /// <summary>
        /// 处理建造模式下的取消输入。
        /// </summary>
        InputHandleResult HandleCancelAction();
        #endregion
    }
}
