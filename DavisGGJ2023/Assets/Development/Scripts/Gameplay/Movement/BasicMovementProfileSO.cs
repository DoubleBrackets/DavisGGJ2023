using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Profiles/BasicMovementProfile", fileName = "MovementProfile")]
public class BasicMovementProfileSO : DescriptionBaseSO
{
    public float MaxWalkSpeed => maxWalkSpeed;

    public float WalkAcceleration => walkAcceleration;

    public float MaxRunSpeed => maxRunSpeed;

    public float RunAcceleration => runAcceleration;

    public float FrictionAcceleration => frictionAcceleration;

    [ColorHeader("Walking", ColorHeaderColor.Config)]
    [SerializeField] private float maxWalkSpeed;
    [SerializeField] private float walkAcceleration;
    [SerializeField] private float frictionAcceleration;
    
    [ColorHeader("Running", ColorHeaderColor.Config)]
    [SerializeField] private float maxRunSpeed;
    [SerializeField] private float runAcceleration;

}
