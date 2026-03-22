using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using StarDefense.Data;
using StarDefense.Hero;
using StarDefense.Currency;
using StarDefense.Managers;
using StarDefense.UI;

namespace StarDefense.Map
{
    /// <summary>
    /// 타일 터치 감지
    ///MapManager 오브젝트에 같이 붙여서 사용
    /// </summary>
    [RequireComponent(typeof(MapManager))]
    public class TileInputHandler : MonoBehaviour
    {
        [SerializeField] private HeroSummonManager summonManager;
        [SerializeField] private HeroUpgradeManager upgradeManager;
        [SerializeField] private SummonUI summonUI;
        [SerializeField] private UpgradeUI upgradeUI;
        [SerializeField] private RepairUI repairUI;
        [SerializeField] private TranscendButtonUI transcendButtonUI;
        [SerializeField] private TranscendSelectUI transcendSelectUI;

        [Header("비용")]
        [SerializeField] private int repairCost = 30;
        [SerializeField] private int transcendCost = 100;

        private MapManager mapManager;
        private Gold gold;
        private Vector2Int selectedTile;
        private HeroBase selectedHero;
        private bool hasTileSelected;
        private bool hasRepairSelected;
        private bool isReady;

        private enum ActiveUI { None, Summon, Repair, Upgrade, TranscendButton, TranscendSelect }
        private ActiveUI activeUI;
        private int activeCost;

        #region 초기화
        public void Init(Gold mGold)
        {
            gold = mGold;
            gold.OnGoldChanged += OnGoldChanged;
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

            HideAllUI();

            // 영웅이 있는 타일 클릭
            HeroBase hero = FindHeroAtGrid(gridPos);

            if (hero != null)
            {
                if (upgradeManager.CanTranscend(hero))
                {
                    selectedHero = hero;
                    hasTileSelected = false;
                    hasRepairSelected = false;

                    Vector3 tileWorldPos = mapManager.GridToWorldPosition(gridPos.x, gridPos.y);
                    transcendButtonUI.Show(tileWorldPos, transcendCost, gold.CurrentGold);
                    activeUI = ActiveUI.TranscendButton;
                    activeCost = transcendCost;
                    return;
                }

                if (upgradeManager.CanUpgrade(hero))
                {
                    selectedHero = hero;
                    hasTileSelected = false;
                    hasRepairSelected = false;

                    Vector3 tileWorldPos = mapManager.GridToWorldPosition(gridPos.x, gridPos.y);
                    upgradeUI.Show(tileWorldPos);
                    activeUI = ActiveUI.Upgrade;
                    return;
                }
            }

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

        private void HideAllUI()
        {
            summonUI.Hide();
            upgradeUI.Hide();
            repairUI.Hide();
            transcendButtonUI.Hide();
            transcendSelectUI.Hide();
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

            if (!gold.SpendGold(summonManager.SummonCost))
            {
                return;
            }

            summonManager.TrySummon(selectedTile.x, selectedTile.y);

            summonUI.Hide();
            hasTileSelected = false;
            activeUI = ActiveUI.None;
        }

        public void OnUpgradeConfirmed()
        {
            if (selectedHero == null) return;

            upgradeManager.TryUpgrade(selectedHero);

            upgradeUI.Hide();
            selectedHero = null;
            activeUI = ActiveUI.None;
        }

        public void OnRepairConfirmed()
        {
            if (!hasRepairSelected) return;

            if (!gold.SpendGold(repairCost))
            {
                return;
            }

            mapManager.RepairTile(selectedTile.x, selectedTile.y);

            repairUI.Hide();
            hasRepairSelected = false;
            activeUI = ActiveUI.None;
        }

        /// <summary>
        /// 초월 버튼 클릭 → 골드 차감 → 선택지 패널 오픈
        /// </summary>
        public void OnTranscendButtonClicked()
        {
            if (selectedHero == null) return;

            if (!gold.SpendGold(transcendCost))
            {
                return;
            }

            transcendButtonUI.Hide();

            HeroData[] options = upgradeManager.GetTranscendOptions(selectedHero);
            transcendSelectUI.Show(options);
            activeUI = ActiveUI.TranscendSelect;
        }

        /// <summary>
        /// 선택지에서 유닛 선택 → TryTranscend
        /// </summary>
        public void OnTranscendConfirmed(HeroData selectedLegend)
        {
            if (selectedHero == null) return;

            upgradeManager.TryTranscend(selectedHero, selectedLegend);

            transcendSelectUI.Hide();
            selectedHero = null;
            activeUI = ActiveUI.None;
        }
        #endregion
    }
}