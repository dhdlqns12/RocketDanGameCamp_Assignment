using UnityEngine;

namespace StarDefense.Managers
{
    public class StageInitManager : MonoBehaviour
    {
        [Header("Stage")]
        [SerializeField] private int stageId;

        [Header("Scene Managers")]
        [SerializeField] private MapManager mapManager;
        [SerializeField] private WaveManager waveManager;

        private void Start()
        {
            InitializeStage();
        }

        private void InitializeStage()
        {
            // 1. 맵 먼저 (그리드 + 타일 + 웨이포인트)
            mapManager.InitMap(stageId);
            // 2. 웨이브 (맵 경로 데이터 필요)
            waveManager.Initialize(stageId);
            // 3. 첫 웨이브 시작
            waveManager.StartNextWave();

        }
    }
}
