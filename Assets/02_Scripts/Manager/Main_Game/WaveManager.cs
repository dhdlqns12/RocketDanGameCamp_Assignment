using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarDefense.Data;
using StarDefense.Enemy;

namespace StarDefense.Managers
{
    /// <summary>
    /// 웨이브 진행 및 적 스폰 관리
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MapManager mapManager;
        [SerializeField] private EnemyPool enemyPool;

        [Header("Wave Settings")]
        [SerializeField] private float firstWaveDelay = 10f;
        [SerializeField] private float betweenWaveDelay = 10f;

        private WaveData waveData;
        private int currentWaveIndex;
        private bool isWaveActive;

        public int CurrentWave => currentWaveIndex + 1;
        public int TotalWaves => waveData != null ? waveData.waveCount : 0;
        public bool IsWaveActive => isWaveActive;
        public float FirstWaveDelay => firstWaveDelay;
        public float BetweenWaveDelay => betweenWaveDelay;

        /// <summary>
        /// 웨이브 변경 시 발행
        /// </summary>
        public event System.Action<int, int> OnWaveChanged;

        /// <summary>
        /// 모든 웨이브 클리어 시 발행
        /// </summary>
        public event System.Action OnAllWavesCleared;

        #region 초기화
        public void Init(int mStageId)
        {
            waveData = DataManager.GetTable<WaveData>().Get(mStageId);

            if (waveData == null)
            {

                return;
            }

            currentWaveIndex = 0;
            isWaveActive = false;

            RegisterEnemyPrefabs();
        }

        /// <summary>
        /// phases에서 사용하는 enemyId를 수집하여 프리팹을 Resources에서 자동 로드
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
                    Debug.LogError($"[WaveManager] Enemy prefab not found: Enemy/{phase.enemyId}");
                    continue;
                }

                enemyPool.RegisterPrefab(phase.enemyId.ToString(), prefab);
                registeredIds.Add(phase.enemyId);
            }
        }
        #endregion

        #region 웨이브 진행
        /// <summary>
        /// 웨이브 자동 진행 시작
        /// </summary>
        public void StartWaveSequence()
        {
            StartCoroutine(WaveSequenceCoroutine());
        }

        private IEnumerator WaveSequenceCoroutine()
        {
            yield return new WaitForSeconds(firstWaveDelay);

            while (currentWaveIndex < TotalWaves)
            {
                yield return StartCoroutine(RunWaveCoroutine());

                currentWaveIndex++;

                if (currentWaveIndex >= TotalWaves)
                {
                    OnAllWavesComplete();
                    yield break;
                }

                yield return new WaitForSeconds(betweenWaveDelay);
            }
        }

        private IEnumerator RunWaveCoroutine()
        {
            isWaveActive = true;
            int waveNumber = currentWaveIndex + 1;

            OnWaveChanged?.Invoke(CurrentWave, TotalWaves);

            List<Coroutine> spawnCoroutines = new List<Coroutine>();

            foreach (WavePhase phase in waveData.phases)
            {
                if (waveNumber < phase.fromWave || waveNumber > phase.toWave) continue;

                int count = CalculateCount(phase, waveNumber);
                spawnCoroutines.Add(StartCoroutine(SpawnPhaseCoroutine(phase, count)));
            }

            foreach (Coroutine co in spawnCoroutines)
            {
                yield return co;
            }

            isWaveActive = false;
        }
        #endregion

        #region 스폰
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
            EnemyBase enemy = enemyPool.Get(enemyId.ToString());

            if (enemy == null) return;

            enemy.Init(enemyData, waypoints);
        }
        #endregion

        #region 완료
        private void OnAllWavesComplete()
        {
            OnAllWavesCleared?.Invoke();
        }
        #endregion
    }
}