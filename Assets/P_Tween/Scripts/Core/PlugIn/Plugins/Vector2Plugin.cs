using UnityEngine;

namespace MYTween
{

    public class Vector2Plugin : TweenPlugin<Vector2, Vector2>
    {
        public override Vector2 ConvertToStartValue(TweenerCore<Vector2, Vector2> t, Vector2 value)
        {
            return value;
        }

        public override void SetChangeValue(TweenerCore<Vector2, Vector2> tween)
        {
            tween.ChangeValue = tween.EndValue - tween.StartValueT2;
        }

        public override void EvaluateAndApply(Tween t, Getter<Vector2> getter, Setter<Vector2> setter, float elapsed, Vector2 startValue, Vector2 changeValue, float duration)
        {
            float linear = elapsed / duration;

            startValue += changeValue * linear;

            setter(startValue);
        }
    }
}
