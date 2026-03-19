using UnityEngine;

namespace StarDefense.Enemy
{
    /// <summary>
    /// 적 공통 인터페이스 영웅 공격 시스템에서 타겟 타입으로 사용
    /// </summary>
    public interface IEnemy
    {
        public int CurrentHp { get; }
        public bool IsAlive { get; }
        public bool IsAir { get; }
        public Transform Transform { get; }

        public void TakeDamage(int damage);
        public void Initialize(Data.EnemyData data, System.Collections.Generic.List<Vector3> waypoints);
    }
}
