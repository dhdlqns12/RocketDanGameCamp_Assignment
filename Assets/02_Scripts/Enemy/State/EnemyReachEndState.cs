namespace StarDefense.Enemy.State
{
    public class EnemyReachEndState : IEnemyState
    {
        public void Enter(EnemyBase enemy)
        {
            enemy.OnReachEnd();
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
