using StarDefense.Hero;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace StarDefense.UI
{
    /// <summary>
    /// 영웅 스탯 팝업
    /// </summary>
    public class HeroStatPopupUI : UIBase
    {
        [SerializeField] private Image heroImage;
        [SerializeField] private TextMeshProUGUI heroNameText;
        [SerializeField] private TextMeshProUGUI attackDamageText;
        [SerializeField] private TextMeshProUGUI attackSpeedText;

        #region 초기화
        protected override void SetupUI()
        {
        }
        #endregion

        #region 이벤트 구독
        protected override void SubscribeEvents()
        {
        }

        protected override void UnsubscribeEvents()
        {
        }
        #endregion

        #region 표시
        public void Show(HeroBase hero)
        {
            base.Show();

            Sprite heroSprite = Resources.Load<Sprite>($"Sprite/Hero/{hero.heroId}");

            if (heroSprite != null && heroImage != null)
            {
                heroImage.sprite = heroSprite;
            }

            heroNameText.text = hero.heroName;
            attackDamageText.text = $"공격력: {hero.AttackDamage}";
            attackSpeedText.text = $"공격 속도: {hero.AttackSpeed:F2}";
        }
        #endregion
    }
}