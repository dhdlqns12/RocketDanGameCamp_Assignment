using StarDefense.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace StarDefense.UI
{
    public class SummonUI : MonoBehaviour
    {
        [SerializeField] private Button summonButton;
        [SerializeField] private TextMeshProUGUI summonText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TileInputHandler tileInputHandler;

        [Header("위치 오프셋")]
        [SerializeField] private Vector2 offset = new Vector2(0f, 15f);

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
        public void Show(Vector3 tileWorldPos, int cost)
        {
            summonButton.gameObject.SetActive(true);

            priceText.text = $"{cost}G";

            // 월드 좌표 → 스크린 좌표 → UI 위치
            Vector2 screenPos = mainCamera.WorldToScreenPoint(tileWorldPos);
            buttonRect.position = screenPos + offset;
        }

        public void Hide()
        {
            summonButton.gameObject.SetActive(false);
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
