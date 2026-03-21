using System;
using System.Collections.Generic;
using UnityEngine;
using StarDefense.Data;
using StarDefense.Enemy.State;

namespace StarDefense.Enemy
{
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

        private IEnemyState currentState;

        // 상태 캐싱
        private EnemySpawnState spawnState = new EnemySpawnState();
        private EnemyMoveState moveState = new EnemyMoveState();
        private EnemyDieState dieState = new EnemyDieState();
        private EnemyReachEndState reachEndState = new EnemyReachEndState();

        public int CurrentHp => currentHp;
        public bool IsAlive => isAlive;
        public abstract bool IsAir { get; }
        public Transform Transform => transform;
        public EnemyData Data => data;
        public List<Vector3> Waypoints => waypoints;

        public int WaypointIndex
        {
            get => waypointIndex;
            set => waypointIndex = value;
        }

        public EnemySpawnState SpawnState => spawnState;
        public EnemyMoveState MoveState => moveState;
        public EnemyDieState DieState => dieState;
        public EnemyReachEndState ReachEndState => reachEndState;

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
            if (!isAlive || currentState == null) return;

            currentState.OnStateUpdate(this);
        }
        #endregion

        #region 초기화
        public virtual void Init(EnemyData mData, List<Vector3> mWaypoints)
        {
            data = mData;
            waypoints = mWaypoints;
            currentHp = data.hp;
            isAlive = true;

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }

            ChangeState(spawnState);
        }
        #endregion

        #region FSM
        public void ChangeState(IEnemyState newState)
        {
            currentState?.Exit(this);
            currentState = newState;
            currentState.Enter(this);
        }
        #endregion

        #region 데미지
        public virtual void TakeDamage(int damage)
        {
            if (!isAlive) return;

            currentHp -= damage;

            if (currentHp <= 0)
            {
                currentHp = 0;
                isAlive = false;
                ChangeState(dieState);
            }
        }
        #endregion

        #region 이벤트 발행
        public void OnDeath()
        {
            OnEnemyDied?.Invoke(data);
        }

        public void OnReachEnd()
        {
            isAlive = false;
            OnEnemyReachedEnd?.Invoke(data);
        }
        #endregion

        #region 애니메이션
        public void UpdateAnimation(Vector3 direction)
        {
            if (animator == null) return;

            animator.SetFloat(DIRX, direction.x);
            animator.SetFloat(DIRY, direction.y);
        }
        #endregion

        #region ObjectPool관련
        public void ReturnToPool()
        {
            currentState = null;

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
