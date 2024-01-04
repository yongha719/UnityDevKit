using System.Drawing;
using UnityEngine;


namespace MyObjectPool
{
    [CreateAssetMenu(fileName = "ObjectPoolData", menuName = "MyScriptable/ObjectPool", order = int.MinValue)]
    public class ObjectPoolData : ScriptableObject
    {
        public int size;

        public GameObject pooledObjectPrefab;

        public Transform poolParent;

        [Tooltip("자동 삭제를 켜놓을 것인지")]
        public bool shouldAutoDestroy;

        public bool shouldDestroyWithLerp;

        [Tooltip("자동으로 삭제할지 체크할 때 딜레이")]
        public float AutoDestroyPoolDelay = 1;

        [Tooltip("삭제하려고 할때 pool이 늘어날 수 있으니 그전에 두는 딜레이")]
        public float DelayBeforePossiblePoolIncrease = 10;

        [Tooltip("Lerp로 지울 때 한번에 지울 사이즈")]
        public int SizeToDestroy;

        [Tooltip("Lerp로 삭제할 때의 반복 딜레이")]
        public float AfterLerpDestroyDelay = 1f;


        public ObjectPool<T> CreatePool<T>(Transform parent) where T : MonoBehaviour, IPooledObject<T>
        {
            var pool = new ObjectPool<T>(this, parent, CreateObject, OnGetObject, OnReleaseObject, _defaultSize: size);
            pool.InitPool();

            return pool;
        }

        private T CreateObject<T>(ObjectPool<T> pool) where T : MonoBehaviour, IPooledObject<T>
        {
            GameObject Object = Instantiate(pooledObjectPrefab, poolParent);
            T obj = Object.GetComponent<T>();

            obj.Pool = pool;

            return obj;
        }

        private void OnGetObject<T>(T obj) where T : MonoBehaviour, IPooledObject<T>
        {
            obj.SetActive(true);
        }

        private void OnReleaseObject<T>(T obj) where T : MonoBehaviour, IPooledObject<T>
        {
            obj.SetActive(false);
        }
    }
}