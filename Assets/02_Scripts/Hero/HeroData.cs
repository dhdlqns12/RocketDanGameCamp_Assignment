using System;
namespace StarDefense.Data
{
    /// <summary>
    /// 영웅 데이터
    /// </summary>
    [Serializable]
    public class HeroData
    {
        public int heroId;
        public string heroName;
        public string rarity;
        public string tribe;
        public int attackDamage;
        public float attackSpeed;
        public float attackRange;
        public float splashRadius;
        public int heroSprite;
        public int projectileSprite;
        public int[] transcendOptions;
    }
}
