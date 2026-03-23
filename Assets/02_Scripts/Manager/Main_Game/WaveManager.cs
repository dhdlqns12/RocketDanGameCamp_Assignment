using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarDefense.Data;
using StarDefense.Enemy;
using System;

namespace StarDefense.Managers
{
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

        public event Action<int, int> OnWaveChanged;

        #region 초기화
        public void Init(int mStageId)
        {
            waveData = DataManager.GetTable<WaveData>().Get(mStageId);

            if (waveData == null)
            {
                Debug.LogError($"웨이브 데이터 찾을 수 없음: {mStageId}");
                return;
            }

            currentWaveIndex = 0;
            isWaveActive = false;

            RegisterEnemyPrefabs();
        }
        #endregion

        #region 웨이브 시작 관리
        /// <summary>
        /// 웨이브 자동 진행을 시작
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
                OnWaveChanged?.Invoke(CurrentWave, TotalWaves);
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

            // 각 phase 스폰 시작
            List<Coroutine> spawnCoroutines = new List<Coroutine>();

            foreach (WavePhase phase in waveData.phases)
            {
                if (waveNumber < phase.fromWave || waveNumber > phase.toWave) continue;

                int count = CalculateCount(phase, waveNumber);
                spawnCoroutines.Add(StartCoroutine(SpawnPhaseCoroutine(phase, count)));
            }

            // 모든 스폰 코루틴 완료 대기
            foreach (Coroutine co in spawnCoroutines)
            {
                yield return co;
            }

            isWaveActive = false;
        }

        private int CalculateCount(WavePhase phase, int waveNumber)
        {
            if (phase.fromWave == phase.toWave)
                return phase.startCount;

            float t = (float)(waveNumber - phase.fromWave) / (phase.toWave - phase.fromWave);
            return Mathf.RoundToInt(Mathf.Lerp(phase.startCount, phase.endCount, t));
        }
        #endregion

        #region 적 소환
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

        private void OnAllWavesComplete()
        {
            Debug.Log("모든 웨이브 클리어, 승리!");
            // TODO: 승리 이벤트 발행
        }

        private void RegisterEnemyPrefabs()
        {
            HashSet<int> registeredIds = new HashSet<int>();

            foreach (WavePhase phase in waveData.phases)
            {
                if (registeredIds.Contains(phase.enemyId)) 
                    continue;

                GameObject prefab = Resources.Load<GameObject>($"Enemy/{phase.enemyId}");

                if (prefab == null)
                {
                    Debug.LogError($"적 프리팹 찾을 수 없음: Enemy/{phase.enemyId}");
                    continue;
                }

                enemyPool.RegisterPrefab(phase.enemyId.ToString(), prefab);
                registeredIds.Add(phase.enemyId);
            }
        }
        #endregion
    }
}