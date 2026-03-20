using System;

namespace StarDefense.Data
{
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
