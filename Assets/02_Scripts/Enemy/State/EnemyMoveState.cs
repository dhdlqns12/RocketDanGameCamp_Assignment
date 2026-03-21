using UnityEngine;
namespace StarDefense.Enemy.State
{
    public class EnemyMoveState : IEnemyState
    {
        public void Enter(EnemyBase enemy)
        {
        }

        public void OnStateUpdate(EnemyBase enemy)
        {
            if (enemy.WaypointIndex >= enemy.Waypoints.Count)
            {
                enemy.ChangeState(enemy.ReachEndState);
                return;
            }

            Vector3 target = enemy.Waypoints[enemy.WaypointIndex];
            Vector3 direction = (target - enemy.Transform.position).normalized;
            float step = enemy.Data.moveSpeed * Time.deltaTime;

            enemy.Transform.position = Vector3.MoveTowards(enemy.Transform.position, target, step);

            enemy.UpdateAnimation(direction);

            if (Vector3.Distance(enemy.Transform.position, target) < 0.05f)
            {
                enemy.WaypointIndex++;
            }
        }

        public void Exit(EnemyBase enemy)
        {
        }
    }
}
