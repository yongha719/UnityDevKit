using System;
using UnityEngine.Experimental.AI;

namespace MYTween
{
    public delegate T Getter<out T>();

    public delegate void Setter<in T>(T value);

    [Serializable]
    public class TweenerCore<T1, T2> : Tweener
    {
        private T1 StartValueT1;

        internal T2 StartValueT2;

        internal T2 EndValue;

        internal T2 ChangeValue;

        internal Getter<T1> Getter;

        internal Setter<T1> Setter;

        internal TweenPlugin<T1, T2> tweenPlugin;

        public void SetUp(string name, Getter<T1> _getter, Setter<T1> _setter, T2 _endValue, float _duration)
        {
            if (tweenPlugin == null)
            {
                tweenPlugin = PlugManager.GetPlugin<T1, T2>();
            }

            objectName = name;

            Getter = _getter;
            Setter = _setter;
            EndValue = _endValue;
            Duration = _duration;

            StartValueT1 = Getter();
            StartValueT2 = tweenPlugin.ConvertToStartValue(this, Getter());
            tweenPlugin.SetChangeValue(this);
        }

        /// <summary>
        /// Update될 때 호출
        /// </summary>
        internal override void ApplyTween()
        {
            // 타입에 맞게 연산해야해서 넘겨줌
            tweenPlugin.EvaluateAndApply(this, Getter, Setter, ElapsedTime, StartValueT2, ChangeValue, Duration);
        }

        internal override void SetOriginValue()
        {
            Setter(StartValueT1);
        }
    }
}
