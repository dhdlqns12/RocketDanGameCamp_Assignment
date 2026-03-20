using UnityEngine;
using StarDefense.Core;

namespace StarDefense.Hero
{
    /// <summary>
    /// 영웅 공통 인터페이스
    /// </summary>
    public interface IHero
    {
        public int AttackDamage { get; }
        public float AttackRange { get; }
        public float AttackSpeed { get; }
        public HeroRarity Rarity { get; }
        public HeroTribe Tribe { get; }
        public bool CanUpgrade { get; }
        public Transform Transform { get; }
        public void Attack();
    }
}