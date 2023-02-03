using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PersistentTimeManager : MonoBehaviour
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private FloatEventChannelSO askFreezeFrame;

    private List<TimeScaleModifier> currentTimeModifiers = new();

    private float baseFixedDeltaTime;
    
    class TimeScaleModifier
    {
        public float timeScale;
        public float remainingDuration;
    }

    private void OnEnable()
    {
        askFreezeFrame.OnRaised += FreezeFrame;
        baseFixedDeltaTime = Time.fixedDeltaTime;
    }
    
    private void OnDisable()
    {
        askFreezeFrame.OnRaised -= FreezeFrame;
    }
    
    private void Update()
    {
        float timeScale = 1f;
        for(int i = 0;i < currentTimeModifiers.Count;i++)
        {
            var modifier = currentTimeModifiers[i];
            
            modifier.remainingDuration -= Time.unscaledDeltaTime;
            
            if (modifier.remainingDuration <= 0f)
            {
                currentTimeModifiers.RemoveAt(i);
                i--;
            }
            else
            {
                timeScale *= modifier.timeScale;
            }
        }

        Time.timeScale = timeScale;
    }

    private void FreezeFrame(float duration)
    {
        var modifier = new TimeScaleModifier()
        {
            timeScale = 0f,
            remainingDuration = duration
        };
        
        currentTimeModifiers.Add(modifier);
    }
}
