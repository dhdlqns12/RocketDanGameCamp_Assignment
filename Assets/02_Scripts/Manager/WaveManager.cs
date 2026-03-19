using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarDefense.Data;
using StarDefense.Enemy;

namespace StarDefense.Managers
{
    /// <summary>
    /// 웨이브 진행 및 적 스폰 관리
    /// WaveData의 phases에서 필요한 적 프리팹을 자동 로드한
    /// 프리팹 경로: Resources/Enemy/{enemyId}
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MapManager mapManager;
        [SerializeField] private EnemyPool enemyPool;

        private WaveData waveData;
        private int currentWaveIndex;
        private int aliveEnemyCount;
        private bool isWaveActive;

        public int CurrentWave => currentWaveIndex + 1;
        public int TotalWaves => waveData != null ? waveData.waveCount : 0;
        public bool IsWaveActive => isWaveActive;

        public void Initialize(int mStageId)
        {
            waveData = DataManager.GetTable<WaveData>().Get(mStageId);

            if (waveData == null)
            {
                Debug.LogError($"웨이브 데이터 없음: {mStageId}");
                return;
            }

            currentWaveIndex = 0;
            isWaveActive = false;

            RegisterEnemyPrefabs();
        }

        /// <summary>
        /// phases에서 사용하는 enemyId를 수집하여 프리팹을 Resources에서 자동 로드.
        /// 프리팹 경로: Resources/Enemy/{enemyId}
        /// </summary>
        private void RegisterEnemyPrefabs()
        {
            HashSet<int> registeredIds = new HashSet<int>();

            foreach (WavePhase phase in waveData.phases)
            {
                if (registeredIds.Contains(phase.enemyId)) continue;

                GameObject prefab = Resources.Load<GameObject>($"Enemy/{phase.enemyId}");

                if (prefab == null)
                {
                    Debug.LogError($"적 프리팹 없음/{phase.enemyId}");
                    continue;
                }

                enemyPool.RegisterPrefab(phase.enemyId.ToString(), prefab);
                registeredIds.Add(phase.enemyId);
            }
        }

        public void StartNextWave()
        {
            if (isWaveActive) return;
            if (currentWaveIndex >= TotalWaves)
            {
                return;
            }

            isWaveActive = true;
            aliveEnemyCount = 0;
            int waveNumber = currentWaveIndex + 1;

            foreach (WavePhase phase in waveData.phases)
            {
                if (waveNumber < phase.fromWave || waveNumber > phase.toWave) continue;

                int count = CalculateCount(phase, waveNumber);
                aliveEnemyCount += count;

                StartCoroutine(SpawnPhaseCoroutine(phase, count));
            }
        }

        private int CalculateCount(WavePhase phase, int waveNumber)
        {
            if (phase.fromWave == phase.toWave)
                return phase.startCount;

            float t = (float)(waveNumber - phase.fromWave) / (phase.toWave - phase.fromWave);
            return Mathf.RoundToInt(Mathf.Lerp(phase.startCount, phase.endCount, t));
        }

        private IEnumerator SpawnPhaseCoroutine(WavePhase phase, int count)
        {
            if (phase.delay > 0)
            {
                yield return new WaitForSeconds(phase.delay);
            }

            EnemyData enemyData = DataManager.GetTable<EnemyData>().Get(phase.enemyId);
            List<Vector3> waypoints = mapManager.GetWaypoints(phase.pathId);

            if (enemyData == null || waypoints == null)
            {
                Debug.LogError($"잘못된 Phase: enemyId={phase.enemyId}, pathId={phase.pathId}");
                yield break;
            }

            for (int i = 0; i < count; i++)
            {
                SpawnEnemy(enemyData, waypoints, phase.enemyId);

                if (i < count - 1)
                {
                    yield return new WaitForSeconds(waveData.spawnInterval);
                }
            }
        }

        private void SpawnEnemy(EnemyData enemyData, List<Vector3> waypoints, int enemyId)
        {
            Enemy.Enemy enemy = enemyPool.Get(enemyId.ToString());
            if (enemy == null) return;

            enemy.Initialize(enemyData, waypoints);
        }

        public void OnEnemyRemoved()
        {
            aliveEnemyCount--;

            if (aliveEnemyCount <= 0)
            {
                isWaveActive = false;
                currentWaveIndex++;
                OnWaveComplete();
            }
        }

        private void OnWaveComplete()
        {
            if (currentWaveIndex >= TotalWaves)
            {
                Debug.Log("모든 웨이브 클리어 승리!");
            }
            else
            {
                Debug.Log($"현재 웨이브: {currentWaveIndex}/전체 웨이브: {TotalWaves}");
            }
        }
    }
}