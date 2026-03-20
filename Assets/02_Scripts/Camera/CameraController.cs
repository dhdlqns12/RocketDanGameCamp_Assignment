using UnityEngine;

namespace StarDefense.Managers
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Background Reference")]
        [Tooltip("배경 SpriteRenderer. 이 스프라이트 크기에 맞춰 카메라 설정")]
        [SerializeField] private SpriteRenderer background;

        [Header("Padding")]
        [Tooltip("상단 여백 (UI 영역)")]
        [SerializeField] private float topPadding;

        [Tooltip("하단 여백 (UI 영역)")]
        [SerializeField] private float bottomPadding;

        [Tooltip("좌우 여백")]
        [SerializeField] private float sidePadding;

        private Camera cam;

        #region 유니티 Event
        private void Awake()
        {
            cam = GetComponent<Camera>();

            Init();
        }
        #endregion

        #region 초기화
        private void Init()
        {
            topPadding = -2f;
            bottomPadding = -4f;
            sidePadding = -4f;
        }

        public void AdjustCamera()
        {
            if (cam == null || background == null || background.sprite == null) return;

            Vector2 bgSize = background.sprite.bounds.size;
            Vector3 bgPos = background.transform.position;

            float totalHeight = bgSize.y + topPadding + bottomPadding;
            float totalWidth = bgSize.x + sidePadding * 2f;

            float screenAspect = (float)Screen.width / Screen.height;

            float sizeByHeight = totalHeight * 0.5f;

            float sizeByWidth = (totalWidth * 0.5f) / screenAspect;

            cam.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);

            float yOffset = (bottomPadding - topPadding) * 0.5f;
            cam.transform.position = new Vector3(
                bgPos.x,
                bgPos.y - yOffset,
                cam.transform.position.z
            );
        }
        #endregion
    }
}