using System;
using UnityEngine;

/// <summary>
/// Pass in game level SO to load.
/// Specify transitions in and out
/// Returns whether or not the scene can be loaded
/// </summary>
[CreateAssetMenu(menuName = "Channels/Funcs/LoadGameLevelFuncChannel", fileName = "NewLoadGameLevelFuncChannel")]
public class LoadGameLevelFuncChannelSO : GenericFuncChannelSO<GameLevelSO, TransitionEffect, TransitionEffect,  bool>
{
    public override bool CallFunc(GameLevelSO t1, TransitionEffect transitionOut, TransitionEffect transitionIn)
    {
        return base.CallFunc(t1, transitionOut, transitionIn);
    }
}
