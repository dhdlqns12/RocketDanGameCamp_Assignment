using UnityEngine;
using StarDefense.Enemy;

namespace StarDefense.Hero
{
    /// <summary>
    /// 범위(스플래시) 공격 전략
    /// </summary>
    public class SplashAttackStrategy : IAttackStrategy
    {
        private float splashRadius;

        public SplashAttackStrategy(float mSplashRadius)
        {
            splashRadius = mSplashRadius;
        }

        public void Execute(Vector3 spawnPos, Vector3 targetPos, int damage, EnemyBase target, Sprite projectileSprite, ProjectilePool pool)
        {
            Projectile proj = pool.Get(spawnPos);
            if (proj != null)
            {
                proj.Init(targetPos, damage, target, projectileSprite, splashRadius);
            }
        }
    }
}
