using StarDefense.Currency;
using StarDefense.Managers;
using TMPro;
using UnityEngine;
namespace StarDefense.UI
{
    /// <summary>
    /// 재화 표시 컴포넌트. 각 패널의 CurrencyPanel에 붙여서 사용
    /// 골드/미네랄/탐사정 실시간 갱신
    /// 필요 없는 필드는 Inspector에서 비워두면 됨
    /// </summary>
    public class CurrencyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI mineralText;
        [SerializeField] private TextMeshProUGUI probeText;

        private Gold gold;
        private Mineral mineral;
        private ProbeManager probeManager;
        private bool isInit;

        #region 초기화
        public void Init(Gold mGold, Mineral mMineral, ProbeManager mProbeManager)
        {
            if (isInit) return;

            gold = mGold;
            mineral = mMineral;
            probeManager = mProbeManager;
            isInit = true;

            if (gameObject.activeInHierarchy)
            {
                Subscribe();
                RefreshAll();
            }
        }
        #endregion

        #region 유니티 Event
        private void OnEnable()
        {
            if (!isInit) return;

            Subscribe();
            RefreshAll();
        }

        private void OnDisable()
        {
            if (!isInit) return;

            Unsubscribe();
        }

        private void Subscribe()
        {
            gold.OnGoldChanged += OnGoldChanged;
            mineral.OnMineralChanged += OnMineralChanged;

            if (probeManager != null && probeText != null)
            {
                probeManager.OnProbeCountChanged += OnProbeCountChanged;
            }
        }

        private void Unsubscribe()
        {
            gold.OnGoldChanged -= OnGoldChanged;
            mineral.OnMineralChanged -= OnMineralChanged;

            if (probeManager != null && probeText != null)
            {
                probeManager.OnProbeCountChanged -= OnProbeCountChanged;
            }
        }
        #endregion

        #region UI 갱신
        private void RefreshAll()
        {
            if (goldText != null) 
                goldText.text = $"{gold.CurrentGold}";

            if (mineralText != null)
                mineralText.text = $"{mineral.CurrentMineral}";

            if (probeText != null && probeManager != null) 
                probeText.text = $"{probeManager.ProbeCount}/{probeManager.MaxProbeCount}";
        }

        private void OnGoldChanged(int currentGold, int delta)
        {
            if (goldText != null) goldText.text = $"{currentGold}";
        }

        private void OnMineralChanged(int currentMineral, int delta)
        {
            if (mineralText != null) mineralText.text = $"{currentMineral}";
        }

        private void OnProbeCountChanged(int count, int max)
        {
            if (probeText != null) probeText.text = $"{count}/{max}";
        }
        #endregion
    }
}