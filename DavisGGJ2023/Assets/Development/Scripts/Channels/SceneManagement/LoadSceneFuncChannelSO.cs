using System;
using UnityEngine;

/// <summary>
/// Pass in name of scene to load and the scene layer to load it on.
/// Returns whether or not the scene can be loaded. Should not be used for game levels,
/// <see cref="LoadGameLevelFuncChannelSO"/>
/// </summary>
[CreateAssetMenu(menuName = "Channels/Funcs/LoadSceneFuncChannel", fileName = "NewLoadSceneFuncChannel")]
public class LoadSceneFuncChannelSO : GenericFuncChannelSO<string, int, TransitionEffect, TransitionEffect, bool>
{
    public override bool CallFunc(string sceneName, int sceneLayer, TransitionEffect transitionOut, TransitionEffect transitionIn)
    {
        return base.CallFunc(sceneName, sceneLayer, transitionOut, transitionIn);
    }
}
