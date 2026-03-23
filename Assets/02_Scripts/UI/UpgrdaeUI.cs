using StarDefense.Map;
using UnityEngine;
using UnityEngine.UI;

namespace StarDefense.UI
{
    /// <summary>
    /// 승급 UI
    /// </summary>
    public class UpgradeUI : UIBase
    {
        [SerializeField] private Button upgradeButton;

        [Header("위치 오프셋")]
        [SerializeField] private Vector2 offset = new Vector2(0f, -50f);

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
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }

        protected override void UnsubscribeEvents()
        {
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
        }
        #endregion

        #region 표시
        public void Show(Vector3 tileWorldPos)
        {
            base.Show();

            Vector2 screenPos = mainCamera.WorldToScreenPoint(tileWorldPos);
            rectTransform.position = screenPos + offset;
        }
        #endregion

        #region 버튼 이벤트
        private void OnUpgradeButtonClicked()
        {
            tileInputHandler.OnUpgradeConfirmed();
        }
        #endregion
    }
}