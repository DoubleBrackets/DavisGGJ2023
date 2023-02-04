using UnityEngine;

[CreateAssetMenu(menuName = "Architecture/Blackboards/GameStateSO")]
public class GameStateSO : DescriptionBaseSO
{
    [SerializeField, ReadOnly] GameLevelSO entryGameLevel = null;

    public GameLevelSO EntryGameLevel
    {
        set => entryGameLevel = value;
        get => entryGameLevel;
    }

    public GameLevelSO CurrentlyLoadedLevel
    {
        get => currentlyLoadedLevel;
        set => currentlyLoadedLevel = value;
    }

    public LevelEntranceSO TargetEntrance
    {
        get => targetEntrance;
        set => targetEntrance = value;
    }

    [SerializeField, ReadOnly] private GameLevelSO currentlyLoadedLevel = null;
    [SerializeField, ReadOnly] private LevelEntranceSO targetEntrance;

}
