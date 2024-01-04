using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MYTween
{

    public class QuaternionVector3Plugin : TweenPlugin<Quaternion, Vector3>
    {
        public override Vector3 ConvertToStartValue(TweenerCore<Quaternion, Vector3> t, Quaternion value)
        {
            return value.eulerAngles;
        }

        public override void SetChangeValue(TweenerCore<Quaternion, Vector3> tween)
        {
            tween.ChangeValue = tween.EndValue - tween.StartValueT2;
        }

        public override void EvaluateAndApply(Tween t, Getter<Quaternion> getter, Setter<Quaternion> setter, float elapsed, Vector3 startValue, Vector3 changeValue, float duration)
        {
            float linear = elapsed / duration;

            startValue += changeValue * linear;

            setter(Quaternion.Euler(startValue));
        }
    }
}
