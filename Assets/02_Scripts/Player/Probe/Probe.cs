using System;
using UnityEngine;

namespace StarDefense.Managers
{
    /// <summary>
    /// 탐사정 유닛
    /// </summary>
    public class Probe : MonoBehaviour
    {
        private Vector3 nexusPos;
        private Vector3 minePos;
        private float speed;
        private Action onMineralCollected;

        private Vector3 targetPos;
        private bool goingToMine = true;
        private bool hasMineral;

        #region 초기화
        public void Init(Vector3 mNexusPos, Vector3 mMinePos, float mSpeed, Action onCollected)
        {
            nexusPos = mNexusPos;
            minePos = mMinePos;
            speed = mSpeed;
            onMineralCollected = onCollected;

            transform.position = nexusPos;
            targetPos = minePos;
            goingToMine = true;
            hasMineral = false;

            Flip();
        }
        #endregion

        #region 유니티 Event
        private void Update()
        {
            Move();
        }
        #endregion

        #region 이동
        private void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                if (goingToMine)
                {
                    hasMineral = true;
                    goingToMine = false;
                    targetPos = nexusPos;
                }
                else
                {
                    if (hasMineral)
                    {
                        onMineralCollected?.Invoke();
                        hasMineral = false;
                    }

                    goingToMine = true;
                    targetPos = minePos;
                }

                Flip();
            }
        }

        /// <summary>
        /// 이동 방향에 따라 스프라이트 좌우 반전
        /// </summary>
        private void Flip()
        {
            float dirX = targetPos.x - transform.position.x;

            if (Mathf.Abs(dirX) > 0.01f)
            {
                Vector3 scale = transform.localScale;
                scale.x = dirX > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }
        #endregion
    }
}