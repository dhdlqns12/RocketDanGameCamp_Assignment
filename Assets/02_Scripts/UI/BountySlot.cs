using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace StarDefense.UI
{
    /// <summary>
    /// 현상금 슬롯
    /// </summary>
    public class BountySlot : MonoBehaviour
    {
        [SerializeField] private Button slotButton;
        [SerializeField] private Image enemyImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Image rewardIcon;

        private int enemyId;
        private Action<int> onClickCallback;

        #region 초기화
        public void Init(int mEnemyId, Sprite sprite, string enemyName, int hp, string reward, Sprite currencyIcon, Action<int> onClick)
        {
            enemyId = mEnemyId;
            onClickCallback = onClick;
            if (sprite != null)
            {
                enemyImage.sprite = sprite;
            }
            nameText.text = enemyName;
            hpText.text = $"HP {hp}";
            rewardText.text = reward;
            if (currencyIcon != null && rewardIcon != null)
            {
                rewardIcon.sprite = currencyIcon;
            }
            slotButton.onClick.AddListener(OnClicked);
        }
        #endregion

        #region 쿨타임
        public void SetInteractable(bool interactable)
        {
            slotButton.interactable = interactable;
            enemyImage.color = interactable ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        #endregion

        #region 버튼 이벤트
        private void OnClicked()
        {
            onClickCallback?.Invoke(enemyId);
        }
        #endregion
    }
}