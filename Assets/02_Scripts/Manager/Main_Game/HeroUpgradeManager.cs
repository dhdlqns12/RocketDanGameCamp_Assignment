using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StarDefense.Core;
using StarDefense.Data;
using StarDefense.Hero;

namespace StarDefense.Managers
{
    /// <summary>
    /// 영웅 승급/초월 매니저
    /// HeroRegistry 인스턴스 참조
    /// </summary>
    public class HeroUpgradeManager : MonoBehaviour
    {
        private HeroSummonManager summonManager;
        private ProjectilePool projectilePool;
        private MapManager mapManager;
        private HeroRegistryManager heroRegistryManager;

        #region 초기화
        public void Init(HeroSummonManager mSummonManager, ProjectilePool mProjectilePool, MapManager mMapManager, HeroRegistryManager mHeroRegistryManager)
        {
            summonManager = mSummonManager;
            projectilePool = mProjectilePool;
            mapManager = mMapManager;
            heroRegistryManager = mHeroRegistryManager;
        }
        #endregion

        #region 영웅 등록/해제
        public void RegisterHero(HeroBase hero)
        {
            heroRegistryManager.Register(hero);

            UpdateUpgradeIndicators(hero.heroId);
            UpdateTranscendIndicator(hero);
        }

        public void UnregisterHero(HeroBase hero)
        {
            int heroId = hero.heroId;

            heroRegistryManager.Unregister(hero);

            if (heroRegistryManager.CountById(heroId) > 0)
            {
                UpdateUpgradeIndicators(heroId);
            }
        }
        #endregion

        #region 승급 가능 확인
        public bool CanUpgrade(HeroBase hero)
        {
            if (!hero.CanUpgrade) return false;

            return heroRegistryManager.CountById(hero.heroId) >= 2;
        }

        private void UpdateUpgradeIndicators(int heroId)
        {
            List<HeroBase> heroes = heroRegistryManager.GetHeroesById(heroId);
            bool canUpgrade = heroes.Count >= 2 && heroes[0].CanUpgrade;

            foreach (HeroBase hero in heroes)
            {
                // 초월 가능하면 승급 표시 끄기
                if (hero.CanTranscend)
                {
                    hero.SetUpgradeIndicator(false);
                }
                else
                {
                    hero.SetUpgradeIndicator(canUpgrade);
                }
            }
        }
        #endregion

        #region 승급 실행
        public bool TryUpgrade(HeroBase clickedHero)
        {
            if (!CanUpgrade(clickedHero)) return false;

            int heroId = clickedHero.heroId;
            HeroRarity currentRarity = clickedHero.Rarity;
            HeroRarity nextRarity = currentRarity + 1;
            Vector3 upgradePos = clickedHero.Transform.position;
            Vector2Int gridPos = mapManager.WorldToGridPosition(upgradePos);

            List<HeroBase> sameHeroes = heroRegistryManager.GetHeroesById(heroId);
            HeroBase sacrificeHero = sameHeroes.FirstOrDefault(h => h != clickedHero);

            if (sacrificeHero == null) return false;

            Vector2Int sacrificeGridPos = mapManager.WorldToGridPosition(sacrificeHero.Transform.position);

            UnregisterHero(sacrificeHero);
            mapManager.SetOccupied(sacrificeGridPos.x, sacrificeGridPos.y, false);
            Destroy(sacrificeHero.gameObject);

            UnregisterHero(clickedHero);
            Destroy(clickedHero.gameObject);

            HeroData nextHeroData = GetRandomHeroDataByRarity(nextRarity);

            if (nextHeroData == null)
            {
                Debug.LogError($"{nextRarity} 등급 데이터 없음");
                return false;
            }

            GameObject heroPrefab = summonManager.HeroPrefab;
            GameObject heroObj = Instantiate(heroPrefab, upgradePos, Quaternion.identity);
            HeroBase newHero = heroObj.GetComponent<HeroBase>();

            newHero.Init(nextHeroData, projectilePool);

            if (mapManager.IsBuffTile(gridPos.x, gridPos.y))
            {
                newHero.ApplyBuffTile(0.3f);
            }

            RegisterHero(newHero);

            return true;
        }

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

        #region 초월
        private void UpdateTranscendIndicator(HeroBase hero)
        {
            hero.SetTranscendIndicator(hero.CanTranscend);
        }

        public bool CanTranscend(HeroBase hero)
        {
            if (!hero.CanTranscend) return false;

            HeroData heroData = DataManager.GetTable<HeroData>().Get(hero.heroId);

            return heroData != null && heroData.transcendOptions != null && heroData.transcendOptions.Length > 0;
        }

        public HeroData[] GetTranscendOptions(HeroBase hero)
        {
            HeroData heroData = DataManager.GetTable<HeroData>().Get(hero.heroId);

            if (heroData == null || heroData.transcendOptions == null) return null;

            HeroData[] options = new HeroData[heroData.transcendOptions.Length];

            for (int i = 0; i < heroData.transcendOptions.Length; i++)
            {
                options[i] = DataManager.GetTable<HeroData>().Get(heroData.transcendOptions[i]);
            }

            return options;
        }

        public bool TryTranscend(HeroBase hero, HeroData selectedLegend)
        {
            if (!CanTranscend(hero)) return false;

            Vector3 pos = hero.Transform.position;
            Vector2Int gridPos = mapManager.WorldToGridPosition(pos);

            UnregisterHero(hero);
            Destroy(hero.gameObject);

            GameObject heroPrefab = summonManager.HeroPrefab;
            GameObject heroObj = Instantiate(heroPrefab, pos, Quaternion.identity);
            HeroBase newHero = heroObj.GetComponent<HeroBase>();

            newHero.Init(selectedLegend, projectilePool);

            if (mapManager.IsBuffTile(gridPos.x, gridPos.y))
            {
                newHero.ApplyBuffTile(0.3f);
            }

            RegisterHero(newHero);

            return true;
        }
        #endregion
    }
}