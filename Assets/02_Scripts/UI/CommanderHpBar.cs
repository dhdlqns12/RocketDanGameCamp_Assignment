using StarDefense.Hero;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace StarDefense.UI
{
    /// <summary>
    /// 지휘관 HP바. 지휘관 프리팹 하위 월드 Canvas에 배치
    /// 슬라이더 + 현재 체력 텍스트
    /// </summary>
    public class CommanderHpBar : MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;

        private Commander commander;

        #region 초기화
        public void Init(Commander mCommander)
        {
            commander = mCommander;

            commander.OnHpChanged += OnHpChanged;

            hpSlider.maxValue = commander.MaxHp;
            hpSlider.value = commander.CurrentHp;
            hpText.text = $"{commander.CurrentHp}";
        }
        #endregion

        #region UI 갱신
        private void OnHpChanged(int currentHp, int maxHp)
        {
            hpSlider.value = currentHp;
            hpText.text = $"{currentHp}";
        }
        #endregion
    }
}
