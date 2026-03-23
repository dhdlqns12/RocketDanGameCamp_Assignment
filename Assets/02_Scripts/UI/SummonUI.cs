using StarDefense.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarDefense.UI
{
    /// <summary>
    /// 소환 UI
    /// </summary>
    public class SummonUI : UIBase
    {
        [SerializeField] private Button summonButton;
        [SerializeField] private TextMeshProUGUI priceText;

        [Header("위치 오프셋")]
        [SerializeField] private Vector2 offset = new Vector2(0f, -15f);

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
            summonButton.onClick.AddListener(OnSummonButtonClicked);
        }

        protected override void UnsubscribeEvents()
        {
            summonButton.onClick.RemoveListener(OnSummonButtonClicked);
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
        private void OnSummonButtonClicked()
        {
            tileInputHandler.OnSummonConfirmed();
        }
        #endregion
    }
}