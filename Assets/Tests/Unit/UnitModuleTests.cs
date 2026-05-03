using System.Collections.Generic;
using GameDesign4.Unit.Component;
using GameDesign4.Unit.Contracts.Command;
using GameDesign4.Unit.Contracts.Model;
using GameDesign4.Unit.Registry;
using GameDesign4.Unit.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace GameDesign4.Tests.Unit
{
    /// <summary>
    /// Unit 模块核心测试。
    /// 仅覆盖当前最稳定的运行时标识、命令上下文和注册表行为。
    /// </summary>
    public sealed class UnitModuleTests
    {
        private readonly List<GameObject> createdGameObjects = new List<GameObject>();

        #region UnitId 测试
        /// <summary>
        /// 构造 UnitId 时应自动生成有效且唯一的标识。
        /// </summary>
        [Test]
        public void UnitId_Constructor_ShouldGenerateValidAndUniqueIdentifier()
        {
            UnitId firstUnitId = new UnitId();
            UnitId secondUnitId = new UnitId();

            Assert.That(firstUnitId, Is.Not.Null);
            Assert.That(secondUnitId, Is.Not.Null);
            Assert.That(firstUnitId.IsValid, Is.True);
            Assert.That(secondUnitId.IsValid, Is.True);
            Assert.That(firstUnitId.Value, Is.Not.EqualTo(secondUnitId.Value));
        }

        /// <summary>
        /// UnitId 的字符串输出应与内部值一致。
        /// </summary>
        [Test]
        public void UnitId_ToString_ShouldReturnUnderlyingValue()
        {
            UnitId unitId = new UnitId();

            Assert.That(unitId.ToString(), Is.EqualTo(unitId.Value));
        }
        #endregion

        #region 命令上下文测试
        /// <summary>
        /// 写入移动命令后，应标记为手动移动并清空攻击目标。
        /// </summary>
        [Test]
        public void UnitCommandContext_SetMoveCommand_ShouldMarkManualMove()
        {
            UnitId unitId = new UnitId();
            Vector3 destination = new Vector3(3f, 0f, 5f);
            UnitMoveCommand command = new UnitMoveCommand(unitId, destination);
            UnitCommandContext commandContext = new UnitCommandContext();

            commandContext.SetMoveCommand(command);

            Assert.That(commandContext.HasCommand, Is.True);
            Assert.That(commandContext.IsManualCommand, Is.True);
            Assert.That(commandContext.IsMoveCommand, Is.True);
            Assert.That(commandContext.IsAttackCommand, Is.False);
            Assert.That(commandContext.Destination, Is.EqualTo(destination));
            Assert.That(commandContext.TargetUnitId, Is.Null);
        }

        /// <summary>
        /// 写入攻击命令后，应标记为手动攻击并保存目标单位。
        /// </summary>
        [Test]
        public void UnitCommandContext_SetAttackCommand_ShouldMarkManualAttack()
        {
            UnitId unitId = new UnitId();
            UnitId targetUnitId = new UnitId();
            UnitAttackCommand command = new UnitAttackCommand(unitId, targetUnitId);
            UnitCommandContext commandContext = new UnitCommandContext();

            commandContext.SetAttackCommand(command);

            Assert.That(commandContext.HasCommand, Is.True);
            Assert.That(commandContext.IsManualCommand, Is.True);
            Assert.That(commandContext.IsMoveCommand, Is.False);
            Assert.That(commandContext.IsAttackCommand, Is.True);
            Assert.That(commandContext.TargetUnitId, Is.EqualTo(targetUnitId));
        }

        /// <summary>
        /// 写入自动攻击目标后，应标记为自动攻击命令。
        /// </summary>
        [Test]
        public void UnitCommandContext_SetAutoAttackTarget_ShouldMarkAutoAttack()
        {
            UnitId targetUnitId = new UnitId();
            UnitCommandContext commandContext = new UnitCommandContext();

            commandContext.SetAutoAttackTarget(targetUnitId);

            Assert.That(commandContext.HasCommand, Is.True);
            Assert.That(commandContext.IsManualCommand, Is.False);
            Assert.That(commandContext.IsMoveCommand, Is.False);
            Assert.That(commandContext.IsAttackCommand, Is.True);
            Assert.That(commandContext.TargetUnitId, Is.EqualTo(targetUnitId));
        }

        /// <summary>
        /// 清空命令上下文后，应恢复到无命令状态。
        /// </summary>
        [Test]
        public void UnitCommandContext_Clear_ShouldResetState()
        {
            UnitId unitId = new UnitId();
            UnitMoveCommand command = new UnitMoveCommand(unitId, Vector3.one);
            UnitCommandContext commandContext = new UnitCommandContext();

            commandContext.SetMoveCommand(command);
            commandContext.Clear();

            Assert.That(commandContext.HasCommand, Is.False);
            Assert.That(commandContext.IsManualCommand, Is.False);
            Assert.That(commandContext.IsMoveCommand, Is.False);
            Assert.That(commandContext.IsAttackCommand, Is.False);
            Assert.That(commandContext.TargetUnitId, Is.Null);
            Assert.That(commandContext.Destination, Is.EqualTo(default(Vector3)));
        }
        #endregion

        #region 注册表测试
        /// <summary>
        /// 注册单位后，应能通过 UnitId 查询到对应实例。
        /// </summary>
        [Test]
        public void UnitRegistry_Register_ShouldAllowLookup()
        {
            UnitEntity unitEntity = CreateUnitEntityWithRuntimeId();

            UnitRegistry.Register(unitEntity);
            bool hasUnit = UnitRegistry.TryGetUnit(unitEntity.UnitId, out UnitEntity resolvedUnitEntity);

            Assert.That(hasUnit, Is.True);
            Assert.That(resolvedUnitEntity, Is.EqualTo(unitEntity));
        }

        /// <summary>
        /// 注销单位后，应无法再通过 UnitId 查询到实例。
        /// </summary>
        [Test]
        public void UnitRegistry_Unregister_ShouldRemoveLookup()
        {
            UnitEntity unitEntity = CreateUnitEntityWithRuntimeId();

            UnitRegistry.Register(unitEntity);
            UnitRegistry.Unregister(unitEntity.UnitId);
            bool hasUnit = UnitRegistry.TryGetUnit(unitEntity.UnitId, out UnitEntity resolvedUnitEntity);

            Assert.That(hasUnit, Is.False);
            Assert.That(resolvedUnitEntity, Is.Null);
        }
        #endregion

        #region 测试收尾
        /// <summary>
        /// 清理测试中创建的场景对象。
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            for (int index = 0; index < createdGameObjects.Count; index++)
            {
                GameObject createdGameObject = createdGameObjects[index];
                if (createdGameObject != null)
                {
                    Object.DestroyImmediate(createdGameObject);
                }
            }

            createdGameObjects.Clear();
        }
        #endregion

        #region 测试辅助
        /// <summary>
        /// 创建一个带运行时 UnitId 的单位组件。
        /// </summary>
        private UnitEntity CreateUnitEntityWithRuntimeId()
        {
            GameObject gameObject = new GameObject("UnitEntityTestObject");
            createdGameObjects.Add(gameObject);

            UnitEntity unitEntity = gameObject.AddComponent<UnitEntity>();
            Assert.That(unitEntity.UnitId, Is.Not.Null);
            return unitEntity;
        }
        #endregion
    }
}
