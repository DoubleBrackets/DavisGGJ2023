using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Profiles/BasicMovementProfile", fileName = "MovementProfile")]
public class BasicMovementProfileSO : DescriptionBaseSO
{
    public float MaxWalkSpeed => maxWalkSpeed;

    public float WalkAcceleration => walkAcceleration;

    public float FrictionAcceleration => frictionAcceleration;

    [ColorHeader("Run", ColorHeaderColor.Config)]
    [SerializeField] private float maxWalkSpeed;
    [SerializeField] private float walkAcceleration;
    [SerializeField] private float frictionAcceleration;
    

}
