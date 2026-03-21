using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StarDefense.Core;
using StarDefense.Data;
using StarDefense.Hero;

namespace StarDefense.Managers
{
    /// <summary>
    /// 영웅 승급 매니저
    /// </summary>
    public class HeroUpgradeManager : MonoBehaviour
    {
        private List<HeroBase> spawnedHeroes = new List<HeroBase>();
        private Dictionary<int, List<HeroBase>> heroesByIdMap = new Dictionary<int, List<HeroBase>>();

        private HeroSummonManager summonManager;
        private ProjectilePool projectilePool;
        private MapManager mapManager;

        #region 초기화
        public void Init(HeroSummonManager mSummonManager, ProjectilePool mProjectilePool, MapManager mMapManager)
        {
            summonManager = mSummonManager;
            projectilePool = mProjectilePool;
            mapManager = mMapManager;
        }
        #endregion

        #region 영웅 등록/해제
        /// <summary>
        /// 소환된 영웅을 리스트에 등록 소환 시 호출
        /// </summary>
        public void RegisterHero(HeroBase hero)
        {
            spawnedHeroes.Add(hero);

            if (!heroesByIdMap.ContainsKey(hero.heroId))
            {
                heroesByIdMap[hero.heroId] = new List<HeroBase>();
            }

            heroesByIdMap[hero.heroId].Add(hero);

            UpdateUpgradeIndicators(hero.heroId);
        }

        /// <summary>
        /// 영웅을 리스트에서 해제 승급/제거 시 호출
        /// </summary>
        public void UnregisterHero(HeroBase hero)
        {
            spawnedHeroes.Remove(hero);

            if (heroesByIdMap.ContainsKey(hero.heroId))
            {
                heroesByIdMap[hero.heroId].Remove(hero);

                if (heroesByIdMap[hero.heroId].Count == 0)
                {
                    heroesByIdMap.Remove(hero.heroId);
                }
                else
                {
                    UpdateUpgradeIndicators(hero.heroId);
                }
            }
        }
        #endregion

        #region 승급 duqn
        /// <summary>
        /// 해당 영웅이 승급 가능한지 확인
        /// </summary>
        public bool CanUpgrade(HeroBase hero)
        {
            if (!hero.CanUpgrade) return false;

            if (!heroesByIdMap.ContainsKey(hero.heroId)) return false;

            return heroesByIdMap[hero.heroId].Count >= 2;
        }

        /// <summary>
        /// 같은 heroId 유닛들의 UpgradeSprite 활성/비활성 처리
        /// </summary>
        private void UpdateUpgradeIndicators(int heroId)
        {
            if (!heroesByIdMap.ContainsKey(heroId)) return;

            List<HeroBase> heroes = heroesByIdMap[heroId];
            bool canUpgrade = heroes.Count >= 2 && heroes[0].CanUpgrade;

            foreach (HeroBase hero in heroes)
            {
                hero.SetUpgradeIndicator(canUpgrade);
            }
        }
        #endregion

        #region 승급
        /// <summary>
        /// 클릭한 영웅 위치에서 승급
        /// </summary>
        public bool TryUpgrade(HeroBase clickedHero)
        {
            if (!CanUpgrade(clickedHero)) return false;

            int heroId = clickedHero.heroId;
            HeroRarity currentRarity = clickedHero.Rarity;
            HeroRarity nextRarity = currentRarity + 1;
            Vector3 upgradePos = clickedHero.Transform.position;
            Vector2Int gridPos = mapManager.WorldToGridPosition(upgradePos);

            // 같은 heroId 중 클릭한 것 제외 1기 찾아서 제거
            HeroBase sacrificeHero = heroesByIdMap[heroId].FirstOrDefault(h => h != clickedHero);

            if (sacrificeHero == null) return false;

            Vector2Int sacrificeGridPos = mapManager.WorldToGridPosition(sacrificeHero.Transform.position);

            // 희생 유닛 해제 + 타일 해제 + 파괴
            UnregisterHero(sacrificeHero);
            mapManager.SetOccupied(sacrificeGridPos.x, sacrificeGridPos.y, false);
            Destroy(sacrificeHero.gameObject);

            // 클릭 유닛 해제 + 파괴
            UnregisterHero(clickedHero);
            Destroy(clickedHero.gameObject);

            // 다음 등급 랜덤 HeroData 선택
            HeroData nextHeroData = GetRandomHeroDataByRarity(nextRarity);

            if (nextHeroData == null)
            {
                return false;
            }

            // 새 영웅 생성
            GameObject heroPrefab = summonManager.HeroPrefab;
            GameObject heroObj = Instantiate(heroPrefab, upgradePos, Quaternion.identity);
            HeroBase newHero = heroObj.GetComponent<HeroBase>();

            newHero.Init(nextHeroData, projectilePool);

            // 버프 타일이면 공속 버프 유지
            if (mapManager.IsBuffTile(gridPos.x, gridPos.y))
            {
                newHero.ApplyBuffTile(0.3f);
            }

            RegisterHero(newHero);

            return true;
        }

        /// <summary>
        /// 해당 등급의 랜덤 HeroData 반환
        /// </summary>
        private HeroData GetRandomHeroDataByRarity(HeroRarity rarity)
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("HeroData");
            HeroData[] allHeroes = Newtonsoft.Json.JsonConvert.DeserializeObject<HeroData[]>(jsonFile.text);

            List<HeroData> candidates = new List<HeroData>();

            foreach (HeroData data in allHeroes)
            {
                if (System.Enum.Parse<HeroRarity>(data.rarity) == rarity)
                {
                    candidates.Add(data);
                }
            }

            if (candidates.Count == 0) return null;

            return candidates[Random.Range(0, candidates.Count)];
        }
        #endregion
    }
}