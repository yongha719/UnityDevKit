using System.Collections.Generic;
using UnityEngine;

namespace MYTween
{
    public class TweenManager : Singleton<TweenManager>
    {
        private List<Tweener> updateActiveTweens = new List<Tweener>(20);

        private List<Tweener> lateUpdateActiveTweens = new List<Tweener>(20);

        private List<Tweener> fixedUpdateActiveTweens = new List<Tweener>(20);

        private List<Tweener> tweenerPool = new List<Tweener>(20);

        [SerializeField, Tooltip("인스펙터 확인용")]
        private int activeTweensCount;

        [SerializeField, Tooltip("인스펙터 확인용")]
        private int tweenerPoolCount;

        protected override bool dontDestroy => true;

        /// <summary>
        /// TweenerCore 가져오고 Update할 리스트에 추가
        /// </summary>
        public TweenerCore<T1, T2> GetTweener<T1, T2>(UpdateMode updateMode)
        {
            if (tweenerPool.Count > 0)
            {
                for (int i = 0; i < tweenerPool.Count; i++)
                {
                    // Tweener지만 실제론 TweenerCore<,>이 담겨있어서 안쓰는 애들중에 타입에 맞는걸 가져옴
                    if (tweenerPool[i] != null && tweenerPool[i].typeT1 == typeof(T1) && tweenerPool[i].typeT2 == typeof(T2))
                    {
                        AddActiveTween(tweenerPool[i], updateMode);
                        tweenerPool.RemoveAt(i);
                        tweenerPoolCount--;

                        return (TweenerCore<T1, T2>)tweenerPool[i];
                    }

                }
            }

            // 맞는 타입이 없다면 생성
            TweenerCore<T1, T2> tweener = new TweenerCore<T1, T2>();
            AddActiveTween(tweener, updateMode);

            return tweener;
        }

        /// <summary>
        /// Update해줄 Tween List에 추가
        /// </summary>
        private void AddActiveTween(Tweener tweener, UpdateMode updateMode)
        {
            tweener.Reset();

            List<Tweener> tweeners = GetActiveTweens(updateMode);

            if (tweeners != null)
            {
                tweeners.Add(tweener);
                activeTweensCount++;
            }
        }

        /// <summary>
        /// 끝난 Tween들 리스트에서 제거하고 풀에 넣어줌
        /// </summary>
        /// <param name="tweener"></param>
        public void RemoveActiveTween(Tweener tweener, bool shouldGoToOriginValue = false)
        {
            if (tweener != null && updateActiveTweens.Contains(tweener))
            {
                if (shouldGoToOriginValue)
                {
                    tweener.SetOriginValue();
                }

                tweener.onKill?.Invoke();

                updateActiveTweens.Remove(tweener);
                activeTweensCount--;

                tweenerPool.Add(tweener);
                tweenerPoolCount++;
            }
        }

        /// <summary>
        /// Active Tweens 에서 지워줌
        /// </summary>
        /// <param name="shouldGoToOriginValue">제거될 때 다시 원래 값으로 돌려야 하는 경우</param>
        public void RemoveTweens(List<Tweener> tweens, bool shouldGoToOriginValue = false)
        {
            int tweensCount = tweens.Count - 1;

            for (int i = tweensCount; i >= 0; i--)
            {
                RemoveActiveTween(tweens[i], shouldGoToOriginValue);
            }
        }

        /// <summary>
        /// <see cref="MyTweenComponent.Update"/>에서 호출됨<br></br>
        /// Tween Update
        /// </summary>
        public void UpdateTweens(UpdateMode updateMode, float _deltaTime)
        {
            List<Tweener> tweens = GetActiveTweens(updateMode);

            for (int i = 0; i < tweens.Count; i++)
            {
                Tweener tweener = tweens[i];

                if (tweener != null)
                {
                    tweener.UpdateTime(_deltaTime);
                }
            }
        }

        private List<Tweener> GetActiveTweens(UpdateMode updateMode)
        {
            List<Tweener> resultTweens = null;

            if (updateMode == UpdateMode.Update)
            {
                resultTweens = updateActiveTweens;
            }
            else if (updateMode == UpdateMode.LateUpdate)
            {
                resultTweens = lateUpdateActiveTweens;
            }
            else if (updateMode == UpdateMode.FixedUpdate)
            {
                resultTweens = fixedUpdateActiveTweens;
            }

            if (resultTweens == null)
            {
                Debug.Assert(false, $"UpdateMode에 맞는 Tween이 없음 : {updateMode}");
            }

            return resultTweens;
        }
    }
}
