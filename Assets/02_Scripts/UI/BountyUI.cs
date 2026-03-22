using System.Collections.Generic;
using StarDefense.Data;
using StarDefense.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarDefense.UI
{
    /// <summary>
    /// 현상금 UI
    /// </summary>
    public class BountyUI : MonoBehaviour
    {
        [Header("토글 버튼")]
        [SerializeField] private Button bountyButton;

        [Header("쿨타임 표시")]
        [SerializeField] private GameObject cooldownImg;
        [SerializeField] private TextMeshProUGUI cooldownText;

        [Header("패널")]
        [SerializeField] private GameObject bountyPanel;
        [SerializeField] private Button closeButton;
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject bountySlotPrefab;

        [Header("재화 아이콘")]
        [SerializeField] private Sprite goldIcon;
        [SerializeField] private Sprite mineralIcon;

        private BountyManager bountyManager;
        private List<BountySlot> slots = new List<BountySlot>();

        #region 초기화
        public void Init(BountyManager mBountyManager)
        {
            bountyManager = mBountyManager;

            bountyButton.onClick.AddListener(OnToggleClicked);
            closeButton.onClick.AddListener(Hide);

            bountyManager.OnCooldownStateChanged += OnCooldownStateChanged;
            bountyManager.OnCooldownTick += OnCooldownTick;

            CreateSlots();
            SetCooldownDisplay(false);

            Hide();
        }

        private void CreateSlots()
        {
            foreach (EnemyData data in bountyManager.BountyEnemies)
            {
                GameObject slotObj = Instantiate(bountySlotPrefab, contentParent);
                BountySlot slot = slotObj.GetComponent<BountySlot>();

                Sprite enemySprite = Resources.Load<Sprite>($"Sprite/Enemy/{data.enemyId}");

                bool isGoldReward = data.goldReward > 0;
                string reward = isGoldReward ? $"+{data.goldReward}" : $"+{data.mineralReward}";
                Sprite currencyIcon = isGoldReward ? goldIcon : mineralIcon;

                slot.Init(data.enemyId, enemySprite, data.enemyName, data.hp, reward, currencyIcon, OnSlotClicked);

                slots.Add(slot);
            }
        }
        #endregion

        #region UI 표시/숨김
        public void Show()
        {
            bountyPanel.SetActive(true);
        }

        public void Hide()
        {
            bountyPanel.SetActive(false);
        }

        private void OnToggleClicked()
        {
            if (!bountyManager.IsReady) return;

            if (bountyPanel.activeSelf)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
        #endregion

        #region 쿨타임 표시
        private void OnCooldownStateChanged(bool ready)
        {
            SetCooldownDisplay(!ready);

            if (!ready)
            {
                Hide();
            }
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

        #region 슬롯 클릭
        private void OnSlotClicked(int enemyId)
        {
            if (bountyManager.TrySpawnBounty(enemyId))
            {
                Hide();
            }
        }
        #endregion
    }
}