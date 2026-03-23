using StarDefense.Data;
using StarDefense.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarDefense.UI
{
    /// <summary>
    /// 초월 선택지 UI
    /// </summary>
    public class TranscendSelectUI : UIBase
    {
        [Header("선택지 버튼(임시)")]
        [SerializeField] private Button[] optionButtons;
        [SerializeField] private TextMeshProUGUI[] optionNames;

        private TileInputHandler tileInputHandler;
        private HeroData[] currentOptions;

        #region 초기화
        protected override void SetupUI()
        {
        }

        public void SetTileInputHandler(TileInputHandler handler)
        {
            tileInputHandler = handler;
        }
        #endregion

        #region 이벤트 구독
        protected override void SubscribeEvents()
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                int index = i;
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
            }
        }

        protected override void UnsubscribeEvents()
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                optionButtons[i].onClick.RemoveAllListeners();
            }
        }
        #endregion

        #region 표시
        public void Show(HeroData[] options)
        {
            if (options == null || options.Length == 0) return;

            currentOptions = options;

            base.Show();

            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (i < options.Length && options[i] != null)
                {
                    optionButtons[i].gameObject.SetActive(true);
                    optionNames[i].text = options[i].heroName;
                }
                else
                {
                    optionButtons[i].gameObject.SetActive(false);
                }
            }
        }

        protected override void OnClose()
        {
            currentOptions = null;
        }
        #endregion

        #region 선택
        private void OnOptionSelected(int index)
        {
            if (currentOptions == null || index >= currentOptions.Length) return;

            tileInputHandler.OnTranscendConfirmed(currentOptions[index]);

            Close();
        }
        #endregion
    }
}