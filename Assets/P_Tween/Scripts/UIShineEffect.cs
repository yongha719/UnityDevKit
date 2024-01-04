using MYTween;
using NaughtyAttributes;
using UnityEngine;

public enum ShineDirection
{
    LeftToRight,
    RightToLeft
}


[AddComponentMenu("UI/Shine Effect")]
public class UIShineEffect : MonoBehaviour
{
    private GameObject shineEffectPrefab;

    private RectTransform shineEffect;

    private Vector2 startPos;
    private Vector2 endPos;

    [SerializeField]
    private ShineDirection shineDirection;

    [SerializeField, Range(0, 360f)]
    private float rotateValue;

    [SerializeField]
    private float effectWidth;

    [SerializeField]
    private float duration = 2f;

    private Tweener shineTween;

    private void Start()
    {
        Init();

        Shine(duration, effectWidth, shineDirection);
    }

    private void Init()
    {
        shineEffectPrefab = Resources.Load<GameObject>("Prefabs/ShineEffect");
        shineEffectPrefab = Instantiate(shineEffectPrefab, transform);
        shineEffectPrefab.name = shineEffectPrefab.name.Replace("(Clone)", null);

        shineEffect = shineEffectPrefab.transform.GetChild(0) as RectTransform;

        SetShineEffect(shineDirection);
        shineEffect.anchoredPosition = startPos;

        // 크기 설정
        shineEffect.sizeDelta = new Vector2(effectWidth, Vector2.Distance(startPos, endPos) * 2.5f);
        shineEffect.rotation = Quaternion.Euler(0, 0, rotateValue);

        shineEffectPrefab.SetActive(false);
    }

    [Button]
    private void TestActiveFalse()
    {
        shineEffectPrefab.SetActive(false);
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void TestShine()
    {
        shineEffect.rotation = Quaternion.Euler(0, 0, rotateValue);

        Shine(duration, effectWidth, shineDirection);
    }

    public void Shine(float duration, float width = 10f, ShineDirection dir = ShineDirection.LeftToRight)
    {
        shineTween?.Kill();

        SetShineEffect(dir);
        shineEffect.sizeDelta = new Vector2(width, Vector2.Distance(startPos, endPos) * 2.5f);

        shineEffectPrefab.SetActive(true);

        shineTween = shineEffect.AnchorPos(endPos, duration).OnComplete(() =>
        {
            shineEffectPrefab.SetActive(false);
        }).OnKill(() => shineEffectPrefab.SetActive(false));
    }

    /// <summary>
    /// Shine 방향 설정과 위치 설정
    /// </summary>
    /// <param name="shineDirection"></param>
    private void SetShineEffect(ShineDirection shineDirection)
    {
        var shineEffectPrefabRect = (RectTransform)transform;

        float width = shineEffectPrefabRect.rect.width;
        float height = shineEffectPrefabRect.rect.height;

        if (shineDirection == ShineDirection.LeftToRight)
        {
            startPos = new Vector2(-width / 2, height / 2);
            endPos = new Vector2(width / 2, -height / 2);
        }
        else if (shineDirection == ShineDirection.RightToLeft)
        {
            startPos = new Vector2(width / 2, height / 2);
            endPos = new Vector2(-width / 2, -height / 2);
        }

        shineEffect.anchoredPosition = startPos;
    }

    private void OnDisable()
    {

        TweenManager.Instance.RemoveActiveTween(shineTween);
    }
}
