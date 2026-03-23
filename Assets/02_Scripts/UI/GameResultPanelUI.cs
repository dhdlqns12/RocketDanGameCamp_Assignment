using TMPro;
using UnityEngine;
using StarDefense.Core;
namespace StarDefense.UI
{
    /// <summary>
    /// 게임 결과 패널 (Popup)
    /// 승리 또는 패배 텍스트 표시
    /// </summary>
    public class GameResultPanelUI : UIBase
    {
        [SerializeField] private TextMeshProUGUI resultText;

        #region 초기화
        protected override void SetupUI()
        {
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

        #region 표시
        public void ShowVictory()
        {
            resultText.text = "승리!";
            ManagerRoot.Instance.UIManager.ShowPanel<GameResultPanelUI>();
            Time.timeScale = 0f;
        }

        public void ShowDefeat()
        {
            resultText.text = "패배...";
            ManagerRoot.Instance.UIManager.ShowPanel<GameResultPanelUI>();
            Time.timeScale = 0f;
        }
        #endregion
    }
}