using System;
using UnityEngine;

namespace MyObjectPool
{
    [Serializable]
    public enum PlayerType
    {
        LeftPlayer = 1,
        RightPlayer = -1
    }

    public class Player : MonoBehaviour, IPooledObject<Player>
    {
        private PlayerType playerType = PlayerType.LeftPlayer;
        public PlayerType PlayerType
        {
            get => playerType;

            set
            {
                playerType = value;
                spriteRenderer.color = playerType == PlayerType.LeftPlayer ? Color.red : Color.blue;
            }
        }

        public float Speed;

        private SpriteRenderer spriteRenderer;

        public bool CanGet { get; set; }

        public int CalledObjectHashCode { get; set; }

        public IReleaseObjectPool<Player> Pool { get; set; }


        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void FixedUpdate()
        {
            float vertical = Input.GetAxis(PlayerType.ToString());

            transform.Translate(Vector2.down * (vertical * Speed * Time.deltaTime), Space.Self);
        }

        public void Attack()
        {
            IGetObjectPool<Bullet> bulletPool = ObjectPoolManager.Instance.GetPool<Bullet>();
            Bullet bullet = bulletPool.Get(transform, GetHashCode());

            bullet.SetDir(PlayerType);
            bullet.transform.SetPositionAndRotation(transform.position + (Vector3.right * (float)PlayerType), transform.rotation);
        }

    }
}