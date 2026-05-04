using System.Collections.Generic;
using GameDesign4.Build.Component;
using GameDesign4.Build.Contracts;
using GameDesign4.Build.Definition;
using GameDesign4.Infrastructure.Utilities;
using UnityEngine;

namespace GameDesign4.Build.Runtime
{
    /// <summary>
    /// 建筑注册表。
    /// 负责记录当前场景中已建成建筑实例，并提供数量和最近出生点查询能力。
    /// </summary>
    public sealed class BuildingRegistry : IBuildingRegistry
    {
        // XXX: 当前注册表只覆盖运行时新增建筑；若场景存在预摆建筑或后续支持建筑拆除，需要补初始化扫描与注销流程。
        private readonly Dictionary<BuildingDef, List<BuildingEntity>> buildingLookup = new Dictionary<BuildingDef, List<BuildingEntity>>();

        #region 建筑登记
        /// <summary>
        /// 登记一个已建成建筑实例。
        /// </summary>
        public void RegisterBuilding(BuildingEntity buildingEntity)
        {
            Guard.EnsureNotNull(buildingEntity, nameof(buildingEntity));
            Guard.EnsureNotNull("建筑实例缺少定义。", buildingEntity.Definition);
            Guard.EnsureNotNullOrWhiteSpace(buildingEntity.Definition.Id, nameof(buildingEntity.Definition.Id));

            BuildingDef buildingDef = buildingEntity.Definition;
            if (buildingLookup.TryGetValue(buildingDef, out List<BuildingEntity> buildingEntities) == false)
            {
                buildingEntities = new List<BuildingEntity>();
                buildingLookup.Add(buildingDef, buildingEntities);
            }

            if (buildingEntities.Contains(buildingEntity) == false)
            {
                buildingEntities.Add(buildingEntity);
            }
        }

        /// <summary>
        /// 获取指定建筑当前已建成数量。
        /// </summary>
        public int GetBuildingCount(BuildingDef buildingDef)
        {
            Guard.EnsureNotNull(buildingDef, nameof(buildingDef));

            if (buildingLookup.TryGetValue(buildingDef, out List<BuildingEntity> buildingEntities) == false)
            {
                return 0;
            }

            return buildingEntities.Count;
        }

        /// <summary>
        /// 尝试获取离目标点最近的部署出生点。
        /// </summary>
        public bool TryGetNearestDeploySpawnPoint(BuildingDef buildingDef, Vector3 targetPosition, out Transform deploySpawnPoint)
        {
            Guard.EnsureNotNull(buildingDef, nameof(buildingDef));

            deploySpawnPoint = null;
            if (buildingLookup.TryGetValue(buildingDef, out List<BuildingEntity> buildingEntities) == false)
            {
                return false;
            }

            float nearestDistanceSqr = float.MaxValue;
            for (int index = 0; index < buildingEntities.Count; index++)
            {
                BuildingEntity buildingEntity = buildingEntities[index];
                if (buildingEntity == null || buildingEntity.DeploySpawnPoint == null)
                {
                    continue;
                }

                Vector3 offset = buildingEntity.transform.position - targetPosition;
                float distanceSqr = offset.sqrMagnitude;
                if (distanceSqr >= nearestDistanceSqr)
                {
                    continue;
                }

                nearestDistanceSqr = distanceSqr;
                deploySpawnPoint = buildingEntity.DeploySpawnPoint;
            }

            return deploySpawnPoint != null;
        }
        #endregion
    }
}
