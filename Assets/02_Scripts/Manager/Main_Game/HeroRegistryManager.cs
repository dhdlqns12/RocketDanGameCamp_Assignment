using System;
using System.Collections.Generic;
using UnityEngine;
using StarDefense.Hero;

namespace StarDefense.Managers
{
    /// <summary>
    /// 소환된 영웅 리스트 관리
    /// </summary>
    public class HeroRegistryManager : MonoBehaviour
    {
        private List<HeroBase> heroes = new List<HeroBase>();

        public IReadOnlyList<HeroBase> Heroes => heroes;
        public int Count => heroes.Count;

        /// <summary>
        /// 영웅 등록/해제 시 발행
        /// </summary>
        public event Action<HeroBase> OnHeroRegistered;
        public event Action<HeroBase> OnHeroUnregistered;

        #region 등록/해제
        public void Register(HeroBase hero)
        {
            heroes.Add(hero);

            OnHeroRegistered?.Invoke(hero);
        }

        public void Unregister(HeroBase hero)
        {
            heroes.Remove(hero);

            OnHeroUnregistered?.Invoke(hero);
        }
        #endregion

        #region 조회
        public int CountById(int heroId)
        {
            int count = 0;

            foreach (HeroBase hero in heroes)
            {
                if (hero.heroId == heroId)
                {
                    count++;
                }
            }

            return count;
        }

        public List<HeroBase> GetHeroesById(int heroId)
        {
            List<HeroBase> result = new List<HeroBase>();

            foreach (HeroBase hero in heroes)
            {
                if (hero.heroId == heroId)
                {
                    result.Add(hero);
                }
            }

            return result;
        }
        #endregion
    }
}