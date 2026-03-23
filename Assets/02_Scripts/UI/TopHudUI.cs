using StarDefense.Data;
using StarDefense.Managers;
using TMPro;
using UnityEngine;
namespace StarDefense.UI
{
    /// <summary>
    /// 상단 HUD (Scene)
    /// </summary>
    public class TopHudUI : UIBase
    {
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] private TextMeshProUGUI waveText;

        private WaveManager waveManager;

        #region 초기화
        protected override void SetupUI()
        {
        }

        public void SetDependencies(int stageId, WaveManager mWaveManager)
        {
            waveManager = mWaveManager;

            StageData stageData = DataManager.GetTable<StageData>().Get(stageId);

            if (stageData != null)
            {
                int stageNumber = stageId % 10;
                stageText.text = $"Stage: {stageData.chapter}-{stageNumber}";
            }

            waveManager.OnWaveChanged += OnWaveChanged;

            waveText.text = $"Wave: {waveManager.CurrentWave}/{waveManager.TotalWaves}";
        }
        #endregion

        #region 이벤트 구독
        protected override void SubscribeEvents()
        {
        }

        protected override void UnsubscribeEvents()
        {
        }
        #endregion

        #region UI 갱신
        private void OnWaveChanged(int currentWave, int totalWaves)
        {
            waveText.text = $"웨이브: {currentWave}/{totalWaves}";
        }
        #endregion
    }
}