using StarDefense.Currency;
using StarDefense.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarDefense.Managers
{
    /// <summary>
    /// 탐사정 시스템
    /// </summary>
    public class ProbeManager : MonoBehaviour
    {
        [Header("위치")]
        [SerializeField] private Transform nexusPoint;
        [SerializeField] private Transform leftMinePoint;
        [SerializeField] private Transform rightMinePoint;

        [Header("프로브 설정")]
        [SerializeField] private GameObject probePrefab;
        [SerializeField] private float probeSpeed = 2f;
        [SerializeField] private int maxProbeCount = 20;

        [Header("구매 비용")]
        [SerializeField] private int baseCost = 50;
        [SerializeField] private float costIncreaseRate = 0.2f;

        private Gold gold;
        private Mineral mineral;
        private List<Probe> probes = new List<Probe>();
        private int probeCount;

        public int ProbeCount => probeCount;
        public int MaxProbeCount => maxProbeCount;
        public bool CanBuyMore => probeCount < maxProbeCount;

        /// <summary>
        /// 프로브 수 변경 시 발행
        /// </summary>
        public event Action<int, int> OnProbeCountChanged;

        #region 초기화
        public void Init(Gold mGold, Mineral mMineral)
        {
            gold = mGold;
            mineral = mMineral;
            probeCount = 0;
        }
        #endregion

        #region 구매
        /// <summary>
        /// 탐사정 구매. 가격 20%씩 증가
        /// </summary>
        public bool TryBuyProbe()
        {
            if (!CanBuyMore)
            {
                return false;
            }

            int cost = GetCurrentCost();

            if (!gold.SpendGold(cost))
            {
                return false;
            }

            SpawnProbe();

            return true;
        }

        /// <summary>
        /// 현재 구매 비용. 기본 50골드, 구매마다 20% 증가 (반올림)
        /// </summary>
        public int GetCurrentCost()
        {
            float cost = baseCost * Mathf.Pow(1f + costIncreaseRate, probeCount);

            return Mathf.RoundToInt(cost);
        }
        #endregion

        #region 프로브 스폰
        private void SpawnProbe()
        {
            probeCount++;

            // 홀수: 오른쪽, 짝수: 왼쪽
            bool isRight = probeCount % 2 == 1;
            Transform minePoint = isRight ? rightMinePoint : leftMinePoint;
            Vector3 spawnPos = isRight ? rightMinePoint.position : leftMinePoint.position;

            GameObject probeObj = Instantiate(probePrefab, spawnPos, Quaternion.identity, transform);
            Probe probe = probeObj.GetComponent<Probe>();

            probe.Init(nexusPoint.position, minePoint.position, probeSpeed, OnMineralCollected);

            probes.Add(probe);

            OnProbeCountChanged?.Invoke(probeCount, maxProbeCount);
        }

        /// <summary>
        /// 프로브가 왕복 완료 시 호출
        /// </summary>
        private void OnMineralCollected()
        {
            mineral.AddMineral(1);
        }
        #endregion
    }
}