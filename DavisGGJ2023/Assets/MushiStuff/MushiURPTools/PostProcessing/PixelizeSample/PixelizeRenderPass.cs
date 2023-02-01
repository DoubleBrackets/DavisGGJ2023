using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using MushiURPTools.PostProcessing;

[System.Serializable]
public class PixelizeRenderPass : SimplePostProcessPass
{
    // Pixelize by using a downsampled RT
    // This looks terrible but its just an example
    private RTHandle downsampleRT;

    readonly int downsampleRTId = Shader.PropertyToID("_PixelizeRenderPassTempRT");

    [System.Serializable]
    public struct PixelizeRenderPassSettings
    {
        public Vector2 resolution;
    }

    private PixelizeRenderPassSettings settings;

    public PixelizeRenderPass(PixelizeRenderPassSettings settings, bool applyInEditor, RenderPassEvent renderPassEvent) : 
        base(applyInEditor, renderPassEvent)
    {
        this.settings = settings;
    }

    public override void CreateTemporaryRT(CommandBuffer cmd, RenderTextureDescriptor descriptor)
    {
        // Downsample
        descriptor.width = (int)settings.resolution.x;
        descriptor.height = (int)settings.resolution.y;
        
        cmd.GetTemporaryRT(downsampleRTId , descriptor, FilterMode.Point);
        downsampleRT = RTHandles.Alloc(downsampleRTId);
    }

    public override void CleanupTemporaryRT(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(downsampleRTId);
    }
    
    protected override void PerformCustomPostProcessing(CommandBuffer cmd, VolumeStack postProcessVolumeStack)
    {
        bool anyApplied = false;
        
        var volumeComponent = postProcessVolumeStack.GetComponent<PixelizeVolumeComponent>();
        
        if (volumeComponent.IsActive())
        {
            anyApplied = true;
            // Blit from color text to low res downsampleRT
            Blit(cmd, cameraColorTarget, downsampleRT);
        }
 
        // Done; Blit the results of the pass to the camera color texture
        if (anyApplied)
            Blit(cmd, downsampleRT, cameraColorTarget);
    }

    protected override bool CheckMissingDependencies()
    {
        return false;
    }
}