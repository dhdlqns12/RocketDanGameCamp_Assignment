using UnityEngine;
using StarDefense.Core;

namespace StarDefense.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        [Header("UI 설정")]
        [SerializeField] protected UIType uiType;

        public UIType UIType => uiType;

        protected bool isInit = false;
        #region 유니티 Event
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }
        #endregion

        #region 초기화
        public virtual void Init()
        {
            if (isInit) return;

            SetupUI();
            isInit = true;
        }

        protected abstract void SetupUI();
        protected abstract void SubscribeEvents();
        protected abstract void UnsubscribeEvents();
        #endregion

        #region 표시/숨김
        public virtual void Show()
        {
            gameObject.SetActive(true);
            OnShow();
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            OnClose();
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnClose()
        {
        }
        #endregion
    }
}