using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MYTween
{
    public class MyTweenComponent : MonoBehaviour
    {
        private void LateUpdate()
        {
            TweenManager.Instance.UpdateTweens(UpdateMode.LateUpdate, Time.deltaTime);
        }

        private void FixedUpdate()
        {
            TweenManager.Instance.UpdateTweens(UpdateMode.FixedUpdate, Time.deltaTime);
        }

        private void Update()
        {
            TweenManager.Instance.UpdateTweens(UpdateMode.Update, Time.deltaTime);
        }

        /// <summary>
        /// MyTweenComponent 오브젝트 생성
        /// </summary>
        internal static void Create()
        {
            if (MyTween.TweenComponent != null)
            {
                return;
            }

            GameObject gameObject = new GameObject("[MYTween]");
            DontDestroyOnLoad(gameObject);
            MyTween.TweenComponent = gameObject.AddComponent<MyTweenComponent>();
        }
    }
}
