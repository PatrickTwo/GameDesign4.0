using System;
using System.Collections.Generic;
using System.Reflection;
using GameDesign4.Combat.Component;
using GameDesign4.Combat.Runtime;
using GameDesign4.Unit.Component;
using GameDesign4.Unit.Contracts.Model;
using NUnit.Framework;
using UnityEngine;

namespace GameDesign4.Tests.Combat
{
    /// <summary>
    /// Combat 规则服务测试。
    /// 覆盖最小自动索敌、目标有效性、距离判断与攻击结算行为。
    /// </summary>
    public sealed class CombatRuleServiceTests
    {
        private readonly List<GameObject> createdGameObjects = new List<GameObject>();

        #region 初始化
        /// <summary>
        /// 每个用例开始前清理静态注册表，避免前序测试残留影响结果。
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            ClearCombatAgentRegistry();
        }
        #endregion

        #region 自动索敌测试
        /// <summary>
        /// 自动索敌应选择最近的敌方有效目标。
        /// </summary>
        [Test]
        public void TryGetAutoAttackTarget_ShouldReturnNearestEnemy()
        {
            UnitCombatAgent sourceAgent = CreateCombatAgent(1, Vector3.zero, 10f, 5f, 1f, 8f, 100f);
            UnitCombatAgent farEnemyAgent = CreateCombatAgent(2, new Vector3(6f, 0f, 0f), 10f, 5f, 1f, 8f, 100f);
            UnitCombatAgent nearEnemyAgent = CreateCombatAgent(2, new Vector3(3f, 0f, 0f), 10f, 5f, 1f, 8f, 100f);
            CombatRuleService combatRuleService = new CombatRuleService();

            bool hasTarget = combatRuleService.TryGetAutoAttackTarget(sourceAgent.UnitId, out UnitId targetUnitId);

            Assert.That(hasTarget, Is.True);
            Assert.That(targetUnitId, Is.EqualTo(nearEnemyAgent.UnitId));
            Assert.That(targetUnitId, Is.Not.EqualTo(farEnemyAgent.UnitId));
        }
        #endregion

        #region 目标校验测试
        /// <summary>
        /// 同阵营目标应视为无效。
        /// </summary>
        [Test]
        public void IsTargetValid_ShouldReturnFalse_ForSameCamp()
        {
            UnitCombatAgent sourceAgent = CreateCombatAgent(1, Vector3.zero, 10f, 5f, 1f, 8f, 100f);
            UnitCombatAgent friendlyAgent = CreateCombatAgent(1, new Vector3(2f, 0f, 0f), 10f, 5f, 1f, 8f, 100f);
            CombatRuleService combatRuleService = new CombatRuleService();

            bool isValid = combatRuleService.IsTargetValid(sourceAgent.UnitId, friendlyAgent.UnitId);

            Assert.That(isValid, Is.False);
        }

        /// <summary>
        /// 死亡目标应视为无效。
        /// </summary>
        [Test]
        public void IsTargetValid_ShouldReturnFalse_ForDeadTarget()
        {
            UnitCombatAgent sourceAgent = CreateCombatAgent(1, Vector3.zero, 10f, 5f, 1f, 8f, 100f);
            UnitCombatAgent deadEnemyAgent = CreateCombatAgent(2, new Vector3(2f, 0f, 0f), 10f, 5f, 1f, 8f, 0f);
            CombatRuleService combatRuleService = new CombatRuleService();

            bool isValid = combatRuleService.IsTargetValid(sourceAgent.UnitId, deadEnemyAgent.UnitId);

            Assert.That(isValid, Is.False);
        }
        #endregion

        #region 攻击范围测试
        /// <summary>
        /// 在攻击距离内应返回 true。
        /// </summary>
        [Test]
        public void IsTargetInAttackRange_ShouldReturnTrue_WhenWithinRange()
        {
            UnitCombatAgent sourceAgent = CreateCombatAgent(1, Vector3.zero, 10f, 5f, 1f, 8f, 100f);
            UnitCombatAgent enemyAgent = CreateCombatAgent(2, new Vector3(4f, 0f, 0f), 10f, 5f, 1f, 8f, 100f);
            CombatRuleService combatRuleService = new CombatRuleService();

            bool inRange = combatRuleService.IsTargetInAttackRange(sourceAgent.UnitId, sourceAgent.transform.position, enemyAgent.UnitId, enemyAgent.transform.position);

            Assert.That(inRange, Is.True);
        }

        /// <summary>
        /// 超出攻击距离应返回 false。
        /// </summary>
        [Test]
        public void IsTargetInAttackRange_ShouldReturnFalse_WhenOutOfRange()
        {
            UnitCombatAgent sourceAgent = CreateCombatAgent(1, Vector3.zero, 10f, 3f, 1f, 8f, 100f);
            UnitCombatAgent enemyAgent = CreateCombatAgent(2, new Vector3(4f, 0f, 0f), 10f, 5f, 1f, 8f, 100f);
            CombatRuleService combatRuleService = new CombatRuleService();

            bool inRange = combatRuleService.IsTargetInAttackRange(sourceAgent.UnitId, sourceAgent.transform.position, enemyAgent.UnitId, enemyAgent.transform.position);

            Assert.That(inRange, Is.False);
        }
        #endregion

