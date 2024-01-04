using Coffee.UIExtensions;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttractorParticleUI : MonoBehaviour
{
    [SerializeField]
    private UIParticleAttractor coinParticleAttractor;

    [SerializeField]
    private Text coinText;

    private int coinCount;

    private void Start()
    {
        coinParticleAttractor.onAttracted.AddListener(() =>
        {
            coinCount++;
            coinText.text = coinCount.ToString();
        });
    }

    [Button]
    private void TestEmitParticle()
    {
        EmitParticle(5);
    }

    public void EmitParticle(int count)
    {
        coinParticleAttractor.particleSystem.Emit(count);
    }

    public void Stop()
    {
        coinParticleAttractor.particleSystem.Stop();
    }
}

