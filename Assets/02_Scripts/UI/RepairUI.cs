using StarDefense.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace StarDefense.UI
{
    /// <summary>
    /// 수리 UI
    /// </summary>
    public class RepairUI : MonoBehaviour
    {
        [SerializeField] private Button repairButton;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TileInputHandler tileInputHandler;

        [Header("위치 오프셋")]
        [SerializeField] private Vector2 offset = new Vector2(0f, 15f);

        private RectTransform buttonRect;
        private Camera mainCamera;

        #region 유니티 Event
        private void Awake()
        {
            buttonRect = repairButton.GetComponent<RectTransform>();
            mainCamera = Camera.main;

            repairButton.onClick.AddListener(OnRepairButtonClicked);
            Hide();
        }
        #endregion

        #region UI 표시/숨김
        public void Show(Vector3 tileWorldPos, int cost)
        {
            repairButton.gameObject.SetActive(true);

            priceText.text = $"{cost}G";

            Vector2 screenPos = mainCamera.WorldToScreenPoint(tileWorldPos);
            buttonRect.position = screenPos + offset;
        }

        public void Hide()
        {
            repairButton.gameObject.SetActive(false);
        }
        #endregion

        #region 버튼 이벤트
        private void OnRepairButtonClicked()
        {
            tileInputHandler.OnRepairConfirmed();
        }
        #endregion
    }
}