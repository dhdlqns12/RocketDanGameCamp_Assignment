using UnityEngine;
using UnityEngine.Tilemaps;

namespace StarDefense.Data
{
    /// <summary>
    /// 타일 비주얼 테마 스테이지 컨셉에 따라 교체 가능
    /// 각 필드가 MapData layout 숫자와 대응
    ///
    /// 매핑
    /// 0 = Empty, 1 = Block, 2 = FixBlock, 3 = Buff,
    /// 4 = Spawn, 5 = Commander
    /// </summary>
    [CreateAssetMenu(fileName = "NewTileTheme", menuName = "Tile/Tile Theme")]
    public class TileTheme : ScriptableObject
    {
        public string ThemeName;

        [Header("Tile Mapping")]
        public TileBase Empty;       // 0
        public TileBase Block;       // 1
        public TileBase FixBlock;    // 2
        public TileBase Buff;        // 3
        public TileBase Spawn;       // 4
        public TileBase Commander;   // 5
    }
}
