using System.Collections.Generic;
using GameDesign4.Build.Contracts;
using GameDesign4.Infrastructure.Utilities;

namespace GameDesign4.Build.Runtime
{
    /// <summary>
    /// 建筑生产能力注册表。
    /// 负责记录当前场景中已建成的生产建筑数量。
    /// </summary>
    public sealed class BuildingRegistry : IBuildingRegistry
    {
        // XXX: 当前注册表只覆盖运行时新增建筑；若场景存在预摆建筑或后续支持建筑拆除，需要补初始化扫描与注销流程。
        private readonly Dictionary<string, int> buildingCountLookup = new Dictionary<string, int>();

        #region 生产建筑登记
        /// <summary>
        /// 登记一个已建成建筑。
        /// </summary>
        public void RegisterBuilding(string buildingId)
        {
            Guard.EnsureNotNullOrWhiteSpace(buildingId, nameof(buildingId));

            if (buildingCountLookup.TryGetValue(buildingId, out int count) == false)
            {
                buildingCountLookup.Add(buildingId, 1);
                return;
            }

            buildingCountLookup[buildingId] = count + 1;
        }

        /// <summary>
        /// 获取指定建筑当前已建成数量。
        /// </summary>
        public int GetBuildingCount(string buildingId)
        {
            Guard.EnsureNotNullOrWhiteSpace(buildingId, nameof(buildingId));

            if (buildingCountLookup.TryGetValue(buildingId, out int count) == false)
            {
                return 0;
            }

            return count;
        }
        #endregion
    }
}
