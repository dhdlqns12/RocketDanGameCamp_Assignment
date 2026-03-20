using UnityEngine;
using StarDefense.Managers;

namespace StarDefense.Core
{
    /// <summary>
    /// 전역 매니저 초기화 및 생명주기 관리
    /// static 매니저는 BeforeSceneLoad에서
    /// MonoBehaviour 전역 매니저는 Init()에서 순서대로 초기화
    /// 씬 종속 매니저는 여기서 관리 X
    /// </summary>
    public class ManagerRoot : Singleton<ManagerRoot>
    {
        #region 초기화
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            // static 매니저 초기화
            DataManager.Init();
        }

        protected override void Init()
        {
            // MonoBehaviour 전역 매니저 초기화
        }
        #endregion
    }
}