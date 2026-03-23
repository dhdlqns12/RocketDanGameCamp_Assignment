using StarDefense.Currency;
using StarDefense.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarDefense.UI
{
    /// <summary>
    /// 탐사정 UI
    /// BottomCanvas > Probe_Btn → 패널 토글
    /// PanelCanvas > ProbePanel → 구매 버튼 + 수량/비용
    /// </summary>
    public class ProbeUI : MonoBehaviour
    {
        [Header("토글 버튼 (BottomCanvas)")]
        [SerializeField] private Button probeToggleButton;

        [Header("패널 (PanelCanvas)")]
        [SerializeField] private GameObject probePanel;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI costText;

        private ProbeManager probeManager;
        private Gold gold;

        #region 초기화
        public void Init(ProbeManager mProbeManager, Gold mGold)
        {
            probeManager = mProbeManager;
            gold = mGold;

            probeToggleButton.onClick.AddListener(Toggle);
            buyButton.onClick.AddListener(OnBuyClicked);
            closeButton.onClick.AddListener(Hide);

            probeManager.OnProbeCountChanged += OnProbeCountChanged;
            gold.OnGoldChanged += OnGoldChanged;

            Hide();
        }
        #endregion

        #region UI 표시/숨김
        public void Show()
        {
            probePanel.SetActive(true);
            Refresh();
        }

        public void Hide()
        {
            probePanel.SetActive(false);
        }

        public void Toggle()
        {
            if (probePanel.activeSelf)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
        #endregion

        #region UI 갱신
        private void Refresh()
        {
            int cost = probeManager.GetCurrentCost();

            costText.text = $"{cost}G";
            costText.color = gold.CurrentGold >= cost ? Color.white : Color.red;

            buyButton.interactable = probeManager.CanBuyMore;
        }

        private void OnProbeCountChanged(int count, int max)
        {
            if (probePanel.activeSelf)
            {
                Refresh();
            }
        }

        private void OnGoldChanged(int currentGold, int delta)
        {
            if (probePanel.activeSelf)
            {
                Refresh();
            }
        }
        #endregion

        #region 버튼 이벤트
        private void OnBuyClicked()
        {
            probeManager.TryBuyProbe();
        }
        #endregion
    }
}
