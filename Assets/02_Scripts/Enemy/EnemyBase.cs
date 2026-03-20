using System;
using System.Collections.Generic;
using UnityEngine;
using StarDefense.Data;

namespace StarDefense.Enemy
{
    /// <summary>
    /// 웨이포인트 이동, 체력, 사망 처리 등 공통 로직
    /// 사망/도착 시 EnemyPool에 자동 반환
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour, IEnemy
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Animator animator;

        private static readonly int DIRX = Animator.StringToHash("dirX");
        private static readonly int DIRY = Animator.StringToHash("dirY");

        protected EnemyData data;
        protected List<Vector3> waypoints;
        protected int currentHp;
        protected int waypointIndex;
        protected bool isAlive;
        protected EnemyPool pool;
        protected string poolKey;

        public int CurrentHp => currentHp;
        public bool IsAlive => isAlive;
        public abstract bool IsAir { get; }
        public Transform Transform => transform;
        public EnemyData Data => data;

        /// <summary>
        /// 적 사망 시 발행
        /// </summary>
        public static event Action<EnemyData> OnEnemyDied;

        /// <summary>
        /// 적 경로 끝 도달 시 발행
        /// </summary>
        public static event Action<EnemyData> OnEnemyReachedEnd;

        #region 유니티 Event
        protected virtual void Update()
        {
            if (!isAlive || waypoints == null) return;
            Move();
        }
        #endregion

        #region 초기화
        public virtual void Initialize(EnemyData mData, List<Vector3> mWaypoints)
        {
            data = mData;
            waypoints = mWaypoints;
            currentHp = data.hp;
            waypointIndex = 0;
            isAlive = true;
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }
            transform.position = waypoints[0];
        }
        #endregion

        #region 공통 행동 로직
        protected virtual void Move()
        {
            if (waypointIndex >= waypoints.Count)
            {
                ReachEnd();
                return;
            }
            Vector3 target = waypoints[waypointIndex];
            Vector3 direction = (target - transform.position).normalized;
            float step = data.moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            UpdateAnimation(direction);
            if (Vector3.Distance(transform.position, target) < 0.05f)
            {
                waypointIndex++;
            }
        }

        public virtual void TakeDamage(int damage)
        {
            if (!isAlive) return;
            currentHp -= damage;
            if (currentHp <= 0)
            {
                currentHp = 0;
                Die();
            }
        }

        protected virtual void Die()
        {
            isAlive = false;
            OnDeath();
            ReturnToPool();
        }

        protected virtual void ReachEnd()
        {
            isAlive = false;
            OnReachEnd();
            ReturnToPool();
        }

        protected virtual void OnDeath()
        {
            OnEnemyDied?.Invoke(data);
        }

        protected virtual void OnReachEnd()
        {
            OnEnemyReachedEnd?.Invoke(data);
        }
        #endregion

        #region 애니메이션
        protected void UpdateAnimation(Vector3 direction)
        {
            if (animator == null) return;
            animator.SetFloat(DIRX, direction.x);
            animator.SetFloat(DIRY, direction.y);
        }
        #endregion

        #region ObjectPool관련
        private void ReturnToPool()
        {
            if (pool != null)
            {
                pool.Return(this, poolKey);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void SetPool(EnemyPool mPool, string mPoolKey)
        {
            pool = mPool;
            poolKey = mPoolKey;
        }
        #endregion
    }
}