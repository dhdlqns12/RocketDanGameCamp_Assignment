using System;
namespace StarDefense.Data
{
    /// <summary>
    /// 영웅 데이터. DataManager에서 테이블로 관리
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
    }
}
