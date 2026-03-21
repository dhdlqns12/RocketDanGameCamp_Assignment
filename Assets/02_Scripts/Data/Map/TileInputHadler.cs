using UnityEngine;
using UnityEngine.EventSystems;
using StarDefense.Hero;
using StarDefense.Managers;
using StarDefense.UI;

namespace StarDefense.Map
{
    [RequireComponent(typeof(MapManager))]
    public class TileInputHandler : MonoBehaviour
    {
        [SerializeField] private HeroSummonManager summonManager;
        [SerializeField] private HeroUpgradeManager upgradeManager;
        [SerializeField] private SummonUI summonUI;
        [SerializeField] private UpgradeUI upgradeUI;

        private MapManager mapManager;
        private Vector2Int selectedTile;
        private HeroBase selectedHero;
        private bool hasTileSelected;

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

            HeroBase hero = FindHeroAtGrid(gridPos);

            if (hero != null && upgradeManager.CanUpgrade(hero))
            {
                selectedHero = hero;
                hasTileSelected = false;

                summonUI.Hide();

                Vector3 tileWorldPos = mapManager.GridToWorldPosition(gridPos.x, gridPos.y);
                upgradeUI.Show(tileWorldPos);

                return;
            }

            if (mapManager.CanPlaceHero(gridPos.x, gridPos.y))
            {
                selectedTile = gridPos;
                hasTileSelected = true;
                selectedHero = null;

                upgradeUI.Hide();

                Vector3 tileWorldPos = mapManager.GridToWorldPosition(gridPos.x, gridPos.y);
                summonUI.Show(tileWorldPos, summonManager.SummonCost);

                return;
            }

            summonUI.Hide();
            upgradeUI.Hide();
            hasTileSelected = false;
            selectedHero = null;
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

        #region 소환/승급 확인
        /// <summary>
        /// 소환 UI에서 소환 버튼 눌렀을 때 호출
        /// </summary>
        public void OnSummonConfirmed()
        {
            if (!hasTileSelected) return;

            summonManager.TrySummon(selectedTile.x, selectedTile.y);

            summonUI.Hide();
            hasTileSelected = false;
        }

        /// <summary>
        /// 승급 UI에서 승급 버튼 눌렀을 때 호출
        /// </summary>
        public void OnUpgradeConfirmed()
        {
            if (selectedHero == null) return;

            upgradeManager.TryUpgrade(selectedHero);

            upgradeUI.Hide();
            selectedHero = null;
        }
        #endregion
    }
}