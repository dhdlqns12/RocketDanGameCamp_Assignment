using StarDefense.Core;
using StarDefense.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace StarDefense.UI
{
    /// <summary>
    /// 하단 HUD (Scene)
    /// </summary>
    public class BottomHudUI : UIBase
    {
        [Header("버튼")]
        [SerializeField] private Button bountyButton;
        [SerializeField] private Button probeButton;
        [SerializeField] private Button upgradeButton;

        [Header("현상금 쿨타임")]
        [SerializeField] private GameObject cooldownImg;
        [SerializeField] private TextMeshProUGUI cooldownText;

        private BountyManager bountyManager;

        #region 초기화
        protected override void SetupUI()
        {
        }

        public void SetDependencies(BountyManager mBountyManager)
        {
            bountyManager = mBountyManager;

            bountyManager.OnCooldownStateChanged += OnCooldownStateChanged;
            bountyManager.OnCooldownTick += OnCooldownTick;

            SetCooldownDisplay(false);
        }
        #endregion

        #region 이벤트 구독
        protected override void SubscribeEvents()
        {
            bountyButton.onClick.AddListener(OnBountyClicked);
            probeButton.onClick.AddListener(OnProbeClicked);
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }

        protected override void UnsubscribeEvents()
        {
            bountyButton.onClick.RemoveListener(OnBountyClicked);
            probeButton.onClick.RemoveListener(OnProbeClicked);
            upgradeButton.onClick.RemoveListener(OnUpgradeClicked);
        }
        #endregion

        #region 버튼 이벤트
        private void OnBountyClicked()
        {
            if (bountyManager != null && !bountyManager.IsReady) return;

            if (ManagerRoot.Instance.UIManager.IsShowing<BountyPanelUI>())
            {
                ManagerRoot.Instance.UIManager.ClosePanel<BountyPanelUI>();
            }
            else
            {
                ManagerRoot.Instance.UIManager.ShowPanel<BountyPanelUI>();
            }
        }

        private void OnProbeClicked()
        {
            if (ManagerRoot.Instance.UIManager.IsShowing<ProbePanelUI>())
            {
                ManagerRoot.Instance.UIManager.ClosePanel<ProbePanelUI>();
            }
            else
            {
                ManagerRoot.Instance.UIManager.ShowPanel<ProbePanelUI>();
            }
        }

        private void OnUpgradeClicked()
        {
            if (ManagerRoot.Instance.UIManager.IsShowing<StatUpgradePanelUI>())
            {
                ManagerRoot.Instance.UIManager.ClosePanel<StatUpgradePanelUI>();
            }
            else
            {
                ManagerRoot.Instance.UIManager.ShowPanel<StatUpgradePanelUI>();
            }
        }
        #endregion

        #region 현상금 쿨타임
        private void OnCooldownStateChanged(bool ready)
        {
            SetCooldownDisplay(!ready);
        }

        private void OnCooldownTick(int minutes, int seconds)
        {
            cooldownText.text = $"{minutes:00}:{seconds:00}";
        }

        private void SetCooldownDisplay(bool showCooldown)
        {
            cooldownImg.SetActive(showCooldown);
            cooldownText.gameObject.SetActive(showCooldown);
            bountyButton.interactable = !showCooldown;
        }
        #endregion
    }
}