        #region 攻击执行测试
        /// <summary>
        /// 执行攻击后应正确扣减目标生命值。
        /// </summary>
        [Test]
        public void TryExecuteAttack_ShouldReduceTargetHealth()
        {
            UnitCombatAgent sourceAgent = CreateCombatAgent(1, Vector3.zero, 15f, 5f, 0f, 8f, 100f);
            UnitCombatAgent enemyAgent = CreateCombatAgent(2, new Vector3(2f, 0f, 0f), 10f, 5f, 1f, 8f, 100f);
            CombatRuleService combatRuleService = new CombatRuleService();

            bool hasExecuted = combatRuleService.TryExecuteAttack(sourceAgent.UnitId, enemyAgent.UnitId);

            Assert.That(hasExecuted, Is.True);
            Assert.That(enemyAgent.CurrentHealth, Is.EqualTo(85f));
        }

        /// <summary>
        /// 冷却中重复攻击应被阻止。
        /// </summary>
        [Test]
        public void TryExecuteAttack_ShouldReturnFalse_WhenStillCoolingDown()
        {
            UnitCombatAgent sourceAgent = CreateCombatAgent(1, Vector3.zero, 15f, 5f, 100f, 8f, 100f);
            UnitCombatAgent enemyAgent = CreateCombatAgent(2, new Vector3(2f, 0f, 0f), 10f, 5f, 1f, 8f, 100f);
            CombatRuleService combatRuleService = new CombatRuleService();

            bool firstExecuteResult = combatRuleService.TryExecuteAttack(sourceAgent.UnitId, enemyAgent.UnitId);
            bool secondExecuteResult = combatRuleService.TryExecuteAttack(sourceAgent.UnitId, enemyAgent.UnitId);

            Assert.That(firstExecuteResult, Is.True);
            Assert.That(secondExecuteResult, Is.False);
            Assert.That(enemyAgent.CurrentHealth, Is.EqualTo(85f));
        }
        #endregion

        #region 收尾
        /// <summary>
        /// 清理测试创建对象。
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            for (int index = 0; index < createdGameObjects.Count; index++)
            {
                GameObject createdGameObject = createdGameObjects[index];
                if (createdGameObject != null)
                {
                    UnityEngine.Object.DestroyImmediate(createdGameObject);
                }
            }

            createdGameObjects.Clear();
            ClearCombatAgentRegistry();
        }
        #endregion

        #region 测试辅助
        /// <summary>
        /// 创建一个带单位身份与战斗参数的战斗代理。
        /// </summary>
        private UnitCombatAgent CreateCombatAgent(
            int campId,
            Vector3 position,
            float attackDamage,
            float attackRange,
            float attackInterval,
            float autoSearchRange,
            float currentHealth)
        {
            GameObject gameObject = new GameObject("CombatAgentTestObject");
            gameObject.transform.position = position;
            createdGameObjects.Add(gameObject);

            UnitEntity unitEntity = gameObject.AddComponent<UnitEntity>();
            UnitCombatAgent combatAgent = gameObject.AddComponent<UnitCombatAgent>();

            SetPrivateField(combatAgent, "campId", campId);
            SetPrivateField(combatAgent, "attackDamage", attackDamage);
            SetPrivateField(combatAgent, "attackRange", attackRange);
            SetPrivateField(combatAgent, "attackInterval", attackInterval);
            SetPrivateField(combatAgent, "autoSearchRange", autoSearchRange);
            SetPrivateField(combatAgent, "maxHealth", 100f);
            SetPrivateField(combatAgent, "currentHealth", currentHealth);

            // 测试里不依赖 DI，只确保对象持有唯一 UnitId。
            Assert.That(unitEntity.UnitId, Is.Not.Null);
            combatAgent.RefreshRegistration();
            Assert.That(combatAgent.UnitId, Is.Not.Null);
            AssertAgentRegistered(combatAgent);
            return combatAgent;
        }

        /// <summary>
        /// 使用反射写入私有字段，便于搭建最小测试数据。
        /// </summary>
        private void SetPrivateField(object target, string fieldName, object fieldValue)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(fieldInfo, Is.Not.Null);
            fieldInfo.SetValue(target, fieldValue);
        }

        /// <summary>
        /// 断言代理已成功写入战斗注册表。
        /// </summary>
        private void AssertAgentRegistered(UnitCombatAgent combatAgent)
        {
            Type registryType = typeof(CombatRuleService).Assembly.GetType("GameDesign4.Combat.Runtime.CombatAgentRegistry");
            Assert.That(registryType, Is.Not.Null);

            MethodInfo tryGetAgentMethod = registryType.GetMethod("TryGetAgent", BindingFlags.Public | BindingFlags.Static);
            Assert.That(tryGetAgentMethod, Is.Not.Null);

            object[] arguments = { combatAgent.UnitId, null };
            Assert.That((bool)tryGetAgentMethod.Invoke(null, arguments), Is.True);
            Assert.That(arguments[1], Is.EqualTo(combatAgent));
        }

        /// <summary>
        /// 清空战斗代理静态注册表。
        /// </summary>
        private void ClearCombatAgentRegistry()
        {
            Type registryType = typeof(CombatRuleService).Assembly.GetType("GameDesign4.Combat.Runtime.CombatAgentRegistry");
            Assert.That(registryType, Is.Not.Null);

            MethodInfo clearMethod = registryType.GetMethod("Clear", BindingFlags.Public | BindingFlags.Static);
            Assert.That(clearMethod, Is.Not.Null);
            clearMethod.Invoke(null, null);
        }
        #endregion
    }
}
