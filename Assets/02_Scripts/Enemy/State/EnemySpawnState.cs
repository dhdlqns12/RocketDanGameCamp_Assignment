namespace StarDefense.Enemy.State
{
    public class EnemySpawnState : IEnemyState
    {
        public void Enter(EnemyBase enemy)
        {
            enemy.Transform.position = enemy.Waypoints[0];
            enemy.WaypointIndex = 0;
        }

        public void OnStateUpdate(EnemyBase enemy)
        {
            enemy.ChangeState(enemy.MoveState);
        }

        public void Exit(EnemyBase enemy)
        {
        }
    }
}
