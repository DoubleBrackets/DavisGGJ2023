using UnityEngine;

public class SimpleParticleEffect : VFXEffect
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float duration;

    private PlayVFXSettings settings;

    private bool isPlaying = true;
    private float startTime;
    
    public override void PlayEffect(VFXEffectProfile profile, PlayVFXSettings settings)
    {
        this.settings = settings;
        transform.position = settings.position;
        transform.rotation = settings.rotation;
        particles.Play();
        settings.stopEvent += StopEffect;
        startTime = Time.time;
    }

    public override void StopEffect()
    {
        settings.stopEvent -= StopEffect;
        isPlaying = false;
    }

    public override bool PollActive()
    {
        return Time.time - startTime < duration;
    }

    public override void DisposeEffect()
    {
        settings.stopEvent -= StopEffect;
        Destroy(gameObject);
    }

    public override void UpdateVFX()
    {
        
    }
}
