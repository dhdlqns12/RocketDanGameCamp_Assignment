using UnityEngine;
using StarDefense.Core;
using StarDefense.Enemy;

namespace StarDefense.Hero
{
    /// <summary>
    /// 영웅 클래스. 데이터 드리븐 방식
    /// HeroData에서 스탯, heroId 기반으로 스프라이트 동적 로드
    /// Hero: Resources/Sprite/Hero/{heroId}
    /// Projectile: Resources/Sprite/Projectile/{heroId + 1000}
    /// 추후 상태전환 늘어나면 FSM으로 변경 고려
    /// </summary>
    public class HeroBase : MonoBehaviour, IHero
    {
        [Header("적 체크")]
        [SerializeField] private LayerMask enemyLayer;

        [Header("승급 표시")]
        [SerializeField] private GameObject upgradeIndicator;

        private SpriteRenderer spriteRenderer;
        private HeroTribe tribe;
        private Sprite projectileSprite;

        public int heroId { get; private set; }

        private HeroRarity rarity;
        private float attackRange;
        private float attackSpeed;
        private int attackDamage;
        private float splashRadius;

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
        /// HeroData에서 스탯 + heroId 기반 스프라이트 초기화
        /// </summary>
        public void Init(Data.HeroData heroData, ProjectilePool mProjectilePool)
        {
            heroId = heroData.heroId;
            rarity = System.Enum.Parse<HeroRarity>(heroData.rarity);
            tribe = System.Enum.Parse<HeroTribe>(heroData.tribe);
            attackDamage = heroData.attackDamage;
            attackSpeed = heroData.attackSpeed;
            attackRange = heroData.attackRange;
            splashRadius = heroData.splashRadius;
            projectilePool = mProjectilePool;
            attackTimer = 1f / attackSpeed;

            LoadSprites(heroData);
            SetAttackStrategy();

            Debug.Log($"[HeroBase] Init | {heroData.heroName} | {rarity} | {tribe} | dmg: {attackDamage} | spd: {attackSpeed} | range: {attackRange} | splash: {splashRadius}");
        }

        /// <summary>
        /// HeroData의 스프라이트 ID로 동적 로드
        /// </summary>
        private void LoadSprites(Data.HeroData heroData)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            Sprite heroSpr = Resources.Load<Sprite>($"Sprite/Hero/{heroData.heroSprite}");

            if (heroSpr != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = heroSpr;
            }

            projectileSprite = Resources.Load<Sprite>($"Sprite/Projectile/{heroData.projectileSprite}");
        }

        /// <summary>
        /// splashRadius에 따라 공격 전략을 자동 결정
        /// </summary>
        private void SetAttackStrategy()
        {
            if (splashRadius > 0f)
            {
                attackStrategy = new SplashAttackStrategy(splashRadius);
            }
            else
            {
                attackStrategy = new SingleAttackStrategy();
            }
        }

        /// <summary>
        /// 버프 타일 효과 적용. 공속 증가 비율 (0.3 = 30%)
        /// </summary>
        public void ApplyBuffTile(float attackSpeedBonus)
        {
            attackSpeed *= (1f + attackSpeedBonus);
        }
        #endregion

        #region 유니티 Event
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

        #region 승급 표시
        /// <summary>
        /// 승급 가능 표시 활성/비활성
        /// </summary>
        public void SetUpgradeIndicator(bool active)
        {
            if (upgradeIndicator != null)
            {
                upgradeIndicator.SetActive(active);
            }
        }
        #endregion
    }
}