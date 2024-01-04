using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

namespace MyObjectPool
{
    public class Bullet : MonoBehaviour, IPooledObject<Bullet>
    {
        public float Speed;

        private PlayerType playerType = PlayerType.LeftPlayer;

        private WaitForSeconds destroyWait = new WaitForSeconds(5f);

        public bool CanGet { get; set; }

        public int CalledObjectHashCode { get; set; }

        public IReleaseObjectPool<Bullet> Pool { get; set; }

        void OnEnable()
        {
            StartCoroutine(ReleaseCoroutine());
        }

        // WaitUntil을 사용하여 특정조건이 됐을 때 릴리즈되도록 해도 됨
        // UniTask 활용가능
        private IEnumerator ReleaseCoroutine()
        {
            yield return destroyWait;

            Pool.Release(this);
        }

        // PlayerType으로 Dir를 설정
        public void SetDir(PlayerType type)
        {
            playerType = type;
        }

        private void FixedUpdate()
        {
            transform.Translate(Vector2.right * (Speed * Time.deltaTime * (float)playerType), Space.World);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Pool.Release(this);
        }

        private void OnDisable()
        {
            Pool.ReleaseNextFrame(this);
        } 
    }
}
