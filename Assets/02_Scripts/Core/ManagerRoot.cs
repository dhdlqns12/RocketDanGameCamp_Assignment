using StarDefense.Managers;
using StarDefense.UI;
using UnityEngine;
namespace StarDefense.Core
{
    /// <summary>
    /// 전역 매니저 초기화 및 생명주기 관리
    /// </summary>
    public class ManagerRoot : Singleton<ManagerRoot>
    {
        [SerializeField] private UIManager uiManager;

        public UIManager UIManager => uiManager;

        #region 초기화
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            DataManager.Init();
        }

        protected override void Init()
        {
            uiManager.Init();
        }
        #endregion
    }
}
