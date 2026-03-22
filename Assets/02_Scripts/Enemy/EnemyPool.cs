using System.Collections.Generic;
using UnityEngine;

namespace StarDefense.Enemy
{
    /// <summary>
    /// 적 오브젝트 풀
    /// </summary>
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private int initialPoolSize = 10;

        private Dictionary<string, Queue<EnemyBase>> pools = new Dictionary<string, Queue<EnemyBase>>();
        private Dictionary<string, GameObject> Baseprefab = new Dictionary<string, GameObject>();
        private Transform poolParent;

        #region 유니티 Event
        private void Awake()
        {
            poolParent = new GameObject("EnemyPool").transform;
            poolParent.SetParent(transform);
        }
        #endregion

        #region 풀 조회/반환
        /// <summary>
        /// 풀에서 적을 가져옴
        /// </summary>
        public EnemyBase Get(string key)
        {
            if (!pools.ContainsKey(key))
            {
                Debug.LogError($"[EnemyPool] Key not registered: {key}");
                return null;
            }

            Queue<EnemyBase> pool = pools[key];

            if (pool.Count > 0)
            {
                EnemyBase enemy = pool.Dequeue();
                enemy.gameObject.SetActive(true);
                return enemy;
            }

            return CreateEnemy(key);
        }

        /// <summary>
        /// 적을 풀에 반환
        /// </summary>
        public void Return(EnemyBase enemy, string key)
        {
            enemy.gameObject.SetActive(false);
            pools[key].Enqueue(enemy);
        }
        #endregion

        #region 프리팹 등록/생성
        /// <summary>
        /// 프리팹을 등록하고 초기 풀을 생성
        /// </summary>
        public void RegisterPrefab(string key, GameObject prefab)
        {
            if (Baseprefab.ContainsKey(key)) return;

            Baseprefab[key] = prefab;
            pools[key] = new Queue<EnemyBase>();

            for (int i = 0; i < initialPoolSize; i++)
            {
                EnemyBase enemy = CreateEnemy(key);
                enemy.gameObject.SetActive(false);
                pools[key].Enqueue(enemy);
            }
        }

        private EnemyBase CreateEnemy(string key)
        {
            GameObject obj = Instantiate(Baseprefab[key], poolParent);
            EnemyBase enemy = obj.GetComponent<EnemyBase>();
            enemy.SetPool(this, key);
            return enemy;
        }
        #endregion
    }
}
