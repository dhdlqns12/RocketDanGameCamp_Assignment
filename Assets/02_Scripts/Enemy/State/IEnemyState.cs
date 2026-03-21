namespace StarDefense.Enemy.State
{
    public interface IEnemyState
    {
        void Enter(EnemyBase enemy);
        void OnStateUpdate(EnemyBase enemy);
        void Exit(EnemyBase enemy);
    }
}
