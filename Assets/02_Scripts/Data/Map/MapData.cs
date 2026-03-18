using System;

namespace StarDefense.Data
{
    /// <summary>
    /// 맵 레이아웃 데이터 layout 숫자가 모든 정보를 담음
    ///
    /// 타일 타입
    /// 0 = Empty (빈 공간, 적 이동 경로)
    /// 1 = Block (영웅 소환 가능)
    /// 2 = FixBlock (수리 필요, 수리 후 Block)
    /// 3 = Buff (영웅 소환 가능 + 버프)
    /// 4 = Spawn (적 소환 지점)
    /// 5 = Commander (지휘관 위치, 적 최종 목적지)
    /// </summary>
    [Serializable]
    public class MapData
    {
        public int mapWidth;
        public int mapHeight;
        public int[][] layout;
    }
}