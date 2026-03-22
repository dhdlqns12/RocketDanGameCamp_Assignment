using UnityEngine;
using StarDefense.Data;
using StarDefense.Currency;
using StarDefense.Enemy;
using StarDefense.Hero;
using StarDefense.Map;
using StarDefense.UI;

namespace StarDefense.Managers
{
    public class StageInitManager : MonoBehaviour
    {
        [Header("Stage")]
        [SerializeField] private int stageId;

        [Header("Scene Managers")]
        [SerializeField] private MapManager mapManager;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private ProjectilePool projectilePool;
        [SerializeField] private HeroSummonManager summonManager;
        [SerializeField] private HeroUpgradeManager upgradeManager;
        [SerializeField] private TileInputHandler tileInputHandler;
        [SerializeField] private StatUpgradeManager statUpgradeManager;
        [SerializeField] private HeroRegistryManager heroRegistryManager;
        [SerializeField] private UpgradeStatUI upgradeStatUI;
        [SerializeField] private BountyManager bountyManager;
        [SerializeField] private BountyUI bountyUI;
        [SerializeField] private EnemyPool enemyPool;

        [Header("지휘관")]
        [SerializeField] private GameObject commanderPrefab;

        private Gold gold;
        private Mineral mineral;
        private Commander commander;

        public Gold Gold => gold;
        public Mineral Mineral => mineral;
        public Commander Commander => commander;

        #region 유니티 Event
        private void Start()
        {
            InitializeStage();
        }

        private void OnDestroy()
        {
            EnemyBase.OnEnemyDied -= OnEnemyDied;
            EnemyBase.OnEnemyReachedEnd -= OnEnemyReachedEnd;

            if (commander != null)
            {
                commander.OnCommanderDead -= OnGameOver;
            }
        }
        #endregion

        #region 초기화
        private void InitializeStage()
        {
            StageData stageData = DataManager.GetTable<StageData>().Get(stageId);

            // 맵 (그리드 + 타일 + 웨이포인트 + 배경)
            mapManager.InitMap(stageId);

            // 카메라 설정
            cameraController.AdjustCamera();

            // 재화 초기화
            gold = new Gold();
            gold.Init(stageData.startingGold);

            mineral = new Mineral();
            mineral.Init(stageData.startingMineral);

            // 지휘관 자동 배치 (Commander 타일 위치)
            SpawnCommander();

            // 이벤트 구독
            EnemyBase.OnEnemyDied += OnEnemyDied;
            EnemyBase.OnEnemyReachedEnd += OnEnemyReachedEnd;

            // 승급 매니저 초기화
            upgradeManager.Init(summonManager, projectilePool, mapManager, heroRegistryManager);

            // 소환 매니저 초기화
            summonManager.Init(projectilePool, mapManager, upgradeManager);

            // 강화 매니저 초기화
            statUpgradeManager.Init(gold, summonManager, heroRegistryManager);

            // 강화 UI 초기화
            upgradeStatUI.Init(statUpgradeManager, gold);

            // 타일 입력 초기화
            tileInputHandler.Init(gold);

            // 현상금 초기화
            bountyManager.Init(mapManager, enemyPool);
            bountyUI.Init(bountyManager);

            // 웨이브 (맵 경로 데이터 필요)
            waveManager.Init(stageId);

            // 웨이브 시작
            waveManager.StartWaveSequence();
        }

        /// <summary>
        /// MapManager의 Commander 위치에 지휘관 프리팹을 자동 배치
        /// </summary>
        private void SpawnCommander()
        {
            if (commanderPrefab == null) return;

            Vector3 commanderPos = mapManager.CommanderWorldPosition;

            GameObject obj = Instantiate(commanderPrefab, commanderPos, Quaternion.identity);
            commander = obj.GetComponent<Commander>();

            if (commander != null)
            {
                commander.Init(projectilePool);
                commander.OnCommanderDead += OnGameOver;
            }
        }
        #endregion

        #region 이벤트 핸들러
        /// <summary>
        /// 적 사망 시 골드 획득
        /// </summary>
        private void OnEnemyDied(EnemyData enemyData)
        {
            gold.AddGold(enemyData.goldReward);
        }

        /// <summary>
        /// 적 경로 끝 도달 시 지휘관 데미지
        /// </summary>
        private void OnEnemyReachedEnd(EnemyData enemyData)
        {
            if (commander != null)
            {
                commander.TakeDamage(enemyData.damage);
            }
        }

        /// <summary>
        /// 지휘관 사망 시 게임 오버
        /// </summary>
        private void OnGameOver()
        {
            // TODO: 게임 오버 UI 표시
            Time.timeScale = 0f;
        }
        #endregion
    }
}
