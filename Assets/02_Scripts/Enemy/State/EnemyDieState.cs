namespace StarDefense.Enemy.State
{
    public class EnemyDieState : IEnemyState
    {
        public void Enter(EnemyBase enemy)
        {
            enemy.OnDeath();
            enemy.ReturnToPool();
        }

        public void OnStateUpdate(EnemyBase enemy)
        {
        }

        public void Exit(EnemyBase enemy)
        {
        }
    }
}
