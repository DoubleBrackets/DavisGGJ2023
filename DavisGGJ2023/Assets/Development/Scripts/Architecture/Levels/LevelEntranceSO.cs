using UnityEngine;

/// <summary>
/// Represents a pathway between two level scenes
/// </summary>
[CreateAssetMenu(menuName = "Gameplay/LevelEntrance")]
public class LevelEntranceSO : DescriptionBaseSO
{
    [SerializeField] private GameLevelSO levelToEnter;

    public GameLevelSO LevelToEnter => levelToEnter;
}
