using UnityEngine;

public abstract class VFXEffect : MonoBehaviour
{
    public abstract void PlayEffect(PlayVFXProfile profile, PlayVFXSettings settings);
    public abstract void StopEffect();
    public abstract bool PollActive();
    public abstract void DisposeEffect();
    public abstract void UpdateVFX();

}
