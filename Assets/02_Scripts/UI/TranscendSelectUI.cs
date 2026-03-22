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
    public class TranscendSelectUI : MonoBehaviour
    {
        [Header("선택지 버튼(임시)")]
        [SerializeField] private Button[] optionButtons;
        [SerializeField] private TextMeshProUGUI[] optionNames;

        [Header("패널")]
        [SerializeField] private GameObject panel;

        [SerializeField] private TileInputHandler tileInputHandler;

        private HeroData[] currentOptions;

        #region 유니티 Event
        private void Awake()
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                int index = i;
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
            }

            Hide();
        }
        #endregion

        #region UI 표시/숨김
        public void Show(HeroData[] options)
        {
            if (options == null || options.Length == 0) return;

            currentOptions = options;

            panel.SetActive(true);

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

        public void Hide()
        {
            panel.SetActive(false);
            currentOptions = null;
        }
        #endregion

        #region 선택
        private void OnOptionSelected(int index)
        {
            if (currentOptions == null || index >= currentOptions.Length) return;

            tileInputHandler.OnTranscendConfirmed(currentOptions[index]);

            Hide();
        }
        #endregion
    }
}