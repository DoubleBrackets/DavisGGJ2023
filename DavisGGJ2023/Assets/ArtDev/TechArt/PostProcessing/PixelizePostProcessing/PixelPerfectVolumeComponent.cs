using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/PixelPerfectVolumeComponent", typeof(UniversalRenderPipeline))]
public class PixelPerfectVolumeComponent : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enabled = new BoolParameter(false, true);

    // Is this post process effect active?
    public bool IsActive() => enabled.value;

    public bool IsTileCompatible() => true;
}