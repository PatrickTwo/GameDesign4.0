using GameDesign4.Infrastructure.Definitions;
using GameDesign4.Infrastructure.Runtime;
using UnityEngine;

namespace GameDesign4.Deployment.Contracts
{
    /// <summary>
    /// 部署服务接口。
    /// 负责向外暴露部署请求初始化与地图点击部署能力。
    /// </summary>
    public interface IDeploymentService
    {
        #region 部署入口
        /// <summary>
        /// 基于仓库中的可部署实体开始一次部署流程。
        /// </summary>
        void StartDeployment(EntityDef deployableEntity);
        #endregion

        #region 输入处理
        /// <summary>
        /// 处理部署模式下的主操作输入。
        /// </summary>
        InputHandleResult HandlePrimaryAction(Vector3 worldPosition, bool hasGroundHit, bool isOverUi);

        /// <summary>
        /// 处理部署模式下的次操作输入。
        /// </summary>
        InputHandleResult HandleSecondaryAction();

        /// <summary>
        /// 处理部署模式下的取消输入。
        /// </summary>
        InputHandleResult HandleCancelAction();
        #endregion
    }
}
