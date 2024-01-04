using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace MYTween
{
    /// <summary>
    /// 제네릭이라 연산이 안되기도 하고 타입마다 연산하는 방법들이 다르기 때문에
    /// 타입별로 오버라이딩해서 만들어줘야함
    /// </summary>
    public abstract class TweenPlugin<T1, T2>
    {
        /// <summary>
        /// T1 to T2
        /// </summary>
        public abstract T2 ConvertToStartValue(TweenerCore<T1, T2> t, T1 value);

        /// <summary>
        /// Change값 셋팅해주는 함수
        /// </summary>
        /// <param name="tween"></param>
        public abstract void SetChangeValue(TweenerCore<T1, T2> tween);

        /// <summary>
        /// 실질적으로 Tween 값 업데이트 해주는 함수
        /// </summary>
        public abstract void EvaluateAndApply(Tween t, Getter<T1> getter, Setter<T1> setter, float elapsed, T2 startValue, T2 changeValue, float duration);
    }
}
