using UnityEngine;
using StarDefense.Core;
using StarDefense.Enemy;

namespace StarDefense.Hero
{
    public class HeroBase : MonoBehaviour, IHero
    {
        [Header("영웅 설정")]
        [SerializeField] private int heroId;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private HeroTribe tribe;
        [SerializeField] private Sprite projectileSprite;

        [Header("범위 공격 설정")]
        [SerializeField] private float splashRadius;

        [Header("탐색")]
        [SerializeField] private LayerMask enemyLayer;


        private HeroRarity rarity;
        private float attackRange;
        private float attackSpeed;
        private int attackDamage;

        private static readonly Collider2D[] hitBuffer = new Collider2D[200];
        private EnemyBase currentTarget;

        private ProjectilePool projectilePool;
        private IAttackStrategy attackStrategy;

        private float attackTimer;

        public HeroRarity Rarity => rarity;
        public HeroTribe Tribe => tribe;
        public int AttackDamage => attackDamage;
        public float AttackRange => attackRange;
        public float AttackSpeed => attackSpeed;
        public bool CanUpgrade => rarity < HeroRarity.Legend;
        public Transform Transform => transform;

        #region 초기화
        /// <summary>
        /// HeroData로 영웅 스탯을 초기화
        /// </summary>
        public void Initialize(Data.HeroData heroData, ProjectilePool mProjectilePool)
        {
            rarity = System.Enum.Parse<HeroRarity>(heroData.rarity);
            attackDamage = heroData.attackDamage;
            attackSpeed = heroData.attackSpeed;
            attackRange = heroData.attackRange;
            projectilePool = mProjectilePool;
            attackTimer = 1f / attackSpeed;

            SetAttackStrategy();
        }

        /// <summary>
        /// tribe에 따라 공격 전략을 자동 결정
        /// </summary>
        private void SetAttackStrategy()
        {
            attackStrategy = tribe switch
            {
                HeroTribe.Human => new SingleAttackStrategy(),
                HeroTribe.Alien => new SplashAttackStrategy(splashRadius),
                _ => new SingleAttackStrategy()
            };
        }
        #endregion

        #region 유니티 Event
        private void Start()
        {
            projectilePool = FindObjectOfType<ProjectilePool>();
            Data.HeroData heroData = Managers.DataManager.GetTable<Data.HeroData>().Get(heroId);

            if (heroData != null)
            {
                Initialize(heroData, projectilePool);
            }
        }

        private void Update()
        {
            if (attackStrategy == null || projectilePool == null) return;

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
        /// <summary>
        /// 투사체 발사
        /// </summary>
        public void Attack()
        {
            if (currentTarget == null || projectilePool == null || attackStrategy == null) return;

            Vector3 targetPos = currentTarget.Transform.position;

            attackStrategy.Execute(transform.position, targetPos, attackDamage, currentTarget, projectileSprite, projectilePool);
        }
        #endregion
    }
}