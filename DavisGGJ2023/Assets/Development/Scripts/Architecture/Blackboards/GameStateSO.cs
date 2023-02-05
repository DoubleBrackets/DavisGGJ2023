using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
        set
        {
            currentEnemyCount = value;
            currentEnemyCountDebug = value;
        }
    }

    public ProtagBlackboard ProtagBlackboard
    {
        get => protagBlackboard;
        set => protagBlackboard = value;
    }

    private GameLevelSO currentlyLoadedLevel = null;
    private LevelEntranceSO targetEntrance = null;
    private int currentEnemyCount;
    private bool[] statuesDestroyed;
    private ProtagBlackboard protagBlackboard;
    
    private HashSet<GameLevelSO> clearedRooms = new();

    private Dictionary<string, bool> flags = new();

    // Display
    [ReadOnly] public int currentEnemyCountDebug;
    [ReadOnly] public bool[] statuesDestroyedDebug;

    public bool IsCurrentLevelCleared()
    {
        return clearedRooms.Contains(currentlyLoadedLevel);
    }

    public bool GetFlag(string id)
    {
        if (flags.ContainsKey(id))
            return flags[id];
        else
        {
            return false;
        }
    }

    public void SetFlag(string id, bool flag)
    {
        flags.TryAdd(id, flag);
        flags[id] = flag;
    }
    
    public void ResetValues()
    {
        targetEntrance = null;
        currentlyLoadedLevel = null;
        entryGameLevel = null;
        CurrentEnemyCount = 0;
        clearedRooms = new();
        flags = new();
        statuesDestroyed = null;
    }

    public void LevelCleared(GameLevelSO room)
    {
        clearedRooms.Add(room);
    }

    public bool IsStatueDestroyed(int index)
    {
        return statuesDestroyed[index];
    }

    public void SetStatueDestroyed(int index, bool val)
    {
        statuesDestroyed[index] = val;
        statuesDestroyedDebug = statuesDestroyed;
    }
}
