using StarDefense.Core;
using StarDefense.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarDefense.UI
{
    /// <summary>
    /// 강화 UI
    /// </summary>
    public class UpgradeStatUI : MonoBehaviour
    {
        [Header("토글 버튼 (HUDCanvas)")]
        [SerializeField] private Button upgradeToggleButton;

        [Header("강화 패널 (PanelCanvas)")]
        [SerializeField] private GameObject upgradePanel;

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

        private StatUpgradeManager statUpgradeManager;
        private int currentGold;

        #region 초기화
        public void Init(StatUpgradeManager mStatUpgradeManager, Currency.Gold gold)
        {
            statUpgradeManager = mStatUpgradeManager;

            upgradeToggleButton.onClick.AddListener(Toggle);
            commonRareButton.onClick.AddListener(() => OnUpgradeClicked(UpgradeType.CommonRare));
            epicButton.onClick.AddListener(() => OnUpgradeClicked(UpgradeType.Epic));
            uniqueLegendButton.onClick.AddListener(() => OnUpgradeClicked(UpgradeType.UniqueLegend));
            summonRateButton.onClick.AddListener(() => OnUpgradeClicked(UpgradeType.SummonRate));

            statUpgradeManager.OnUpgradeLevelChanged += OnUpgradeLevelChanged;
            gold.OnGoldChanged += OnGoldChanged;

            currentGold = gold.CurrentGold;

            SetCurrencyIcons();

            Hide();
        }

        /// <summary>
        /// UpgradeType에 따라 재화 아이콘 동적 로드
        /// Resources/Sprite/Currency/Gold, Mineral
        /// </summary>
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

        /// <summary>
        /// UpgradeType별 사용 재화 결정. 추후 변경 시 여기만 수정
        /// </summary>
        private string GetCurrencySpriteName(UpgradeType type)
        {
            return type switch
            {
                UpgradeType.CommonRare => "Gold",
                UpgradeType.Epic => "Gold",
                UpgradeType.UniqueLegend => "Gold",
                UpgradeType.SummonRate => "Mineral",
                _ => "Gold"
            };
        }
        #endregion

        #region UI 표시/숨김
        public void Show()
        {
            upgradePanel.SetActive(true);
            RefreshAll();
        }

        public void Hide()
        {
            upgradePanel.SetActive(false);
        }

        public void Toggle()
        {
            if (upgradePanel.activeSelf)
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
            RefreshAll();
        }

        private void OnGoldChanged(int gold, int delta)
        {
            currentGold = gold;

            if (upgradePanel.activeSelf)
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
        #endregion
    }
}