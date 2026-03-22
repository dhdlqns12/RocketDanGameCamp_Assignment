using StarDefense.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace StarDefense.UI
{
    /// <summary>
    /// 소환 UI
    /// </summary>
    public class SummonUI : MonoBehaviour
    {
        [SerializeField] private Button summonButton;
        [SerializeField] private TextMeshProUGUI summonText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TileInputHandler tileInputHandler;

        [Header("위치 오프셋")]
        [SerializeField] private Vector2 offset = new Vector2(0f, -50f);

        private RectTransform buttonRect;
        private Camera mainCamera;

        #region 유니티 Event
        private void Awake()
        {
            buttonRect = summonButton.GetComponent<RectTransform>();
            mainCamera = Camera.main;

            summonButton.onClick.AddListener(OnSummonButtonClicked);
            Hide();
        }
        #endregion

        #region UI 표시/숨김
        /// <summary>
        /// 타일 월드 좌표에 버튼을 표시
        /// </summary>
        public void Show(Vector3 tileWorldPos, int cost, int currentGold)
        {
            summonButton.gameObject.SetActive(true);

            priceText.text = $"{cost}G";
            priceText.color = currentGold >= cost ? Color.white : Color.red;

            Vector2 screenPos = mainCamera.WorldToScreenPoint(tileWorldPos);
            buttonRect.position = screenPos + offset;
        }

        public void Hide()
        {
            summonButton.gameObject.SetActive(false);
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