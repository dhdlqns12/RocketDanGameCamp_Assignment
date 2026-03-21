using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using StarDefense.Core;
using StarDefense.Data;
using StarDefense.Currency;
using StarDefense.Hero;
using StarDefense.Managers;

namespace StarDefense.Managers
{
    /// <summary>
    /// 영웅 소환 매니저. 데이터 드리븐 방식
    /// 범용 HeroBase 프리팹 1개 + HeroData에서 모든 정보 로드
    /// 확률 테이블 기반 등급 결정 → 해당 등급 유닛 풀에서 랜덤 소환
    /// </summary>
    public class HeroSummonManager : MonoBehaviour
    {
        [Header("소환 설정")]
        [SerializeField] private int summonCost = 20;
        [SerializeField] private GameObject heroPrefab;
        [SerializeField] private Vector2 spawnOffset = new Vector2(0f, 0.3f);

        private Dictionary<HeroRarity, List<HeroData>> heroDataByRarity = new Dictionary<HeroRarity, List<HeroData>>();
        private Dictionary<HeroRarity, float> rarityRates = new Dictionary<HeroRarity, float>();

        private Gold gold;
        private ProjectilePool projectilePool;
        private MapManager mapManager;
        private HeroUpgradeManager upgradeManager;

        public int SummonCost => summonCost;
        public GameObject HeroPrefab => heroPrefab;
        #region 초기화
        public void Init(Gold mGold, ProjectilePool mProjectilePool, MapManager mMapManager, HeroUpgradeManager mUpgradeManager)
        {
            gold = mGold;
            projectilePool = mProjectilePool;
            mapManager = mMapManager;
            upgradeManager = mUpgradeManager;

            InitRarityRates();
            ClassifyHeroData();

            Debug.Log($"[HeroSummonManager] 초기화 | 소환 비용: {summonCost}g");
        }

        private void InitRarityRates()
        {
            rarityRates[HeroRarity.Common] = 9596f;
            rarityRates[HeroRarity.Rare] = 266f;
            rarityRates[HeroRarity.Epic] = 99f;
            rarityRates[HeroRarity.Unique] = 39f;
            rarityRates[HeroRarity.Legend] = 0f;
        }

        /// <summary>
        /// HeroData JSON에서 전체 로드 후 등급별 분류
        /// </summary>
        private void ClassifyHeroData()
        {
            heroDataByRarity.Clear();

            foreach (HeroRarity rarity in System.Enum.GetValues(typeof(HeroRarity)))
            {
                heroDataByRarity[rarity] = new List<HeroData>();
            }

            TextAsset jsonFile = Resources.Load<TextAsset>("HeroData");
            HeroData[] allHeroes = JsonConvert.DeserializeObject<HeroData[]>(jsonFile.text);

            foreach (HeroData data in allHeroes)
            {
                HeroRarity rarity = System.Enum.Parse<HeroRarity>(data.rarity);
                heroDataByRarity[rarity].Add(data);

                Debug.Log($"[HeroSummonManager] 등록 | {data.heroName} | {rarity}");
            }
        }
        #endregion

        #region 소환
        /// <summary>
        /// 지정 타일에 영웅 소환 시도
        /// </summary>
        public bool TrySummon(int gridX, int gridY)
        {
            if (!mapManager.CanPlaceHero(gridX, gridY))
            {
                Debug.Log("[HeroSummonManager] 소환 불가: 배치할 수 없는 타일");

                return false;
            }

            if (!gold.SpendGold(summonCost))
            {
                Debug.Log("[HeroSummonManager] 소환 불가: 골드 부족");

                return false;
            }

            HeroRarity rarity = RollRarity();
            HeroData heroData = GetRandomHeroData(rarity);

            if (heroData == null)
            {
                Debug.LogError($"[HeroSummonManager] {rarity} 등급 데이터 없음");
                gold.AddGold(summonCost);

                return false;
            }

            Vector3 worldPos = mapManager.GridToWorldPosition(gridX, gridY);
            worldPos += (Vector3)spawnOffset;
            GameObject heroObj = Instantiate(heroPrefab, worldPos, Quaternion.identity);
            HeroBase hero = heroObj.GetComponent<HeroBase>();

            hero.Init(heroData, projectilePool);

            upgradeManager.RegisterHero(hero);

            mapManager.SetOccupied(gridX, gridY, true);

            Debug.Log($"[HeroSummonManager] 소환 성공 | {rarity} | {heroData.heroName} | 위치: ({gridX},{gridY}) | 남은 골드: {gold.CurrentGold}");

            return true;
        }

        /// <summary>
        /// 확률 테이블로 등급 결정 (10000 기준)
        /// </summary>
        private HeroRarity RollRarity()
        {
            float roll = Random.Range(0f, 10000f);
            float cumulative = 0f;

            foreach (var pair in rarityRates)
            {
                cumulative += pair.Value;

                if (roll < cumulative)
                {
                    return pair.Key;
                }
            }

            return HeroRarity.Common;
        }

        /// <summary>
        /// 해당 등급 데이터에서 랜덤 1기
        /// </summary>
        private HeroData GetRandomHeroData(HeroRarity rarity)
        {
            List<HeroData> pool = heroDataByRarity[rarity];

            if (pool.Count == 0) return null;

            int index = Random.Range(0, pool.Count);
            return pool[index];
        }
        #endregion

        #region 강화 (추후)
        /// <summary>
        /// Common 확률을 줄이고 나머지 등급 확률을 올린다
        /// </summary>
        public void UpgradeRates(float commonReduction)
        {
            float reduced = Mathf.Min(commonReduction, rarityRates[HeroRarity.Common]);
            rarityRates[HeroRarity.Common] -= reduced;

            float bonus = reduced / 3f;
            rarityRates[HeroRarity.Rare] += bonus;
            rarityRates[HeroRarity.Epic] += bonus;
            rarityRates[HeroRarity.Unique] += bonus;

            Debug.Log($"[HeroSummonManager] 확률 강화 | Common: {rarityRates[HeroRarity.Common]:F0}/10000 | Rare: {rarityRates[HeroRarity.Rare]:F0}/10000 | Epic: {rarityRates[HeroRarity.Epic]:F0}/10000 | Unique: {rarityRates[HeroRarity.Unique]:F0}/10000");
        }
        #endregion
    }
}