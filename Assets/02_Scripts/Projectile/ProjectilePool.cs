using System.Collections.Generic;
using UnityEngine;

namespace StarDefense.Hero
{
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int poolSize = 200;

        private Queue<Projectile> pool = new Queue<Projectile>();
        private Transform poolParent;

        #region 유니티 Event
        private void Awake()
        {
            poolParent = new GameObject("ProjectilePool").transform;
            poolParent.SetParent(transform);
            pool.Clear();
            for (int i = 0; i < poolSize; i++)
            {
                Projectile projectile = CreateProjectile();
                projectile.gameObject.SetActive(false);

                pool.Enqueue(projectile);
            }
        }
        #endregion

        #region 풀 조회/반환
        public Projectile Get(Vector3 position)
        {
            Projectile proj;
            if (pool.Count > 0)
            {
                proj = pool.Dequeue();

                proj.transform.position = position;

                proj.gameObject.SetActive(true);
            }
            else
            {
                proj = CreateProjectile();

                proj.transform.position = position;
            }
            return proj;
        }

        public void Return(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
            pool.Enqueue(projectile);
        }
        #endregion

        #region 생성
        private Projectile CreateProjectile()
        {
            GameObject obj = Instantiate(projectilePrefab, poolParent);
            Projectile proj = obj.GetComponent<Projectile>();

            proj.SetPool(this);

            return proj;
        }
        #endregion
    }
}