using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MYTween
{
    [Serializable]
    /// <summary>
    /// TweenCore와 Sequence가 상속받음
    /// </summary>
    public abstract class Tween
    {
        [Tooltip("끝낸 횟수\nDefault : 0")]
        internal int completedLoops;

        [Tooltip("반복할 횟수\nDefault : 1")]
        internal int loops;

        internal bool isComplete;

        internal abstract void ApplyTween();

        internal abstract void Kill();
    }
}
