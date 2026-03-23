using UnityEngine;
namespace StarDefense.Hero
{
    /// <summary>
    /// 공격범위 원형 표시
    /// </summary>
    public class AttackRangeIndicator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer rangeSprite;

        [Header("색상")]
        [SerializeField] private Color rangeColor = new Color(0f, 1f, 0f, 0.2f);

        private void Awake()
        {
            if (rangeSprite == null)
            {
                rangeSprite = GetComponent<SpriteRenderer>();
            }

            rangeSprite.color = rangeColor;

            Hide();
        }

        /// <summary>
        /// 영웅 위치에 공격범위 원형 표시
        /// </summary>
        public void Show(Vector3 position, float attackRange)
        {
            transform.position = position;

            float diameter = attackRange * 2f;
            transform.localScale = new Vector3(diameter, diameter, 1f);

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}