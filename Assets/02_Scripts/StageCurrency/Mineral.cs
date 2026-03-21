using System;
using UnityEngine;

namespace StarDefense.Currency
{
    public class Mineral
    {
        private int currentMineral;

        public int CurrentMineral => currentMineral;

        public event Action<int, int> OnMineralChanged;

        #region 초기화
        public void Init(int startMineral)
        {
            currentMineral = startMineral;

            OnMineralChanged?.Invoke(currentMineral, startMineral);
        }
        #endregion

        #region 미네랄 조작
        public void AddMineral(int amount)
        {
            currentMineral += amount;

            OnMineralChanged?.Invoke(currentMineral, amount);
        }

        public bool SpendMineral(int amount)
        {
            if (currentMineral < amount) return false;

            currentMineral -= amount;

            OnMineralChanged?.Invoke(currentMineral, -amount);

            return true;
        }
        #endregion
    }
}