using UnityEngine;
using StarDefense.Enemy;

namespace StarDefense.Hero
{
    /// <summary>
    /// 단일 타겟 투사체 공격 전략
    /// </summary>
    public class SingleAttackStrategy : IAttackStrategy
    {
        public void Execute(Vector3 spawnPos, Vector3 targetPos, int damage, EnemyBase target, Sprite projectileSprite, ProjectilePool pool)
        {
            Projectile projectile = pool.Get(spawnPos);
            if (projectile != null)
            {
                projectile.Initialize(targetPos, damage, target, projectileSprite);
            }
        }
    }
}
