using UnityEngine;
using UnityEngine.AI;

namespace GameDesign4.Unit.Runtime
{
    /// <summary>
    /// 单位移动执行器。
    /// 负责普通移动和追击移动。
    /// </summary>
    public sealed class UnitMovementExecutor
    {
        private readonly NavMeshAgent navMeshAgent;

        /// <summary>
        /// 构造移动执行器。
        /// </summary>
        public UnitMovementExecutor(NavMeshAgent navMeshAgent)
        {
            this.navMeshAgent = navMeshAgent;
        }

        #region 移动执行
        /// <summary>
        /// 执行一次移动 Tick。
        /// 返回值表示是否已经到达目标点。
        /// </summary>
        public bool TickMove(Vector3 destination)
        {
            if (navMeshAgent == null || navMeshAgent.enabled == false || navMeshAgent.isOnNavMesh == false)
            {
                return true;
            }

            // 统一由 NavMeshAgent 承担寻路与移动。
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(destination);
            return HasReachedDestination();
        }

        /// <summary>
        /// 停止当前移动。
        /// </summary>
        public void Stop()
        {
            if (navMeshAgent == null || navMeshAgent.enabled == false || navMeshAgent.isOnNavMesh == false)
            {
                return;
            }

            navMeshAgent.isStopped = true;

            // 进入攻击或待机时清掉旧路径，避免路径残留继续推动移动。
            if (navMeshAgent.hasPath)
            {
                navMeshAgent.ResetPath();
            }
        }
        #endregion

        #region 到点判断
        /// <summary>
        /// 判断当前是否到达目标点。
        /// </summary>
        private bool HasReachedDestination()
        {
            if (navMeshAgent.pathPending)
            {
                return false;
            }

            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                return false;
            }

            return navMeshAgent.hasPath == false || navMeshAgent.velocity.sqrMagnitude <= 0.01f;
        }
        #endregion
    }
}
