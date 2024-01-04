using UnityEngine;
using UnityEngine.UI;

namespace MYTween
{
    public enum UpdateMode
    {
        Update,
        LateUpdate,
        FixedUpdate,
    }

    public static class MyTween
    {
        public static MyTweenComponent TweenComponent;

        private static bool initialized;

        /// <summary>
        /// 초기화되었는지 체크
        /// </summary>
        public static void InitCheck()
        {
            if (initialized || Application.isPlaying == false)
            {
                return;
            }

            Init();
        }

        private static void Init()
        {
            initialized = true;

            MyTweenComponent.Create();
        }

        public static TweenerCore<Color, Color> Fade(this Image image, float endValue, float duration, UpdateMode updateMode = UpdateMode.LateUpdate)
        {
            return ApplyTo(image.gameObject.name,
                getter: () => image.color,
                setter: (value) => image.color = value, new Color(0f, 0f, 0f, endValue),
                duration, updateMode);
        }

        public static TweenerCore<Color, Color> Fade(this SpriteRenderer spriteRender, float endValue, float duration, UpdateMode updateMode = UpdateMode.LateUpdate)
        {
            return ApplyTo(spriteRender.gameObject.name,
                getter: () => spriteRender.color,
                setter: (value) => spriteRender.color = value, new Color(0f, 0f, 0f, endValue),
                duration, updateMode);
        }

        /// <summary>
        /// 이미지 2개 Fade In Out
        /// </summary>
        /// <param name="positiveImage">알파값 증가시킬 Image</param>
        /// <param name="negativeImage">알파값 감소시킬 Image</param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static (TweenerCore<Color, Color> positiveImageTween, TweenerCore<Color, Color> negativeImageTween) FadeInOut(
            Image positiveImage, Image negativeImage, float duration, UpdateMode updateMode = UpdateMode.LateUpdate)
        {
            // Alpha값 0으로 만들기
            positiveImage.color -= Color.black;
            TweenerCore<Color, Color> positiveImageTween = positiveImage.Fade(1, duration,updateMode);

            // Alpha값 1로 만들기
            negativeImage.color += Color.black;
            TweenerCore<Color, Color> negativeImageTween = negativeImage.Fade(0, duration, updateMode);

            return (positiveImageTween, negativeImageTween);
        }

        public static TweenerCore<Vector3, Vector3> Scale(this Transform transform, Vector3 endValue, float duration, UpdateMode updateMode = UpdateMode.Update)
        {
            return ApplyTo(transform.gameObject.name,
                getter: () => transform.localScale,
                setter: (value) => transform.localScale = value, endValue,
                duration, updateMode);
        }

        public static TweenerCore<Vector3, Vector3> Move(this Transform transform, Vector3 endValue, float duration, UpdateMode updateMode = UpdateMode.Update)
        {
            return ApplyTo(transform.gameObject.name,
                getter: () => transform.position,
                setter: (value) => transform.position = value, endValue,
                duration, updateMode);
        }

        public static TweenerCore<Quaternion, Vector3> MyRotate(this Transform transform, Vector3 endValue, float duration, UpdateMode updateMode = UpdateMode.Update)
        {
            return ApplyTo(transform.gameObject.name,
                getter: () => transform.rotation,
                setter: (value) => transform.rotation = value, endValue,
                duration, updateMode);
        }

        public static TweenerCore<Vector2, Vector2> AnchorPos(this RectTransform rectTransform, Vector2 endValue, float duration, UpdateMode updateMode = UpdateMode.LateUpdate)
        {
            return ApplyTo(rectTransform.gameObject.name,
                getter: () => rectTransform.anchoredPosition,
                setter: (value) => rectTransform.anchoredPosition = value, endValue,
                duration, updateMode);
        }

        /// <summary>
        /// Tweener 셋팅
        /// </summary>
        /// <param name="getter">시작 값만 가져옴</param>
        /// <param name="setter">값 세팅할 때 쓸 액션 </param>
        private static TweenerCore<T1, T2> ApplyTo<T1, T2>(string objectName, Getter<T1> getter, Setter<T1> setter, T2 endValue, float duration, UpdateMode updateMode)
        {
            InitCheck();
            TweenerCore<T1, T2> tween = TweenManager.Instance.GetTweener<T1, T2>(updateMode);

            tween.SetUp(objectName, getter, setter, endValue, duration);
            return tween;
        }

        public static Tweener OnComplete(this Tweener tween, TweenEvent tweenEvent)
        {
            tween.onComplete += tweenEvent;

            return tween;
        }

        public static Tweener OnKill(this Tweener tweener, TweenEvent tweenEvent)
        {
            tweener.onKill += tweenEvent;

            return tweener;
        }
    }
}
