using System;
using UnityEngine;
using StarDefense.Core;
using StarDefense.Enemy;

namespace StarDefense.Hero
{
    public class Commander : MonoBehaviour
    {
        [Header("지휘관 설정")]
        [SerializeField] private int maxHp;
        [SerializeField] private float attackRange;
        [SerializeField] private float attackSpeed;
        [SerializeField] private int attackDamage;
        [SerializeField] private Sprite projectileSprite;

        [Header("공격 전략")]
        [SerializeField] private AttackStrategyType strategyType;
        [SerializeField] private float splashRadius;

        [Header("탐색")]
        [SerializeField] private LayerMask enemyLayer;

        private int currentHp;
        private static readonly Collider2D[] hitBuffer = new Collider2D[200];
        private EnemyBase currentTarget;
        private ProjectilePool projectilePool;
        private IAttackStrategy attackStrategy;
        private float attackTimer;

        public int MaxHp => maxHp;
        public int CurrentHp => currentHp;
        public bool IsAlive => currentHp > 0;

        /// <summary>
        /// HP 변경 시 발행
        /// </summary>
        public event Action<int, int> OnHpChanged;

        /// <summary>
        /// 지휘관 사망 시 발행 
        /// </summary>
        public event Action OnCommanderDead;

        #region 초기화
        public void Init(ProjectilePool mProjectilePool)
        {
            currentHp = maxHp;
            projectilePool = mProjectilePool;
            attackTimer = 1f / attackSpeed;

            SetAttackStrategy();

            OnHpChanged?.Invoke(currentHp, maxHp);
        }

        private void SetAttackStrategy()
        {
            attackStrategy = strategyType switch
            {
                AttackStrategyType.Single => new SingleAttackStrategy(),
                AttackStrategyType.Splash => new SplashAttackStrategy(splashRadius),
                _ => new SingleAttackStrategy()
            };
        }
        #endregion

        #region 유니티 Event
        private void Update()
        {
            if (projectilePool == null || attackStrategy == null || !IsAlive) return;

            attackTimer -= Time.deltaTime;

            if (attackTimer > 0f) return;

            FindTarget();

            if (currentTarget != null)
            {
                Attack();
                attackTimer = 1f / attackSpeed;
            }
        }
        #endregion

        #region 타겟 감지
        private void FindTarget()
        {
            currentTarget = null;

            float closestDist = float.MaxValue;
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, attackRange, hitBuffer, enemyLayer);

            for (int i = 0; i < count; i++)
            {
                if (!hitBuffer[i].TryGetComponent(out EnemyBase enemy)) continue;

                if (!enemy.IsAlive) continue;

                float dist = Vector3.Distance(transform.position, enemy.Transform.position);

                if (dist > attackRange) continue;

                if (dist < closestDist)
                {
                    closestDist = dist;
                    currentTarget = enemy;
                }
            }
        }
        #endregion

        #region 공격
        private void Attack()
        {
            if (currentTarget == null || projectilePool == null || attackStrategy == null) return;

            Vector3 targetPos = currentTarget.Transform.position;

            attackStrategy.Execute(transform.position, targetPos, attackDamage, currentTarget, projectileSprite, projectilePool);
        }
        #endregion

        #region 데미지 처리
        /// <summary>
        /// 적이 경로 끝에 도달했을 때 호출
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (currentHp <= 0) return;

            currentHp -= damage;

            if (currentHp <= 0)
            {
                currentHp = 0;
            }

            OnHpChanged?.Invoke(currentHp, maxHp);

            if (currentHp <= 0)
            {
                OnDeath();
            }
        }

        private void OnDeath()
        {
            Debug.Log("지휘관 사망");

            OnCommanderDead?.Invoke();
        }
        #endregion

        #region Gizmo
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
        #endregion
    }
}