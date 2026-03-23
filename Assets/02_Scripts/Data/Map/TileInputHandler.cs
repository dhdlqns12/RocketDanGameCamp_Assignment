using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using StarDefense.Core;
using StarDefense.Data;
using StarDefense.Hero;
using StarDefense.Currency;
using StarDefense.Managers;
using StarDefense.UI;

namespace StarDefense.Map
{
    /// <summary>
    /// 타일 터치 감지
    /// </summary>
    [RequireComponent(typeof(MapManager))]
    public class TileInputHandler : MonoBehaviour
    {
        [SerializeField] private HeroSummonManager summonManager;
        [SerializeField] private HeroUpgradeManager upgradeManager;

        [Header("비용")]
        [SerializeField] private int repairCost = 30;
        [SerializeField] private int transcendCost = 100;

        private MapManager mapManager;
        private UIManager uiManager;
        private Gold gold;

        private SummonUI summonUI;
        private UpgradeUI upgradeUI;
        private RepairUI repairUI;
        private TranscendButtonUI transcendButtonUI;
        private TranscendSelectUI transcendSelectUI;
        private HeroStatPopupUI heroStatPopupUI;

        private Vector2Int selectedTile;
        private HeroBase selectedHero;
        private bool hasTileSelected;
        private bool hasRepairSelected;
        private bool isReady;

        private ActiveUI activeUI;
        private int activeCost;

        #region 초기화
        public void Init(Gold mGold, UIManager mUIManager)
        {
            gold = mGold;
            uiManager = mUIManager;

            gold.OnGoldChanged += OnGoldChanged;

            // UIManager에서 패널 참조 가져오기
            summonUI = uiManager.GetPanel<SummonUI>();
            upgradeUI = uiManager.GetPanel<UpgradeUI>();
            repairUI = uiManager.GetPanel<RepairUI>();
            transcendButtonUI = uiManager.GetPanel<TranscendButtonUI>();
            transcendSelectUI = uiManager.GetPanel<TranscendSelectUI>();
            heroStatPopupUI = uiManager.GetPanel<HeroStatPopupUI>();

            // TilePopup UI들에 자신 주입
            summonUI.SetTileInputHandler(this);
            upgradeUI.SetTileInputHandler(this);
            repairUI.SetTileInputHandler(this);
            transcendButtonUI.SetTileInputHandler(this);
            transcendSelectUI.SetTileInputHandler(this);

            isReady = true;
        }

        private void OnDestroy()
        {
            if (gold != null)
            {
                gold.OnGoldChanged -= OnGoldChanged;
            }
        }
        #endregion

        #region 유니티 Event
        private void Awake()
        {
            mapManager = GetComponent<MapManager>();
        }

        private void Update()
        {
            if (!isReady) return;

            if (!Input.GetMouseButtonDown(0)) return;

            if (IsPointerOverUI()) return;

            HandleClick();
        }

        private bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Count > 0;
        }
        #endregion

        #region 클릭 처리
        private void HandleClick()
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;

            Vector2Int gridPos = mapManager.WorldToGridPosition(worldPos);

            HideAllTilePopups();

            HeroBase hero = FindHeroAtGrid(gridPos);

            if (hero != null)
            {
                Vector3 tileWorldPos = mapManager.GridToWorldPosition(gridPos.x, gridPos.y);

                // 영웅 클릭 시 항상 스탯 표시
                heroStatPopupUI.Show(hero);

                // 1. Unique → 초월 버튼
                if (upgradeManager.CanTranscend(hero))
                {
                    selectedHero = hero;
                    hasTileSelected = false;
                    hasRepairSelected = false;

                    transcendButtonUI.Show(tileWorldPos, transcendCost, gold.CurrentGold);
                    activeUI = ActiveUI.TranscendButton;
                    activeCost = transcendCost;
                    return;
                }

                // 2. 승급 가능 → 승급
                if (upgradeManager.CanUpgrade(hero))
                {
                    selectedHero = hero;
                    hasTileSelected = false;
                    hasRepairSelected = false;

                    upgradeUI.Show(tileWorldPos);
                    activeUI = ActiveUI.Upgrade;
                    return;
                }

                // 3. 액션 없는 영웅 → 스탯만 표시
                return;
            }

