using UnityEngine;

namespace MyObjectPool
{
    public interface IGetObjectPool<T> where T : MonoBehaviour
    {
        /// <see cref="ObjectPool{T}.Get(int)"/>
        T Get(int hashCode = default);

        /// <see cref="ObjectPool{T}.Get(Transform, int)"/>
        T Get(Transform parent, int hashCode = default);
    }

    public interface IReleaseObjectPool<T> where T : MonoBehaviour
    {
        /// <see cref="ObjectPool{T}.Release"/>
        void Release(T obj);

        /// <summary>
        /// Get할 때 <see cref="ObjectPool{T}.Get(Transform, int)"/>를 사용했다면 이 함수를 사용
        /// </summary>
        /// <see cref="ObjectPool{T}.ReleaseNextFrame(T)"/>
        void ReleaseNextFrame(T obj);
    }
}