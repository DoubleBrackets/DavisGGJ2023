using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/PlayVFXProfile")]
public class VFXEffectProfile : ScriptableObject
{
    public VFXEffect VFX => vfx;

    [SerializeField] private VFXEffect vfx;
}

public struct PlayVFXSettings
{
    public Vector3 position;
    public Quaternion rotation;
    public Action stopEvent;
}
