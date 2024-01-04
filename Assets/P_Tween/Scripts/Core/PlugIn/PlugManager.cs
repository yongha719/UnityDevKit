using DG.Tweening.Plugins.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MYTween
{
    public static class PlugManager
    {
        private static TweenPlugin<Color, Color> colorPlugin;

        private static TweenPlugin<Vector3, Vector3> vector3Plugin;

        private static TweenPlugin<Vector2, Vector2> vector2Plugin;

        private static TweenPlugin<Quaternion, Vector3> rotatePlugin;

        /// <summary>
        /// 타입에 맞는 Plugin 반환
        /// </summary>
        internal static TweenPlugin<T1, T2> GetPlugin<T1, T2>()
        {
            Type type1 = typeof(T1);
            Type type2 = typeof(T2);

            if (type1 == typeof(Quaternion) && type2 == typeof(Vector3))
            {
                if (rotatePlugin == null)
                {
                    rotatePlugin = new QuaternionVector3Plugin();
                }

                return rotatePlugin as TweenPlugin<T1, T2>;
            }

            if (type1 == typeof(Color))
            {
                if (colorPlugin == null)
                {
                    colorPlugin = new ColorPlugin();
                }

                return colorPlugin as TweenPlugin<T1, T2>;
            }

            if (type1 == typeof(Vector3))
            {
                if (vector3Plugin == null)
                {
                    vector3Plugin = new Vector3Plugin();
                }

                return vector3Plugin as TweenPlugin<T1, T2>;

            }

            if (type1 == typeof(Vector2))
            {
                if (vector2Plugin == null)
                {
                    vector2Plugin = new Vector2Plugin();
                }

                return vector2Plugin as TweenPlugin<T1, T2>;

            }
            
            throw new NotImplementedException($"타입에 맞는 플러그인이 없음\nType 1 : {type1} Type2 : {type2}");
        }
    }
}