            // 3. FixBlock → 수리
            int tileType = mapManager.GetTileType(gridPos.x, gridPos.y);

            if (tileType == 2)
            {
                selectedTile = gridPos;
                hasRepairSelected = true;
                hasTileSelected = false;
                selectedHero = null;

                Vector3 tileWorldPos = mapManager.GridToWorldPosition(gridPos.x, gridPos.y);
                repairUI.Show(tileWorldPos, repairCost, gold.CurrentGold);
                activeUI = ActiveUI.Repair;
                activeCost = repairCost;
                return;
            }

            // 4. 배치 가능 → 소환
            if (mapManager.CanPlaceHero(gridPos.x, gridPos.y))
            {
                selectedTile = gridPos;
                hasTileSelected = true;
                hasRepairSelected = false;
                selectedHero = null;

                Vector3 tileWorldPos = mapManager.GridToWorldPosition(gridPos.x, gridPos.y);
                summonUI.Show(tileWorldPos, summonManager.SummonCost, gold.CurrentGold);
                activeUI = ActiveUI.Summon;
                activeCost = summonManager.SummonCost;
                return;
            }

            // 그 외
            hasTileSelected = false;
            hasRepairSelected = false;
            selectedHero = null;
        }

        private void HideAllTilePopups()
        {
            uiManager.CloseAllTilePopups();
            heroStatPopupUI.Close();
            activeUI = ActiveUI.None;
            activeCost = 0;
        }

        private void OnGoldChanged(int currentGold, int delta)
        {
            switch (activeUI)
            {
                case ActiveUI.Summon:
                    summonUI.UpdatePriceColor(currentGold, activeCost);
                    break;
                case ActiveUI.Repair:
                    repairUI.UpdatePriceColor(currentGold, activeCost);
                    break;
                case ActiveUI.TranscendButton:
                    transcendButtonUI.UpdatePriceColor(currentGold, activeCost);
                    break;
            }
        }

        private HeroBase FindHeroAtGrid(Vector2Int gridPos)
        {
            Vector3 worldPos = mapManager.GridToWorldPosition(gridPos.x, gridPos.y);
            float threshold = mapManager.CellSize.x * 0.5f;

            HeroBase[] heroes = FindObjectsByType<HeroBase>(FindObjectsSortMode.None);

            foreach (HeroBase hero in heroes)
            {
                if (Vector3.Distance(hero.Transform.position, worldPos) < threshold)
                {
                    return hero;
                }
            }

            return null;
        }
        #endregion

        #region 소환/승급/수리/초월
        public void OnSummonConfirmed()
        {
            if (!hasTileSelected) return;

            if (!gold.SpendGold(summonManager.SummonCost)) return;

            summonManager.TrySummon(selectedTile.x, selectedTile.y);

            uiManager.ClosePanel<SummonUI>();
            hasTileSelected = false;
            activeUI = ActiveUI.None;
        }

        public void OnUpgradeConfirmed()
        {
            if (selectedHero == null) return;

            upgradeManager.TryUpgrade(selectedHero);

            uiManager.ClosePanel<UpgradeUI>();
            selectedHero = null;
            activeUI = ActiveUI.None;
        }

        public void OnRepairConfirmed()
        {
            if (!hasRepairSelected) return;

            if (!gold.SpendGold(repairCost)) return;

            mapManager.RepairTile(selectedTile.x, selectedTile.y);

            uiManager.ClosePanel<RepairUI>();
            hasRepairSelected = false;
            activeUI = ActiveUI.None;
        }

        public void OnTranscendButtonClicked()
        {
            if (selectedHero == null) return;

            if (!gold.SpendGold(transcendCost)) return;

            uiManager.ClosePanel<TranscendButtonUI>();

            HeroData[] options = upgradeManager.GetTranscendOptions(selectedHero);
            transcendSelectUI.Show(options);
            activeUI = ActiveUI.TranscendSelect;
        }

        public void OnTranscendConfirmed(HeroData selectedLegend)
        {
            if (selectedHero == null) return;

            upgradeManager.TryTranscend(selectedHero, selectedLegend);

            uiManager.ClosePanel<TranscendSelectUI>();
            selectedHero = null;
            activeUI = ActiveUI.None;
        }
        #endregion
    }
}