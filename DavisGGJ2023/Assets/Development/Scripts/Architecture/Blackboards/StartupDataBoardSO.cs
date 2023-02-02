using UnityEngine;

[CreateAssetMenu(menuName = "Architecture/Blackboards/StartupDataSO")]
public class StartupDataBoardSO : DescriptionBaseSO
{
    private GameLevelSO entryGameLevel = null;

    public GameLevelSO EntryGameLevel
    {
        set => entryGameLevel = value;
        get => entryGameLevel;
    }
}
