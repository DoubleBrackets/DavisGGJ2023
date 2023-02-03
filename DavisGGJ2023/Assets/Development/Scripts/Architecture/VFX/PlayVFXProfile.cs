using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/PlayVFXProfile")]
public class PlayVFXProfile : ScriptableObject
{
    public VFXEffect VFX => vfx;

    public string VFXInstanceID => vfxInstanceID;

    [SerializeField] private VFXEffect vfx;
    [SerializeField] private string vfxInstanceID;
}

public struct PlayVFXSettings
{
    public Vector3 position;
    public Quaternion rotation;
    public Action stopEvent;
}
