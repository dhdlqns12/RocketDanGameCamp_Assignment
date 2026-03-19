using System;

namespace StarDefense.Data
{
    /// <summary>
    /// 적 데이터. DataManager에서 테이블로 관리.
    /// </summary>
    [Serializable]
    public class EnemyData
    {
        public int enemyId;
        public string enemyName;
        public int hp;
        public float moveSpeed;
        public int damage;
        public int goldReward;
        public int mineralReward;
    }
}