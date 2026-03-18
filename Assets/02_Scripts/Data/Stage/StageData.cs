using System;

namespace StarDefense.Data
{
    /// <summary>
    /// 스테이지 데이터
    /// chapter로 큰 스테이지 구분 (1장, 2장), MapManager가 챕터에 맞는 TileTheme을 선택
    /// </summary>
    [Serializable]
    public class StageData
    {
        public int stageId;
        public string stageName;
        public int chapter;
        public string mapFileName;
        public int waveCount;
        public int startingGold;
        public int startingMineral;
    }
}
