using StarDefense.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace StarDefense.UI
{
    /// <summary>
    /// 초월 비용 버튼. Unique 영웅 클릭 시 타일 위치에 표시 (TilePopup)
    /// </summary>
    public class TranscendButtonUI : UIBase
    {
        [SerializeField] private Button transcendButton;
        [SerializeField] private TextMeshProUGUI priceText;

        [Header("위치 오프셋")]
        [SerializeField] private Vector2 offset = new Vector2(0f, 15f);

        private RectTransform rectTransform;
        private Camera mainCamera;
        private TileInputHandler tileInputHandler;

        #region 초기화
        protected override void SetupUI()
        {
            rectTransform = GetComponent<RectTransform>();
            mainCamera = Camera.main;
        }

        public void SetTileInputHandler(TileInputHandler handler)
        {
            tileInputHandler = handler;
        }
        #endregion

        #region 이벤트 구독
        protected override void SubscribeEvents()
        {
            transcendButton.onClick.AddListener(OnTranscendButtonClicked);
        }

        protected override void UnsubscribeEvents()
        {
            transcendButton.onClick.RemoveListener(OnTranscendButtonClicked);
        }
        #endregion

        #region 표시
        public void Show(Vector3 tileWorldPos, int cost, int currentGold)
        {
            base.Show();

            priceText.text = $"{cost}G";
            priceText.color = currentGold >= cost ? Color.white : Color.red;

            Vector2 screenPos = mainCamera.WorldToScreenPoint(tileWorldPos);
            rectTransform.position = screenPos + offset;
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