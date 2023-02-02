using UnityEngine;

public class SimpleLoadLevel : DescriptionMonoBehavior
{
    [SerializeField] private LoadGameLevelFuncChannelSO askLoadGameLevel;
    [SerializeField] private GameLevelSO entryLevel;

    public void Load()
    {
        askLoadGameLevel.CallFunc(entryLevel, TransitionEffect.FadeBlack, TransitionEffect.FadeBlack);
    }
}
