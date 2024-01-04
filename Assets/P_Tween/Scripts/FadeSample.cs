using MYTween;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeSample : MonoBehaviour
{
    [SerializeField]
    private Image positiveImage;
    [SerializeField]
    private Image negativeImage;

    [SerializeField]
    private float duration;

    [SerializeField, Range(0, 1)]
    private int grayScaleValue;

    private List<Tweener> tweeners = new List<Tweener>();

    void Start()
    {
        positiveImage.color -= Color.black;
        TweenerCore<Color, Color> ptween = positiveImage.Fade(1, duration);
        negativeImage.color += Color.black;
        TweenerCore<Color, Color> ntween = negativeImage.Fade(0, duration);

        Tweener.AddTweener(ptween, tweeners);
        Tweener.AddTweener(ntween, tweeners);
    }

    [Button]
    public void SetGrayScale()
    {
        GrayScale grayScale = grayScaleValue == 1 ? GrayScale.On : GrayScale.Off;
        positiveImage.SetGrayScale(grayScale);
    }

    private void OnDisable()
    {
        if (TweenManager.Instance != null)
        {
            TweenManager.Instance.RemoveTweens(tweeners, true);
        }
    }
}
