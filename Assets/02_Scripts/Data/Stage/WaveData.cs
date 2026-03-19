using System;

namespace StarDefense.Data
{
    [Serializable]
    public class WaveData
    {
        public int stageId;
        public int waveCount;
        public float spawnInterval;
        public WavePhase[] phases;
    }

    [Serializable]
    public class WavePhase
    {
        public int fromWave;
        public int toWave;
        public int pathId;
        public int enemyId;
        public int startCount;
        public int endCount;
        public float delay;
    }
}
