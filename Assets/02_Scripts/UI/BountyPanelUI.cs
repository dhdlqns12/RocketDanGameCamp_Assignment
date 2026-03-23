using StarDefense.Core;
using System.Collections.Generic;
using StarDefense.Data;
using StarDefense.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace StarDefense.UI
{
    /// <summary>
    /// 현상금 패널 (Popup)
    /// </summary>
    public class BountyPanelUI : UIBase
    {
        [Header("슬롯")]
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject bountySlotPrefab;

        [Header("재화 아이콘")]
        [SerializeField] private Sprite goldIcon;
        [SerializeField] private Sprite mineralIcon;

        [Header("닫기")]
        [SerializeField] private Button closeButton;

        private BountyManager bountyManager;
        private List<BountySlot> slots = new List<BountySlot>();

        #region 초기화
        protected override void SetupUI()
        {
        }

        public void SetDependencies(BountyManager mBountyManager)
        {
            bountyManager = mBountyManager;

            CreateSlots();
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

        #region 이벤트 구독
        protected override void SubscribeEvents()
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        protected override void UnsubscribeEvents()
        {
            closeButton.onClick.RemoveListener(OnCloseClicked);
        }
        #endregion

        #region 버튼 이벤트
        private void OnCloseClicked()
        {
            ManagerRoot.Instance.UIManager.ClosePanel<BountyPanelUI>();
        }

        private void OnSlotClicked(int enemyId)
        {
            if (bountyManager.TrySpawnBounty(enemyId))
            {
                ManagerRoot.Instance.UIManager.ClosePanel<BountyPanelUI>();
            }
        }
        #endregion
    }
}
