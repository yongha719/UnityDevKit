using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyObjectPool
{
    public interface IPooledObject<T> where T : MonoBehaviour
    {
        bool CanGet { get; set; }

        int CalledObjectHashCode { get; set; }

        IReleaseObjectPool<T> Pool { get; set; }
    }
}