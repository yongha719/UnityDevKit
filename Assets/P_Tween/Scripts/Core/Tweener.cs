using System;
using System.Collections.Generic;

namespace MYTween
{
    public delegate void TweenEvent();

    [Serializable]
    public abstract class Tweener : Tween
    {
        internal TweenEvent onComplete;

        internal TweenEvent onKill;

        public float Duration { get; internal set; }

        public float ElapsedTime { get; internal set; }

        // TweenCore의 타입
        internal Type typeT1;

        internal Type typeT2;

        internal string objectName;

        /// <summary>
        /// TweenManager에서 호출<br></br>
        /// Tween 업데이트
        /// </summary>
        public virtual void UpdateTime(float deltaTime)
        {
            ElapsedTime += deltaTime;

            if (ElapsedTime >= Duration)
            {
                completedLoops++;
                ElapsedTime -= Duration;
            }

            if (completedLoops == loops)
            {
                isComplete = true;

                onComplete?.Invoke();

                TweenManager.Instance.RemoveActiveTween(this);
                return;
            }

            ApplyTween();
        }

        internal override void Kill()
        {
            TweenManager.Instance.RemoveActiveTween(this);
        }

        /// <summary>
        /// List에 추가하고 이벤트에 삭제하는 작업을 등록<br></br>
        /// active false시에 Remove 해줘야하기 때문에 List에서 관리함
        /// </summary>
        /// <param name="tweener"></param>
        /// <param name="tweeners"></param>
        public static void AddTweener(Tweener tweener, List<Tweener> tweeners)
        {
            if (tweeners.Contains(tweener) == false)
            {
                tweeners.Add(tweener);

                tweener.OnKill(() =>
                {
                    tweeners.Remove(tweener);
                });

                tweener.OnComplete(() =>
                {
                    tweeners.Remove(tweener);
                });
            }
        }

        /// <summary>
        /// 원래 값으로 돌려줌
        /// </summary>
        internal abstract void SetOriginValue();

        public void Reset()
        {
            Duration = 0f;
            ElapsedTime = 0f;

            loops = 1;
            completedLoops = 0;

            onComplete = null;
            onKill = null;
        }
    }
}
