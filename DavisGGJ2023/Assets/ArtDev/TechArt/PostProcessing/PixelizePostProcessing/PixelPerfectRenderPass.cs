using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using MushiURPTools.PostProcessing;

[System.Serializable]
public class PixelPerfectRenderPass : SimplePostProcessPass
{
    // Used to blit post processing materials back and forth
    private RTHandle destinationA;
    private RTHandle destinationB;
    private RTHandle previousDest;

    readonly int temporaryRTIdA = Shader.PropertyToID("_PixelizeRenderPassTempRTA");
    readonly int temporaryRTIdB = Shader.PropertyToID("_PixelizeRenderPassTempRTB");

    [System.Serializable]
    public struct PixelPerfectSettings
    {
        public Material postProcessMaterial;
    }
    
    private PixelPerfectSettings settings;

    public PixelPerfectRenderPass(PixelPerfectSettings settings, bool applyInEditor, RenderPassEvent renderPassEvent) : 
        base(applyInEditor, renderPassEvent)
    {
        this.settings = settings;
    }

    public override void CreateTemporaryRT(CommandBuffer cmd, RenderTextureDescriptor descriptor)
    {
        // Create two temporary RTs for blitting post process effects
        cmd.GetTemporaryRT(temporaryRTIdA , descriptor, FilterMode.Point);
        destinationA = RTHandles.Alloc(temporaryRTIdA);
        /*cmd.GetTemporaryRT(temporaryRTIdB , descriptor, FilterMode.Bilinear);
        destinationB = RTHandles.Alloc(temporaryRTIdB);*/
    }

    public override void CleanupTemporaryRT(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(temporaryRTIdA);
        /*cmd.ReleaseTemporaryRT(temporaryRTIdB);*/
    }
    
    protected override void PerformCustomPostProcessing(CommandBuffer cmd, VolumeStack postProcessVolumeStack)
    {
        // Initial source is camera color texture
        previousDest = cameraColorTarget;
        bool anyApplied = false;
        
        // TODO: Get the corresponding volume component
        var volumeComponent = postProcessVolumeStack.GetComponent<PixelPerfectVolumeComponent>();
        
        if (volumeComponent.IsActive())
        {
            anyApplied = true;
            var material = settings.postProcessMaterial;
            
            // TODO: Apply volume component properties to material here
            
            BlitToTemp(cmd, material);
        }
        
        // TODO: More post process effects go here, in order of applying

        // Done; Blit the results of the pass to the camera color texture
        if (anyApplied)
            Blit(cmd, previousDest, cameraColorTarget);
    }

    /// <summary>
    /// Blits back and forth alternating between two temporary RTs
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="mat"></param>
    /// <param name="shaderPass"></param>
    private void BlitToTemp(CommandBuffer cmd, Material mat, int shaderPass = 0)
    {
        // Source is the previous blit destination
        var blitSource = previousDest;
        
        // Destination is the opposite temporary RT
        var blitDest = (blitSource == destinationA) ? destinationB : destinationA;
        
        Blit(cmd, blitSource, blitDest, mat, shaderPass);
        
        previousDest = blitDest;
    }

    protected override bool CheckMissingDependencies()
    {
        if (settings.postProcessMaterial == null)
        {
            Debug.LogError("Missing Post Process Material");
            return true;
        }

        return false;
    }
}