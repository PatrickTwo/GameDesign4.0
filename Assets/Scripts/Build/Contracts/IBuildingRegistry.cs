using GameDesign4.Build.Definition;
using UnityEngine;

namespace GameDesign4.Build.Contracts
{
    /// <summary>
    /// 建筑注册表接口。
    /// 负责向外暴露当前已建成建筑数量与部署出生点查询能力。
    /// </summary>
    public interface IBuildingRegistry
    {
        #region 建筑查询
        /// <summary>
        /// 获取指定建筑当前已建成数量。
        /// </summary>
        int GetBuildingCount(BuildingDef buildingDef);

        /// <summary>
        /// 尝试获取离目标点最近的部署出生点。
        /// </summary>
        bool TryGetNearestDeploySpawnPoint(BuildingDef buildingDef, Vector3 targetPosition, out Transform deploySpawnPoint);
        #endregion
    }
}
