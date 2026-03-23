using StarDefense.Core;
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
        [SerializeField] private BountyManager bountyManager;
        [SerializeField] private EnemyPool enemyPool;
        [SerializeField] private ProbeManager probeManager;

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

            // 맵
            mapManager.InitMap(stageId);

            // 카메라
            cameraController.AdjustCamera();

            // 재화
            gold = new Gold();
            gold.Init(stageData.startingGold);

            mineral = new Mineral();
            mineral.Init(stageData.startingMineral);

            // 지휘관
            SpawnCommander();

            // 이벤트 구독
            EnemyBase.OnEnemyDied += OnEnemyDied;
            EnemyBase.OnEnemyReachedEnd += OnEnemyReachedEnd;

            // 승급 매니저
            upgradeManager.Init(summonManager, projectilePool, mapManager, heroRegistryManager);

            // 소환 매니저
            summonManager.Init(projectilePool, mapManager, upgradeManager);

            // 강화 매니저
            statUpgradeManager.Init(gold, summonManager, heroRegistryManager);

            // 현상금
            bountyManager.Init(mapManager, enemyPool);

            // 탐사정
            probeManager.Init(gold, mineral);

            // 타일 입력
            tileInputHandler.Init(gold, ManagerRoot.Instance.UIManager);

            // 웨이브
            waveManager.Init(stageId);
            waveManager.StartWaveSequence();

            // UI 의존성 주입
            InitializeUI();
        }

        private void InitializeUI()
        {
            var uiManager = ManagerRoot.Instance.UIManager;

            // BottomHud (Scene)
            var bottomHud = uiManager.GetPanel<BottomHudUI>();
            if (bottomHud != null)
            {
                bottomHud.SetDependencies(bountyManager);
            }

            // 강화 패널 (Popup)
            var upgradePanel = uiManager.GetPanel<StatUpgradePanelUI>();
            if (upgradePanel != null)
            {
                upgradePanel.SetDependencies(statUpgradeManager, gold);
            }

            // 현상금 패널 (Popup)
            var bountyPanel = uiManager.GetPanel<BountyPanelUI>();
            if (bountyPanel != null)
            {
                bountyPanel.SetDependencies(bountyManager);
            }

            // 탐사정 패널 (Popup)
            var probePanel = uiManager.GetPanel<ProbePanelUI>();
            if (probePanel != null)
            {
                probePanel.SetDependencies(probeManager, gold);
            }

            // 재화 표시 (모든 Canvas 하위 CurrencyUI Init)
            CurrencyUI[] currencyUIs = uiManager.GetAllCurrencyUIs();
            foreach (CurrencyUI currencyUI in currencyUIs)
            {
                currencyUI.Init(gold, mineral, probeManager);
            }

            // 상단 HUD (Scene)
            var topHud = uiManager.GetPanel<TopHudUI>();
            if (topHud != null)
            {
                topHud.SetDependencies(stageId, waveManager);
            }
        }

        private void SpawnCommander()
        {
            Vector3 commanderPos = mapManager.CommanderWorldPosition;
            GameObject commanderObj = Instantiate(commanderPrefab, commanderPos, Quaternion.identity);
            commander = commanderObj.GetComponent<Commander>();
            commander.Init(projectilePool);
            commander.OnCommanderDead += OnGameOver;
        }
        #endregion

        #region 이벤트
        private void OnEnemyDied(EnemyData enemyData)
        {
            gold.AddGold(enemyData.goldReward);

            if (enemyData.mineralReward > 0)
            {
                mineral.AddMineral(enemyData.mineralReward);
            }
        }

        private void OnEnemyReachedEnd(EnemyData enemyData)
        {
            commander.TakeDamage(enemyData.damage);
        }

        private void OnGameOver()
        {
            Time.timeScale = 0;
        }
        #endregion
    }
}