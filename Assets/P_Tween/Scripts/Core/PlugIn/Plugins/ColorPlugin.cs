using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MYTween
{
    public class ColorPlugin : TweenPlugin<Color, Color>
    {
        public override Color ConvertToStartValue(TweenerCore<Color, Color> t, Color value)
        {
            return value;
        }

        /// <summary>
        /// Ease 계산후 tween에 적용
        /// </summary>
        public override void EvaluateAndApply(Tween t, Getter<Color> getter, Setter<Color> setter, float elapsed, Color startValue, Color changeValue, float duration)
        {
            float linear = elapsed / duration;

            startValue.a += changeValue.a * linear;
            setter(startValue);
        }

        public override void SetChangeValue(TweenerCore<Color, Color> tween)
        {
            tween.ChangeValue = tween.EndValue - tween.StartValueT2;
        }
    }
}
