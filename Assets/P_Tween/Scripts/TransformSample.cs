using Coffee.UIParticleExtensions;
using MYTween;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class TransformSample : MonoBehaviour
{
    private Tweener moveTween;

    private List<Tweener> tweeners = new List<Tweener>();

    [SerializeField]
    private float duration;

    ParticleSystem ParticleSystem;

    void Start()
    {
        Tweener scaleTweener = transform.Scale(transform.localScale * 2f, duration).OnComplete(() => print("Scale 끝"));
        Tweener.AddTweener(scaleTweener, tweeners);

        Tweener moveTweener = transform.Move(transform.position + Vector3.down, duration).OnComplete(() => print("Move 끝"));
        Tweener.AddTweener(moveTweener, tweeners);

        Tweener rotateTweener = transform.MyRotate(new Vector3(0, 0, 90), duration).OnComplete(() => print("Rotate 끝"));
        Tweener.AddTweener(rotateTweener, tweeners);
    }

    [Button]
    private void TestMove()
    {
        moveTween?.Kill();
        transform.position = Vector3.zero;
        gameObject.SetActive(true);

        moveTween = transform.Move(transform.position + Vector3.down, 1f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            print("Move 끝");
        });
        Tweener.AddTweener(moveTween, tweeners);
    }

    private void OnDisable()
    {
        if (TweenManager.Instance != null)
        {
            TweenManager.Instance.RemoveTweens(tweeners, true);
        }
    }
}
