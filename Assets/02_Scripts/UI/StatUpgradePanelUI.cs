using StarDefense.Core;
using StarDefense.Currency;
using StarDefense.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarDefense.UI
{
    /// <summary>
    /// 강화 패널
    /// </summary>
    public class StatUpgradePanelUI : UIBase
    {
        [Header("CommonRare")]
        [SerializeField] private Button commonRareButton;
        [SerializeField] private TextMeshProUGUI commonRareLevelText;
        [SerializeField] private TextMeshProUGUI commonRarePrice;
        [SerializeField] private Image commonRareCurrencyImg;

        [Header("Epic")]
        [SerializeField] private Button epicButton;
        [SerializeField] private TextMeshProUGUI epicLevelText;
        [SerializeField] private TextMeshProUGUI epicPrice;
        [SerializeField] private Image epicCurrencyImg;

        [Header("UniqueLegend")]
        [SerializeField] private Button uniqueLegendButton;
        [SerializeField] private TextMeshProUGUI uniqueLegendLevelText;
        [SerializeField] private TextMeshProUGUI uniqueLegendPrice;
        [SerializeField] private Image uniqueLegendCurrencyImg;

        [Header("SummonRate")]
        [SerializeField] private Button summonRateButton;
        [SerializeField] private TextMeshProUGUI summonRateLevelText;
        [SerializeField] private TextMeshProUGUI summonRatePrice;
        [SerializeField] private Image summonRateCurrencyImg;

        [Header("닫기")]
        [SerializeField] private Button closeButton;

        private StatUpgradeManager statUpgradeManager;
        private int currentGold;

        #region 초기화
        protected override void SetupUI()
        {
            SetCurrencyIcons();
        }

        public void SetDependencies(StatUpgradeManager mStatUpgradeManager, Gold mGold)
        {
            statUpgradeManager = mStatUpgradeManager;
            currentGold = mGold.CurrentGold;

            statUpgradeManager.OnUpgradeLevelChanged += OnUpgradeLevelChanged;
            mGold.OnGoldChanged += OnGoldChanged;
        }

        private void SetCurrencyIcons()
        {
            SetCurrencyIcon(UpgradeType.CommonRare, commonRareCurrencyImg);
            SetCurrencyIcon(UpgradeType.Epic, epicCurrencyImg);
            SetCurrencyIcon(UpgradeType.UniqueLegend, uniqueLegendCurrencyImg);
            SetCurrencyIcon(UpgradeType.SummonRate, summonRateCurrencyImg);
        }

        private void SetCurrencyIcon(UpgradeType type, Image img)
        {
            if (img == null) return;

            string spriteName = GetCurrencySpriteName(type);
            Sprite sprite = Resources.Load<Sprite>($"Sprite/UI/{spriteName}");

            if (sprite != null)
            {
                img.sprite = sprite;
            }
        }

        private string GetCurrencySpriteName(UpgradeType type)
        {
            return type switch
            {
                UpgradeType.CommonRare => "Gold",
                UpgradeType.Epic => "Gold",
                UpgradeType.UniqueLegend => "Gold",
                UpgradeType.SummonRate => "Gold",
                _ => "Gold"
            };
        }
        #endregion

        #region 이벤트 구독
        protected override void SubscribeEvents()
        {
            commonRareButton.onClick.AddListener(() => OnUpgradeClicked(UpgradeType.CommonRare));
            epicButton.onClick.AddListener(() => OnUpgradeClicked(UpgradeType.Epic));
            uniqueLegendButton.onClick.AddListener(() => OnUpgradeClicked(UpgradeType.UniqueLegend));
            summonRateButton.onClick.AddListener(() => OnUpgradeClicked(UpgradeType.SummonRate));
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        protected override void UnsubscribeEvents()
        {
            commonRareButton.onClick.RemoveAllListeners();
            epicButton.onClick.RemoveAllListeners();
            uniqueLegendButton.onClick.RemoveAllListeners();
            summonRateButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region 표시
        protected override void OnShow()
        {
            RefreshAll();
        }
        #endregion

        #region UI 갱신
        private void RefreshAll()
        {
            RefreshButton(UpgradeType.CommonRare, commonRareLevelText, commonRarePrice);
            RefreshButton(UpgradeType.Epic, epicLevelText, epicPrice);
            RefreshButton(UpgradeType.UniqueLegend, uniqueLegendLevelText, uniqueLegendPrice);
            RefreshButton(UpgradeType.SummonRate, summonRateLevelText, summonRatePrice);
        }

        private void RefreshButton(UpgradeType type, TextMeshProUGUI levelText, TextMeshProUGUI priceText)
        {
            int level = statUpgradeManager.GetUpgradeLevel(type);
            int cost = statUpgradeManager.GetUpgradeCost(type);

            levelText.text = $"Lv.{level}";
            priceText.text = $"{cost}";
            priceText.color = currentGold >= cost ? Color.white : Color.red;
        }

        private void OnUpgradeLevelChanged(UpgradeType type, int level, int nextCost)
        {
            if (gameObject.activeSelf)
            {
                RefreshAll();
            }
        }

        private void OnGoldChanged(int gold, int delta)
        {
            currentGold = gold;

            if (gameObject.activeSelf)
            {
                RefreshAll();
            }
        }
        #endregion

        #region 버튼 이벤트
        private void OnUpgradeClicked(UpgradeType type)
        {
            statUpgradeManager.TryUpgrade(type);
        }

        private void OnCloseClicked()
        {
            ManagerRoot.Instance.UIManager.ClosePanel<StatUpgradePanelUI>();
        }
        #endregion
    }
}