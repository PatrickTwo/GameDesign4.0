using GameDesign4.Combat.Runtime;
using GameDesign4.Unit.Contracts.Identity;
using GameDesign4.Unit.Contracts.Model;
using UnityEngine;

namespace GameDesign4.Combat.Component
{
    /// <summary>
    /// 单位战斗代理组件。
    /// 负责保存当前单位参与普通攻击所需的最小战斗数据。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnitCombatAgent : MonoBehaviour
    {
        [Header("战斗属性")]
        [SerializeField] private int campId;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackRange = 5f;
        [SerializeField] private float attackInterval = 1f;
        [SerializeField] private float autoSearchRange = 8f;

        private IUnitIdentity unitIdentity;
        private float lastAttackTime = float.MinValue;

        /// <summary>
        /// 单位唯一标识。
        /// </summary>
        public UnitId UnitId
        {
            get
            {
                ResolveUnitIdentity();
                return unitIdentity == null ? null : unitIdentity.UnitId;
            }
        }

        /// <summary>
        /// 阵营标识。
        /// </summary>
        public int CampId => campId;

        /// <summary>
        /// 最大生命值。
        /// </summary>
        public float MaxHealth => maxHealth;

        /// <summary>
        /// 当前生命值。
        /// </summary>
        public float CurrentHealth => currentHealth;

        /// <summary>
        /// 单次攻击伤害。
        /// </summary>
        public float AttackDamage => attackDamage;

        /// <summary>
        /// 攻击距离。
        /// </summary>
        public float AttackRange => attackRange;

        /// <summary>
        /// 攻击间隔。
        /// </summary>
        public float AttackInterval => attackInterval;

        /// <summary>
        /// 自动索敌范围。
        /// </summary>
        public float AutoSearchRange => autoSearchRange;

        /// <summary>
        /// 上次攻击时间。
        /// </summary>
        public float LastAttackTime => lastAttackTime;

        /// <summary>
        /// 当前是否存活。
        /// </summary>
        public bool IsAlive => currentHealth > 0f;

        #region 生命周期
        /// <summary>
        /// 初始化单位身份与生命值。
        /// </summary>
        private void Awake()
        {
            ResolveUnitIdentity();
        }

        /// <summary>
        /// 注册当前战斗代理。
        /// </summary>
        private void OnEnable()
        {
            RefreshRegistration();
        }

        /// <summary>
        /// 注销当前战斗代理。
        /// </summary>
        private void OnDisable()
        {
            CombatAgentRegistry.Unregister(UnitId);
        }
        #endregion

        #region 战斗数据更新
        /// <summary>
        /// 判断当前是否可以执行攻击。
        /// </summary>
        public bool CanAttack(float currentTime)
        {
            return currentTime - lastAttackTime >= attackInterval;
        }

        /// <summary>
        /// 记录本次攻击时间。
        /// </summary>
        public void MarkAttack(float currentTime)
        {
            lastAttackTime = currentTime;
        }

        /// <summary>
        /// 承受一次伤害。
        /// </summary>
        public void ReceiveDamage(float damage)
        {
            currentHealth -= damage;

            if (currentHealth < 0f)
            {
                currentHealth = 0f;
            }
        }

        /// <summary>
        /// 刷新当前代理注册状态。
        /// 用于确保身份解析完成后，战斗注册表持有最新代理映射。
        /// </summary>
        public void RefreshRegistration()
        {
            ResolveUnitIdentity();
            UnitId currentUnitId = unitIdentity == null ? null : unitIdentity.UnitId;

            if (currentUnitId == null)
            {
                return;
            }

            CombatAgentRegistry.Register(this);
        }

        /// <summary>
        /// 解析同物体上的单位身份提供者。
        /// 使用组件扫描而不是接口泛型获取，避免 Unity 对接口组件查询不稳定。
        /// </summary>
        private void ResolveUnitIdentity()
        {
            if (unitIdentity != null)
            {
                return;
            }

            UnityEngine.Component[] components = GetComponents<UnityEngine.Component>();
            for (int index = 0; index < components.Length; index++)
            {
                UnityEngine.Component component = components[index];
                if (component is IUnitIdentity resolvedIdentity)
                {
                    unitIdentity = resolvedIdentity;
                    return;
                }
            }
        }
        #endregion
    }
}
