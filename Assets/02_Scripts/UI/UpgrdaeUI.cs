using StarDefense.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarDefense.UI
{
    /// <summary>
    /// 승급 UI. 승급 가능 영웅 클릭 시 표시
    /// </summary>
    public class UpgradeUI : MonoBehaviour
    {
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TextMeshProUGUI upgradeText;
        [SerializeField] private TileInputHandler tileInputHandler;

        [Header("위치 오프셋")]
        [SerializeField] private Vector2 offset = new Vector2(0f, -50f);

        private RectTransform buttonRect;
        private Camera mainCamera;

        #region 유니티 Event
        private void Awake()
        {
            buttonRect = upgradeButton.GetComponent<RectTransform>();
            mainCamera = Camera.main;

            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
            Hide();
        }
        #endregion

        #region UI 표시/숨김
        public void Show(Vector3 tileWorldPos)
        {
            upgradeButton.gameObject.SetActive(true);

            Vector2 screenPos = mainCamera.WorldToScreenPoint(tileWorldPos);
            buttonRect.position = screenPos + offset;
        }

        public void Hide()
        {
            upgradeButton.gameObject.SetActive(false);
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