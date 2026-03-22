using System;
using System.Collections.Generic;
using UnityEngine;
using StarDefense.Core;
using StarDefense.Currency;
using StarDefense.Hero;

namespace StarDefense.Managers
{
    /// <summary>
    /// 강화 시스템. HeroRegistry 인스턴스 참조
    /// 등급별 능력치 강화 + 뽑기 확률 강화
    /// </summary>
    public class StatUpgradeManager : MonoBehaviour
    {
        [Header("강화 설정")]
        [SerializeField] private int baseCost = 50;
        [SerializeField] private float costIncreaseRate = 0.1f;
        [SerializeField] private float statBonusPerLevel = 0.1f;

        private Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();
        private Dictionary<UpgradeType, float> statBonuses = new Dictionary<UpgradeType, float>();

        private Gold gold;
        private HeroSummonManager summonManager;
        private HeroRegistryManager heroRegistry;

        public event Action<UpgradeType, int, int> OnUpgradeLevelChanged;

        #region 초기화
        public void Init(Gold mGold, HeroSummonManager mSummonManager, HeroRegistryManager mHeroRegistry)
        {
            gold = mGold;
            summonManager = mSummonManager;
            heroRegistry = mHeroRegistry;

            foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
            {
                upgradeLevels[type] = 0;
                statBonuses[type] = 0f;
            }

            heroRegistry.OnHeroRegistered += OnHeroRegistered;
        }

        private void OnDestroy()
        {
            if (heroRegistry != null)
            {
                heroRegistry.OnHeroRegistered -= OnHeroRegistered;
            }
        }
        #endregion

        #region 강화 실행
        public bool TryUpgrade(UpgradeType type)
        {
            int cost = GetUpgradeCost(type);

            if (!gold.SpendGold(cost))
            {
                return false;
            }

            upgradeLevels[type]++;
            statBonuses[type] += statBonusPerLevel;

            ApplyUpgrade(type);

            int nextCost = GetUpgradeCost(type);
            OnUpgradeLevelChanged?.Invoke(type, upgradeLevels[type], nextCost);

            return true;
        }

        private void ApplyUpgrade(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.CommonRare:
                    ApplyDamageBonusToRarities(statBonuses[type], HeroRarity.Common, HeroRarity.Rare);
                    break;

                case UpgradeType.Epic:
                    ApplyDamageBonusToRarities(statBonuses[type], HeroRarity.Epic);
                    break;

                case UpgradeType.UniqueLegend:
                    ApplyDamageBonusToRarities(statBonuses[type], HeroRarity.Unique, HeroRarity.Legend);
                    break;

                case UpgradeType.SummonRate:
                    summonManager.UpgradeRates(statBonusPerLevel * 100f);
                    break;
            }
        }

        private void ApplyDamageBonusToRarities(float totalBonus, params HeroRarity[] rarities)
        {
            foreach (HeroBase hero in heroRegistry.Heroes)
            {
                foreach (HeroRarity rarity in rarities)
                {
                    if (hero.Rarity == rarity)
                    {
                        hero.ApplyDamageBonus(totalBonus);
                        break;
                    }
                }
            }
        }
        #endregion

        #region 계산
        public int GetUpgradeCost(UpgradeType type)
        {
            int level = upgradeLevels[type];
            float cost = baseCost * Mathf.Pow(1f + costIncreaseRate, level);

            return Mathf.RoundToInt(cost);
        }

        public int GetUpgradeLevel(UpgradeType type)
        {
            return upgradeLevels[type];
        }

        public float GetStatBonus(UpgradeType type)
        {
            return statBonuses[type];
        }
        #endregion

        #region 신규 영웅 보너스 적용
        private void OnHeroRegistered(HeroBase hero)
        {
            HeroRarity rarity = hero.Rarity;

            if ((rarity == HeroRarity.Common || rarity == HeroRarity.Rare) && statBonuses[UpgradeType.CommonRare] > 0f)
            {
                hero.ApplyDamageBonus(statBonuses[UpgradeType.CommonRare]);
            }

            if (rarity == HeroRarity.Epic && statBonuses[UpgradeType.Epic] > 0f)
            {
                hero.ApplyDamageBonus(statBonuses[UpgradeType.Epic]);
            }

            if ((rarity == HeroRarity.Unique || rarity == HeroRarity.Legend) && statBonuses[UpgradeType.UniqueLegend] > 0f)
            {
                hero.ApplyDamageBonus(statBonuses[UpgradeType.UniqueLegend]);
            }
        }
        #endregion
    }
}