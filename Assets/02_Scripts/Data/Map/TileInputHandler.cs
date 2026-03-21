using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] private SummonUI summonUI;
        [SerializeField] private UpgradeUI upgradeUI;
        [SerializeField] private RepairUI repairUI;

        [Header("수리 비용")]
        [SerializeField] private int repairCost = 30;

        private MapManager mapManager;
        private Gold gold;
        private Vector2Int selectedTile;
        private HeroBase selectedHero;
        private bool hasTileSelected;
        private bool hasRepairSelected;

        #region 초기화
        public void Init(Gold mGold)
        {
            gold = mGold;
        }
        #endregion

        #region 유니티 Event
        private void Awake()
        {
            mapManager = GetComponent<MapManager>();
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            HandleClick();
        }
        #endregion

        #region 클릭 처리
        private void HandleClick()
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;

            Vector2Int gridPos = mapManager.WorldToGridPosition(worldPos);

            HideAllUI();

            HeroBase hero = FindHeroAtGrid(gridPos);

            if (hero != null && upgradeManager.CanUpgrade(hero))
            {
                selectedHero = hero;
                hasTileSelected = false;
                hasRepairSelected = false;

                Vector3 tileWorldPos = mapManager.GridToWorldPosition(gridPos.x, gridPos.y);
                upgradeUI.Show(tileWorldPos);
                return;
            }

            int tileType = mapManager.GetTileType(gridPos.x, gridPos.y);

            if (tileType == 2)
            {
                selectedTile = gridPos;
                hasRepairSelected = true;
                hasTileSelected = false;
                selectedHero = null;

                Vector3 tileWorldPos = mapManager.GridToWorldPosition(gridPos.x, gridPos.y);
                repairUI.Show(tileWorldPos, repairCost);
                return;
            }

            if (mapManager.CanPlaceHero(gridPos.x, gridPos.y))
            {
                selectedTile = gridPos;
                hasTileSelected = true;
                hasRepairSelected = false;
                selectedHero = null;

                Vector3 tileWorldPos = mapManager.GridToWorldPosition(gridPos.x, gridPos.y);
                summonUI.Show(tileWorldPos, summonManager.SummonCost);
                return;
            }

            hasTileSelected = false;
            hasRepairSelected = false;
            selectedHero = null;
        }

        private void HideAllUI()
        {
            summonUI.Hide();
            upgradeUI.Hide();
            repairUI.Hide();
        }

        /// <summary>
        /// 그리드 위치에 있는 영웅 찾기
        /// </summary>
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

        #region 소환/승급/수리 확인
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
        }

        public void OnUpgradeConfirmed()
        {
            if (selectedHero == null) return;

            upgradeManager.TryUpgrade(selectedHero);

            upgradeUI.Hide();
            selectedHero = null;
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
        }
        #endregion
    }
}