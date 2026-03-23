using StarDefense.Core;
using StarDefense.Currency;
using StarDefense.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarDefense.UI
{
    /// <summary>
    /// 탐사정 패널 (Popup)
    /// </summary>
    public class ProbePanelUI : UIBase
    {
        [SerializeField] private Button buyButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI costText;

        private ProbeManager probeManager;
        private Gold gold;
        #region 초기화
        protected override void SetupUI()
        {
        }

        public void SetDependencies(ProbeManager mProbeManager, Gold mGold)
        {
            probeManager = mProbeManager;
            gold = mGold;

            probeManager.OnProbeCountChanged += OnProbeCountChanged;
            gold.OnGoldChanged += OnGoldChanged;
        }
        #endregion

        #region 이벤트 구독
        protected override void SubscribeEvents()
        {
            buyButton.onClick.AddListener(OnBuyClicked);
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        protected override void UnsubscribeEvents()
        {
            buyButton.onClick.RemoveListener(OnBuyClicked);
            closeButton.onClick.RemoveListener(OnCloseClicked);
        }
        #endregion

        #region 표시
        protected override void OnShow()
        {
            Refresh();
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
            if (gameObject.activeSelf)
            {
                Refresh();
            }
        }

        private void OnGoldChanged(int currentGold, int delta)
        {
            if (gameObject.activeSelf)
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

        private void OnCloseClicked()
        {
            ManagerRoot.Instance.UIManager.ClosePanel<ProbePanelUI>();
        }
        #endregion
    }
}