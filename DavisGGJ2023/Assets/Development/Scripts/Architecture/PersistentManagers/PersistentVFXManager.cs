using System;
using System.Collections.Generic;
using UnityEngine;

public class PersistentVFXManager : MonoBehaviour
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private PlayVFXFuncChannelSO askGetPlayVFX;
    [SerializeField] private PlayVFXFuncChannelSO askGetStopVFX;
    [SerializeField] private VoidEventChannelSO killAllVFX;


    private Dictionary<string, List<VFXEffect>> effectInstanceDict = new();
    
    private void OnEnable()
    {
        askGetPlayVFX.OnCalled += PlayVFX;
        askGetPlayVFX.OnCalled += StopVFX;
        killAllVFX.OnRaised += DisposeAllVFX;
    }
    
    private void OnDisable()
    {
        askGetPlayVFX.OnCalled -= PlayVFX;
        askGetPlayVFX.OnCalled -= StopVFX;
        killAllVFX.OnRaised -= DisposeAllVFX;
    }

    private void DisposeAllVFX()
    {
        foreach (var pair in effectInstanceDict)
        {
            var effectList = pair.Value;
            foreach(var effect in effectList)
                effect.DisposeEffect();
            effectList.Clear();
        }
        effectInstanceDict.Clear();
    }

    private void Update()
    {
        foreach (var pair in effectInstanceDict)
        {
            var effectList = pair.Value;
            for (int i = 0; i < effectList.Count; i++)
            {
                effectList[i].UpdateVFX();
                if (!effectList[i].PollActive())
                {
                    // Dispose of the effect
                    effectList[i].DisposeEffect();
                    effectList.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    private VFXEffect StopVFX(VFXEffectProfile profile, int id, PlayVFXSettings playVFXSettings)
    {
        string effectname = profile.name + id;
        VFXEffect res = null;
        if (effectInstanceDict.TryGetValue(effectname, out var list))
        {
            foreach (var effect in list)
            {
                effect.StopEffect();
                res = effect;
            }
        }

        return res;
    }

    private VFXEffect PlayVFX(VFXEffectProfile profile, int id, PlayVFXSettings playVFXSettings)
    {
        string effectname = profile.name + id;
        VFXEffect createdEffect = null;
        createdEffect = CreateVFX(profile.VFX);
        if (effectInstanceDict.TryGetValue(effectname, out var list))
        {
            list.Add(createdEffect);
        }
        else
        {
            effectInstanceDict.Add(effectname, new List<VFXEffect>());
            effectInstanceDict[effectname].Add(createdEffect);
        }
        createdEffect.PlayEffect(profile, playVFXSettings);
        return createdEffect;
    }

    private VFXEffect CreateVFX(VFXEffect prefab)
    {
        var res = Instantiate(prefab);
        return res;
    }
}
