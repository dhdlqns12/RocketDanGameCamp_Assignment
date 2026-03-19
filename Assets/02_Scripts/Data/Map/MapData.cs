using System;

namespace StarDefense.Data
{
    /// <summary>
    /// 맵 레이아웃 데이터
    ///
    /// 타일 타입
    /// 0 = Empty (빈 공간, 적 이동 경로)
    /// 1 = Block (영웅 소환 가능)
    /// 2 = FixBlock (수리 필요, 수리 후 Block)
    /// 3 = Buff (영웅 소환 가능 + 버프)
    /// 4 = Spawn (적 소환 지점, 비주얼 마커)
    /// 5 = Commander (지휘관 위치, 비주얼 마커)
    /// 6 = Obstacle (맵 내부, 통행/상호작용 불가)
    /// 7 = Wall (맵 외곽 벽, 배치 안함)
    /// </summary>
    [Serializable]
    public class MapData
    {
        public int mapId;
        public int mapWidth;
        public int mapHeight;
        public int[][] layout;
        public EnemyPath[] paths;
    }

    /// <summary>
    /// 적 이동 경로. 코너 포인트만 지정하면 사이 직선은 코드가 채운다.
    /// corners[0] = 스폰 위치, corners[마지막] = 지휘관 위치.
    /// 좌표는 게임 영역 기준 (Wall 제외, 좌하단 0,0).
    /// </summary>
    [Serializable]
    public class EnemyPath
    {
        public int pathId;
        public int[][] corners;
    }
}