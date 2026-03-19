using System.Collections.Generic;
using UnityEngine;

namespace StarDefense.Enemy
{
    /// <summary>
    /// 적 오브젝트 풀 프리팹별로 풀을 관리.
    /// </summary>
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private int initialPoolSize;

        private Dictionary<string, Queue<Enemy>> pools = new Dictionary<string, Queue<Enemy>>();
        private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>();
        private Transform poolParent;

        private void Awake()
        {
            poolParent = new GameObject("EnemyPool").transform;
            poolParent.SetParent(transform);
        }

        /// <summary>
        /// 프리팹을 등록하고 초기 풀을 생성
        /// </summary>
        public void RegisterPrefab(string key, GameObject prefab)
        {
            if (prefabMap.ContainsKey(key)) return;

            prefabMap[key] = prefab;
            pools[key] = new Queue<Enemy>();

            for (int i = 0; i < initialPoolSize; i++)
            {
                Enemy enemy = CreateEnemy(key);
                enemy.gameObject.SetActive(false);
                pools[key].Enqueue(enemy);
            }
        }

        /// <summary>
        /// 풀에서 적을 가져옴 없으면 새로 생성
        /// </summary>
        public Enemy Get(string key)
        {
            if (!pools.ContainsKey(key))
            {
                Debug.LogError($"[EnemyPool] Key not registered: {key}");
                return null;
            }

            Queue<Enemy> pool = pools[key];

            if (pool.Count > 0)
            {
                Enemy enemy = pool.Dequeue();
                enemy.gameObject.SetActive(true);
                return enemy;
            }

            return CreateEnemy(key);
        }

        /// <summary>
        /// 적을 풀에 반환
        /// </summary>
        public void Return(Enemy enemy, string key)
        {
            enemy.gameObject.SetActive(false);
            pools[key].Enqueue(enemy);
        }

        private Enemy CreateEnemy(string key)
        {
            GameObject obj = Instantiate(prefabMap[key], poolParent);
            Enemy enemy = obj.GetComponent<Enemy>();
            return enemy;
        }
    }
}