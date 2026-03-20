using UnityEngine;
using StarDefense.Enemy;

namespace StarDefense.Hero
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float speed; 

        private Vector3 targetPosition;
        private int damage;
        private EnemyBase target;
        private float splashRadius;

        private bool isActive;

        private ProjectilePool pool;

        #region 초기화

        public void Initialize(Vector3 mTargetPosition, int mDamage, EnemyBase mTarget, Sprite mSprite)
        {
            Initialize(mTargetPosition, mDamage, mTarget, mSprite, 0f);
        }

        public void Initialize(Vector3 mTargetPosition, int mDamage, EnemyBase mTarget, Sprite mSprite, float mSplashRadius)
        {
            targetPosition = mTargetPosition;
            damage = mDamage;
            target = mTarget;
            splashRadius = mSplashRadius;
            isActive = true;
            if (spriteRenderer != null && mSprite != null)
            {
                spriteRenderer.sprite = mSprite;
            }
            Vector3 dir = (targetPosition - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        public void SetPool(ProjectilePool mPool)
        {
            pool = mPool;
        }
        #endregion

        #region 유니티 Event
        private void Update()
        {
            if (!isActive) return;
            Move();
        }
        #endregion

        #region 이동 및 도달 처리
        private void Move()
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                OnReachTarget();
            }
        }

        private void OnReachTarget()
        {
            isActive = false;
            if (splashRadius > 0f)
            {
                ApplySplashDamage();
            }
            else
            {
                ApplySingleDamage();
            }
            ReturnToPool();
        }

        private void ApplySingleDamage()
        {
            if (target != null && target.IsAlive)
            {
                target.TakeDamage(damage);
            }
        }

        private void ApplySplashDamage()
        {
            EnemyBase[] enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
            foreach (EnemyBase enemy in enemies)
            {
                if (!enemy.IsAlive) continue;
                float dist = Vector3.Distance(targetPosition, enemy.Transform.position);
                if (dist <= splashRadius)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
        #endregion

        #region ObjectPool관련
        private void ReturnToPool()
        {
            if (pool != null)
            {
                pool.Return(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        #endregion
    }
}