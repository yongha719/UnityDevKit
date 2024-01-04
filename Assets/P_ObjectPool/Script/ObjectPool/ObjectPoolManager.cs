using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace MyObjectPool
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        /// Value는 <see cref="ObjectPool{T}"/>임
        private Dictionary<string, object> poolDic = new Dictionary<string, object>();

        [SerializeField]
        private List<ObjectPoolData> poolDatalist = new List<ObjectPoolData>();

        private Dictionary<string, ObjectPoolData> poolDataDic;

        protected override void Awake()
        {
            base.Awake();
            poolDataDic = poolDatalist.ToDictionary(key => key.pooledObjectPrefab.name);
        }

        public IGetObjectPool<T> GetPool<T>() where T : MonoBehaviour, IPooledObject<T>
        {
            string name = typeof(T).Name;

            if (poolDic.ContainsKey(name) == false)
                poolDic.Add(name, poolDataDic[name].CreatePool<T>(transform));

            return (IGetObjectPool<T>)poolDic[name];
        }

        public void ReleaseObject<T>(T obj) where T : MonoBehaviour, IPooledObject<T>
        {
            string name = typeof(T).Name;

            if (poolDic.ContainsKey(name) == false)
                return;

            IReleaseObjectPool<T> releasePool = (IReleaseObjectPool<T>)poolDic[name];
            releasePool.Release(obj);
        }
    }
}