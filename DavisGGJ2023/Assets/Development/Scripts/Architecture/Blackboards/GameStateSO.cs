using System.Collections.Generic;
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

    public int CurrentEnemyCount
    {
        get => currentEnemyCount;
        set => currentEnemyCount = value;
    }

    public int StatuesDestroyed
    {
        get => statuesDestroyed;
        set => statuesDestroyed = value;
    }

    [SerializeField, ReadOnly] private GameLevelSO currentlyLoadedLevel = null;
    [SerializeField, ReadOnly] private LevelEntranceSO targetEntrance;
    [SerializeField] private int currentEnemyCount;
    [SerializeField] private int statuesDestroyed;
    
    private HashSet<GameLevelSO> clearedRooms = new();

    public bool IsCurrentLevelCleared()
    {
        return clearedRooms.Contains(currentlyLoadedLevel);
    }

    public void LevelCleared(GameLevelSO room)
    {
        clearedRooms.Add(room);
    }

    public void ResetValues()
    {
        targetEntrance = null;
        currentlyLoadedLevel = null;
        entryGameLevel = null;
        CurrentEnemyCount = 0;
        StatuesDestroyed = 0;
        clearedRooms = new();
    }
}
