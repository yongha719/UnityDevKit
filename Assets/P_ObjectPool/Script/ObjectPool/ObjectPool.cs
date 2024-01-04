using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyObjectPool
{
    [Serializable]
    public class ObjectPool<T> : IGetObjectPool<T>, IReleaseObjectPool<T> where T : MonoBehaviour, IPooledObject<T>
    {
        private ObjectPoolData pooledObjectData;

        private List<T> pool;

        private int count => pool.Count;
        private int countActive;
        private int countInactive => count - countActive;

        private int minPoolSize;

        private Func<ObjectPool<T>, T> createObject;
        private Action<T> onGet;
        private Action<T> onRelease;
        private Action<T> onDestroy;

        #region Destroy List
        [SerializeField, Tooltip("PoolData 리스트가 늘어났는지 체크")]
        private bool hasListIncreased = true;

        [SerializeField, Tooltip("아직 destroyList를 지우는 중인지 체크")]
        private bool stillDestroy = false;

        [Tooltip("지울 오브젝트 담아두는 리스트")]
        private List<T> destroyList;

        [Tooltip("오브젝트 삭제할 때 쓰는 코루틴")]
        private IEnumerator? destroyPoolCoroutine;

        [Tooltip("지울 수 있는지 체크할 때 딜레이")]
        private WaitForSeconds waitAutoDestroyDelay;

        [Tooltip("Lerp로 지울 때 두는 딜레이")]
        private WaitForSeconds waitLerpDestroyDelay;

        [Tooltip("삭제하려고 할때 pool이 늘어날 수 있으니 그전에 두는 딜레이")]
        private WaitForSeconds waitDelayBeforePossiblePoolIncrease;
        #endregion

        public ObjectPool(ObjectPoolData _pooledObjectData, Transform parent, Func<ObjectPool<T>, T> _createObject, Action<T> _onGet = null, Action<T> _onRelease = null, Action<T> _onDestroy = null, int _defaultSize = 20)
        {
            createObject = _createObject;
            onGet = _onGet;
            onRelease = _onRelease;
            onDestroy = _onDestroy;

            minPoolSize = _defaultSize;

            pooledObjectData = _pooledObjectData;
            pooledObjectData.poolParent = CreatePoolParent(parent);

            // 코루틴에서 쓸 wait들 초기화
            waitAutoDestroyDelay = new WaitForSeconds(pooledObjectData.AutoDestroyPoolDelay);
            waitLerpDestroyDelay = new WaitForSeconds(pooledObjectData.AfterLerpDestroyDelay);
            waitDelayBeforePossiblePoolIncrease = new WaitForSeconds(pooledObjectData.DelayBeforePossiblePoolIncrease);

            pool = new List<T>(minPoolSize);
            destroyList = new List<T>(minPoolSize / 5);
        }

        /// <param name="parent"> 
        /// <see cref="ObjectPoolManager"/>의 transform 
        /// </param>
        private Transform CreatePoolParent(Transform parent)
        {
            var poolParent = new GameObject($"{typeof(T).Name}PoolData Parent====").transform;
            poolParent.SetParent(parent);

            return poolParent;
        }

        public void InitPool()
        {
            for (int i = 0; i < minPoolSize; i++)
                pool.Add(createObject(this));

            if (pooledObjectData.shouldAutoDestroy)
                MonoMethods.Start(AutoDestroyCoroutine());
        }

        /// <summary>
        /// 지울 지 체크하고 
        /// <see cref="LerpDestroyCoroutine"/> 관리해줌
        /// </summary>
        private IEnumerator AutoDestroyCoroutine()
        {
            while (true)
            {
                yield return waitAutoDestroyDelay;

                // 리스트가 늘어났거나 비활성화된 오브젝트가 최소 풀 사이즈보다 작을 때는 넘김
                if (hasListIncreased || countInactive <= minPoolSize)
                {
                    if (pooledObjectData.shouldDestroyWithLerp)
                    {
                        StopAndResetDestroyCoroutine();
                    }

                    continue;
                }

                // 지우고 있을 땐 넘기기
                if (stillDestroy)
                    continue;

                // 지울 오브젝트 리스트 갱신
                destroyList.Clear();
                destroyList.AddRange(pool.Where(_obj => _obj.CanGet).Take(countInactive / 10));

                // 지워야하니 가져가지 못하게 해줌
                destroyList.ForEach(_obj => _obj.CanGet = false);

                if (pooledObjectData.shouldDestroyWithLerp)
                {
                    if (destroyPoolCoroutine == null)
                    {
                        stillDestroy = true;

                        destroyPoolCoroutine = MonoMethods.Start(LerpDestroyCoroutine());
                        print("자동 지우기 코루틴 시작");
                    }
                }
                else
                {
                    Destroy(countInactive / 10);
                }
            }
        }

        /// <summary>
        /// Lerp로 지우는 코루틴<br></br>
        /// <see cref="destroyList"/>에 있는 오브젝트를 Lerp로 지움
        /// </summary>
        private IEnumerator LerpDestroyCoroutine()
        {
            yield return waitDelayBeforePossiblePoolIncrease;

            int destroySize;
            int removeCount = 0;

            while (destroyList.Count > 0)
            {
                destroySize = Mathf.Min(pooledObjectData.SizeToDestroy, destroyList.Count);

                // destroySize만큼 지워주고
                for (int i = 0; i < destroySize; i++)
                {
                    DestroyObject(destroyList[i]);
                }

                // 지운거는 없애주기
                destroyList.RemoveRange(0, destroySize);

                removeCount += destroySize;

                yield return waitLerpDestroyDelay;
            }

            StopAndResetDestroyCoroutine();
        }

        /// <summary> 
        /// <see cref="LerpDestroyCoroutine"/> 초기화해주는 작업 
        /// </summary>
        private void StopAndResetDestroyCoroutine()
        {
            if (destroyPoolCoroutine != null)
            {
                MonoMethods.Stop(destroyPoolCoroutine);
                destroyPoolCoroutine = null;

                // 멈췄으면 다시 가져갈 수 있게 만들어줌
                destroyList.ForEach(_obj => _obj.CanGet = true);
                destroyList.Clear();
            }

            stillDestroy = false;
        }

        /// <see cref="IGetObjectPool{T}.Get(int)"/>
        public T Get(int hashCode = default)
        {
            T obj = pool.FirstOrDefault(_obj => _obj.CanGet);

            hasListIncreased = false;

            if (obj == null)
            {
                obj = createObject(this);
                pool.Add(obj);

                hasListIncreased = true;
            }

            countActive++;
            onGet?.Invoke(obj);

            obj.CanGet = false;
            obj.CalledObjectHashCode = hashCode;

            return obj;
        }


        /// <see cref="IGetObjectPool{T}.Get(Transform, int)"/>
        public T Get(Transform parent, int hashCode = default)
        {
            var obj = Get(hashCode);
            obj.SetParent(parent);

            return obj;
        }

        /// <see cref="IReleaseObjectPool{T}.Release"/>
        public void Release(T obj)
        {
            countActive--;

            obj.SetParent(pooledObjectData.poolParent);
            obj.CanGet = true;
            obj.CalledObjectHashCode = default;

            onRelease?.Invoke(obj);
        }

        /// <see cref="IReleaseObjectPool{T}.ReleaseNextFrame(T)"/>
        public void ReleaseNextFrame(T obj)
        {
            MonoMethods.Start(ReleasNextFrameCoroutine(obj));
        }

        // 꺼지는 중이니 다음 프레임에 실행시킴
        private IEnumerator ReleasNextFrameCoroutine(T obj)
        {
            yield return null;

            Release(obj);
        }

        /// <summary>
        /// 인자로 넣은 size와 현재 비활성화된 오브젝트 갯수와 비교해서<br></br>
        /// 작은갯수만큼 <see cref="destroyList"/>에서 지워줌
        /// </summary>
        public void Destroy(int size = 1)
        {
            destroyList.Clear();
            destroyList.AddRange(pool.Where(_obj => _obj.CanGet).Take(Mathf.Min(size, countInactive)));

            Clear();
        }

        /// <summary>
        /// <see cref="destroyList"/>에 있는걸 다 지워주는 작업
        /// </summary>
        private void Clear()
        {
            var destroySize = Mathf.Min(destroyList.Count, countInactive);

            for (int i = 0; i < destroySize; i++)
            {
                DestroyObject(destroyList[i]);
            }
        }

        /// <summary>
        /// 오브젝트를 지울 때 해야할 작업들
        /// </summary>
        private void DestroyObject(T obj)
        {
            onDestroy?.Invoke(obj);
            pool.Remove(obj);
            UnityEngine.Object.Destroy(obj.gameObject);
        }

        // Debug.Log 쓰기 귀찮아서 Mono에 있는 print처럼 쓰려고 만듦
        private void print(object message)
        {
            Debug.Log(message);
        }
    }
}