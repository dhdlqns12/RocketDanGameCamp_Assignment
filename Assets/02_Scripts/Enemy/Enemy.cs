using System.Collections.Generic;
using UnityEngine;
using StarDefense.Data;

namespace StarDefense.Enemy
{
    /// <summary>
    /// 웨이포인트 이동, 체력, 사망 처리 등 공통 로직
    /// 하위 클래스(GroundEnemy, AirEnemy, Boss 등등)에서 구체 구현
    /// </summary>
    public abstract class Enemy : MonoBehaviour, IEnemy
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;

        protected EnemyData data;
        protected List<Vector3> waypoints;
        protected int currentHp;
        protected int waypointIndex;
        protected bool isAlive;

        public int CurrentHp => currentHp;
        public bool IsAlive => isAlive;
        public abstract bool IsAir { get; }
        public Transform Transform => transform;
        public EnemyData Data => data;

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

        protected virtual void Update()
        {
            if (!isAlive || waypoints == null) return;
            Move();
        }

        protected virtual void Move()
        {
            if (waypointIndex >= waypoints.Count)
            {
                ReachEnd();
                return;
            }

            Vector3 target = waypoints[waypointIndex];
            float step = data.moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);

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
            gameObject.SetActive(false);
        }

        protected virtual void ReachEnd()
        {
            isAlive = false;
            OnReachEnd();
            gameObject.SetActive(false);
        }

        protected virtual void OnDeath()
        {
            // TODO: 사망 이벤트 발행(재화 보상 등)
        }

        protected virtual void OnReachEnd()
        {
            // TODO: 지휘관 데미지 이벤트 발행
        }
    }
}