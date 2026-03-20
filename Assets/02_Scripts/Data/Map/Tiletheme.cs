using UnityEngine;

namespace StarDefense.Data
{
    /// <summary>
    /// 매핑
    /// 0 = Empty (배치 안함)
    /// 1 = Block, 2 = FixBlock, 3 = Buff,
    /// 4 = Spawn, 5 = Commander, 6 = Obstacle
    /// 7 = Wall (배치 안함)
    /// </summary>
    [CreateAssetMenu(fileName = "NewTileTheme", menuName = "Tile/Tile Theme")]
    public class TileTheme : ScriptableObject
    {
        public string ThemeName;

        [Header("Tile Prefabs")]
        public GameObject block;       // 1
        public GameObject fixBlock;    // 2
        public GameObject buff;        // 3
        public GameObject spawn;       // 4
        public GameObject commander;   // 5
        public GameObject obstacle;    // 6
    }
}
