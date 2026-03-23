using System.Collections.Generic;
using UnityEngine;
using StarDefense.Core;
namespace StarDefense.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Canvas")]
        [SerializeField] private Canvas sceneCanvas;
        [SerializeField] private Canvas popupCanvas;
        [SerializeField] private Canvas tilePopupCanvas;

        [Header("UI 프리팹")]
        [SerializeField] private List<UIBase> uiPrefabLists;

        private Dictionary<string, UIBase> uiPanels = new Dictionary<string, UIBase>();
        private UIBase currentScenePanel;
        private UIBase currentPopupPanel;
        private UIBase currentTilePopup;
        #region 초기화
        public void Init()
        {
            InstantiatePrefabs();
            InitPanels();
        }

        private void InstantiatePrefabs()
        {
            foreach (var prefab in uiPrefabLists)
            {
                Canvas targetCanvas = GetTargetCanvas(prefab.UIType);

                if (targetCanvas != null)
                {
                    UIBase instance = Instantiate(prefab, targetCanvas.transform);
                    instance.name = prefab.name;
                    uiPanels[instance.GetType().Name] = instance;
                }
            }
        }

        private Canvas GetTargetCanvas(UIType uiType)
        {
            return uiType switch
            {
                UIType.Scene => sceneCanvas,
                UIType.Popup => popupCanvas,
                UIType.TilePopup => tilePopupCanvas,
                _ => sceneCanvas
            };
        }

        private void InitPanels()
        {
            foreach (var panel in uiPanels.Values)
            {
                panel.Init();

                if (panel.UIType != UIType.Scene)
                {
                    panel.gameObject.SetActive(false);
                }
            }
        }
        #endregion

        #region 패널 관리
        public T GetPanel<T>() where T : UIBase
        {
            if (uiPanels.TryGetValue(typeof(T).Name, out UIBase panel))
            {
                return panel as T;
            }

            Debug.LogWarning($"패널 찾을 수 없음: {typeof(T).Name}");
            return null;
        }

        public void ShowPanel<T>() where T : UIBase
        {
            UIBase panel = GetPanel<T>();

            if (panel == null) return;

            switch (panel.UIType)
            {
                case UIType.Scene:
                    if (currentScenePanel != null && currentScenePanel != panel)
                    {
                        currentScenePanel.Close();
                    }

                    currentScenePanel = panel;
                    break;

                case UIType.Popup:
                    if (currentPopupPanel != null && currentPopupPanel != panel)
                    {
                        currentPopupPanel.Close();
                    }

                    currentPopupPanel = panel;
                    break;

                case UIType.TilePopup:
                    if (currentTilePopup != null && currentTilePopup != panel)
                    {
                        currentTilePopup.Close();
                    }

                    currentTilePopup = panel;
                    break;
            }

            panel.Show();
        }

        public void ClosePanel<T>() where T : UIBase
        {
            UIBase panel = GetPanel<T>();

            if (panel == null) return;

            panel.Close();

            if (currentScenePanel == panel) currentScenePanel = null;
            if (currentPopupPanel == panel) currentPopupPanel = null;
            if (currentTilePopup == panel) currentTilePopup = null;
        }

        public void CloseAllPanels()
        {
            foreach (var panel in uiPanels.Values)
            {
                panel.Close();
            }

            currentScenePanel = null;
            currentPopupPanel = null;
            currentTilePopup = null;
        }

        public void CloseAllTilePopups()
        {
            foreach (var panel in uiPanels.Values)
            {
                if (panel.UIType == UIType.TilePopup)
                {
                    panel.Close();
                }
            }

            currentTilePopup = null;
        }

        public bool IsShowing<T>() where T : UIBase
        {
            UIBase panel = GetPanel<T>();
            return panel != null && panel.gameObject.activeSelf;
        }

        /// <summary>
        /// 모든 Canvas 하위에서 CurrencyUI 컴포넌트 찾기
        /// </summary>
        public CurrencyUI[] GetAllCurrencyUIs()
        {
            List<CurrencyUI> result = new List<CurrencyUI>();

            if (sceneCanvas != null)
                result.AddRange(sceneCanvas.GetComponentsInChildren<CurrencyUI>(true));
            if (popupCanvas != null)
                result.AddRange(popupCanvas.GetComponentsInChildren<CurrencyUI>(true));
            if (tilePopupCanvas != null)
                result.AddRange(tilePopupCanvas.GetComponentsInChildren<CurrencyUI>(true));

            return result.ToArray();
        }
        #endregion
    }
}