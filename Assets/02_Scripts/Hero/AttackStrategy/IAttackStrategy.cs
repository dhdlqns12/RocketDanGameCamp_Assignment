using UnityEngine;
using StarDefense.Enemy;

namespace StarDefense.Hero
{
    /// <summary>
    /// 공격 전략 인터페이스
    /// </summary>
    public interface IAttackStrategy
    {
        void Execute(Vector3 spawnPos, Vector3 targetPos, int damage, EnemyBase target, Sprite projectileSprite, ProjectilePool pool);
    }
}
