namespace GameDesign4.CameraControl.Contracts.Service
{
    /// <summary>
    /// 相机控制服务契约。
    /// 对外提供相机控制启用与关闭能力。
    /// </summary>
    public interface ICameraControlService
    {
        /// <summary>
        /// 启用相机控制。
        /// </summary>
        void EnableControl();

        /// <summary>
        /// 关闭相机控制。
        /// </summary>
        void DisableControl();
    }
}
