using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraRecomposeManager : MonoBehaviour
{
    public enum RecomposeEffect
    {
        JetpackShake,
        Landing,
        Footstep1,
        Footstep2,
        Jump
    }

    class RecomposeInstance
    {
        public CameraRecomposeProfileSO profile;
        public float currentTime;
        public float multiplier;
    }

    [ColorHeader("Dependencies")] 
    [SerializeField] private CinemachineRecomposer targetRecomposer;
    
    [ColorHeader("Listening - Ask Start/Stop Impulse Effects", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private RecomposeEffectEventChannelSO askStartEffect;
    
    [ColorHeader("Config", ColorHeaderColor.Config)]
    [SerializeField] private List<CameraRecomposeProfileSO> recomposerProfileList;

    private List<RecomposeInstance> currentRecomposers = new();

    private void Update()
    {
        ResetRecomposerComponent();
        for (int i = 0; i < currentRecomposers.Count; i++)
        {
            var instance = currentRecomposers[i];
            
            instance.currentTime += Time.deltaTime;
            float targetTime = instance.profile.Duration;
            float t = instance.currentTime / targetTime;

            if (t > 1)
            {
                currentRecomposers.RemoveAt(i);
                i--;
                continue;
            }
            
            currentRecomposers[i].profile.ApplyRecomposeEffect(targetRecomposer, t, instance.multiplier);
        }
    }

    private void ResetRecomposerComponent()
    {
        targetRecomposer.m_Tilt = 0;
        targetRecomposer.m_Pan = 0;
        targetRecomposer.m_Dutch = 0;
        targetRecomposer.m_ZoomScale = 1;
    }
    

    private void OnEnable()
    {
        askStartEffect.OnRaised += StartEffect;
    }

    private void OnDisable()
    {
        askStartEffect.OnRaised -= StartEffect;
    }

    private void StartEffect(RecomposeEffect effect, float multiplier)
    {
        var recomposeProfile = recomposerProfileList[(int)effect];
        currentRecomposers.Add(new RecomposeInstance
        {
            profile = recomposeProfile,
            currentTime = 0f,
            multiplier = multiplier
        });
    }
}