using GameDesign4.Infrastructure.Definitions;

namespace GameDesign4.Interaction.Contracts
{
    /// <summary>
    /// 交互模式控制器接口。
    /// 负责向外暴露当前交互模式切换与模式入口能力。
    /// </summary>
    public interface IInteractionModeController
    {
        /// <summary>
        /// 当前交互模式。
        /// </summary>
        InteractionModeType CurrentMode { get; }

        #region 模式切换
        /// <summary>
        /// 切换到指定交互模式。
        /// </summary>
        void SwitchMode(InteractionModeType mode);
        #endregion

        #region 模式入口
        /// <summary>
        /// 进入建造模式并初始化指定蓝图的建造流程。
        /// </summary>
        void EnterBuildMode(string blueprintId);

        /// <summary>
        /// 进入部署模式并初始化指定实体的部署流程。
        /// </summary>
        void EnterDeploymentMode(EntityDef deployableEntity);
        #endregion
    }
}
