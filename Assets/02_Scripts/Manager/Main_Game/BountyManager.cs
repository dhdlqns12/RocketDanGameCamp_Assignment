using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using StarDefense.Data;
using StarDefense.Enemy;

namespace StarDefense.Managers
{
    /// <summary>
    /// 현상금 시스템. 패널에서 적 선택 → 스폰 + 전체 쿨타임
    /// isBounty인 적을 자동 수집
    /// </summary>
    public class BountyManager : MonoBehaviour
    {
        [Header("쿨타임")]
        [SerializeField] private float cooldownTime;

        private MapManager mapManager;
        private EnemyPool enemyPool;
        private List<EnemyData> bountyEnemies = new List<EnemyData>();
        private float cooldownTimer;
        private bool isReady = true;
        private int lastTickSecond = -1;

        public List<EnemyData> BountyEnemies => bountyEnemies;
        public float CooldownTime => cooldownTime;
        public float CooldownTimer => cooldownTimer;
        public bool IsReady => isReady;

        /// <summary>
        /// 쿨타임 상태 변경 시 발행
        /// </summary>
        public event Action<bool> OnCooldownStateChanged;

        /// <summary>
        /// 1초마다 발행
        /// </summary>
        public event Action<int, int> OnCooldownTick;

        #region 유니티 Event
        private void Update()
        {
            if (isReady) return;

            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                cooldownTimer = 0f;
                isReady = true;
                lastTickSecond = -1;

                OnCooldownStateChanged?.Invoke(true);
                return;
            }

            int currentSecond = Mathf.FloorToInt(cooldownTimer);

            if (currentSecond != lastTickSecond)
            {
                lastTickSecond = currentSecond;

                int minutes = Mathf.FloorToInt(cooldownTimer / 60f);
                int seconds = Mathf.FloorToInt(cooldownTimer % 60f);

                OnCooldownTick?.Invoke(minutes, seconds);
            }
        }
        #endregion

        #region 초기화
        public void Init(MapManager mMapManager, EnemyPool mEnemyPool)
        {
            mapManager = mMapManager;
            enemyPool = mEnemyPool;

            LoadBountyEnemies();
            RegisterBountyPrefabs();

            isReady = true;
            cooldownTimer = 0f;
        }

        private void LoadBountyEnemies()
        {
            bountyEnemies.Clear();

            TextAsset jsonFile = Resources.Load<TextAsset>("EnemyData");
            EnemyData[] allEnemies = JsonConvert.DeserializeObject<EnemyData[]>(jsonFile.text);

            foreach (EnemyData data in allEnemies)
            {
                if (data.isBounty)
                {
                    bountyEnemies.Add(data);
                }
            }
        }

        private void RegisterBountyPrefabs()
        {
            foreach (EnemyData data in bountyEnemies)
            {
                GameObject prefab = Resources.Load<GameObject>($"Enemy/{data.enemyId}");

                if (prefab != null)
                {
                    enemyPool.RegisterPrefab(data.enemyId.ToString(), prefab);
                }
            }
        }
        #endregion

        #region 현상금 스폰
        public bool TrySpawnBounty(int enemyId)
        {
            if (!isReady) return false;

            EnemyData bountyData = DataManager.GetTable<EnemyData>().Get(enemyId);

            if (bountyData == null) return false;

            List<Vector3> waypoints = mapManager.GetWaypoints(0);

            if (waypoints == null) return false;

            EnemyBase enemy = enemyPool.Get(enemyId.ToString());

            if (enemy == null) return false;

            enemy.Init(bountyData, waypoints);

            isReady = false;
            cooldownTimer = cooldownTime;
            lastTickSecond = -1;

            OnCooldownStateChanged?.Invoke(false);

            return true;
        }
        #endregion
    }
}