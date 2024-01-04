using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MonoMethods : MonoBehaviour
{
    private static MonoMethods monoMethods;

    private static ConditionalWeakTable<IEnumerator, Coroutine> coroutinesTable = new ConditionalWeakTable<IEnumerator, Coroutine>();

    private void Awake()
    {
        monoMethods = this;
    }

    public static IEnumerator? Start(IEnumerator? coroutine)
    {
        if (coroutine != null)
        {
            coroutinesTable.Add(coroutine, monoMethods.StartCoroutine(coroutine));
        }

        return coroutine;
    }

    public static void Stop(IEnumerator? coroutine)
    {
        if (coroutine != null && coroutinesTable.TryGetValue(coroutine, out var routine))
        {
            monoMethods.StopCoroutine(routine);
        }
    }
}


