using StarDefense.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarDefense.UI
{
    /// <summary>
    /// 초월 비용 버튼
    /// </summary>
    public class TranscendButtonUI : MonoBehaviour
    {
        [SerializeField] private Button transcendButton;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TileInputHandler tileInputHandler;

        [Header("위치 오프셋")]
        [SerializeField] private Vector2 offset = new Vector2(0f, 15f);

        private RectTransform buttonRect;
        private Camera mainCamera;

        #region 유니티 Event
        private void Awake()
        {
            buttonRect = transcendButton.GetComponent<RectTransform>();
            mainCamera = Camera.main;

            transcendButton.onClick.AddListener(OnTranscendButtonClicked);
            Hide();
        }
        #endregion

        #region UI 표시/숨김
        public void Show(Vector3 tileWorldPos, int cost, int currentGold)
        {
            transcendButton.gameObject.SetActive(true);

            priceText.text = $"{cost}G";
            priceText.color = currentGold >= cost ? Color.white : Color.red;

            Vector2 screenPos = mainCamera.WorldToScreenPoint(tileWorldPos);
            buttonRect.position = screenPos + offset;
        }

        public void Hide()
        {
            transcendButton.gameObject.SetActive(false);
        }

        public void UpdatePriceColor(int currentGold, int cost)
        {
            priceText.color = currentGold >= cost ? Color.white : Color.red;
        }
        #endregion

        #region 버튼 이벤트
        private void OnTranscendButtonClicked()
        {
            tileInputHandler.OnTranscendButtonClicked();
        }
        #endregion
    }
}