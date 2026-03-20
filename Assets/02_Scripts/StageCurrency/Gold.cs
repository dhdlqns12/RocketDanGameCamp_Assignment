using System;
using UnityEngine;

namespace StarDefense.Currency
{
    public class Gold
    {
        private int currentGold;

        public int CurrentGold => currentGold;

        public event Action<int, int> OnGoldChanged;

        #region 초기화
        public void Init(int startGold)
        {
            currentGold = startGold;

            OnGoldChanged?.Invoke(currentGold, startGold);

            Debug.Log($"[Gold] 초기화 | gold: {currentGold}");
        }
        #endregion

        #region 골드 조작
        public void AddGold(int amount)
        {
            currentGold += amount;

            OnGoldChanged?.Invoke(currentGold, amount);
        }

        public bool SpendGold(int amount)
        {
            if (currentGold < amount) return false;

            currentGold -= amount;

            OnGoldChanged?.Invoke(currentGold, -amount);

            return true;
        }
        #endregion
    }
}