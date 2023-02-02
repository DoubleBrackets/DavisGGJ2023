using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/GameLevelSO")]
public class GameLevelSO : DescriptionBaseSO
{
    [SerializeField] public string sceneName;
    [SerializeField] public string scenePath;
}
