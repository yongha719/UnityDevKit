using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MYTween
{

    public class Vector3Plugin : TweenPlugin<Vector3, Vector3>
    {
        public override Vector3 ConvertToStartValue(TweenerCore<Vector3, Vector3> t, Vector3 value)
        {
            return value;
        }

        public override void SetChangeValue(TweenerCore<Vector3, Vector3> tween)
        {
            tween.ChangeValue = tween.EndValue - tween.StartValueT2;
        }

        // 나중에 Ease가 추가된다면 여기서 진행
        public override void EvaluateAndApply(Tween t, Getter<Vector3> getter, Setter<Vector3> setter, float elapsed, Vector3 startValue, Vector3 changeValue, float duration)
        {
            // ease linear
            float linear = elapsed / duration;

            startValue += changeValue * linear;

            setter(startValue);
        }
    }
}
