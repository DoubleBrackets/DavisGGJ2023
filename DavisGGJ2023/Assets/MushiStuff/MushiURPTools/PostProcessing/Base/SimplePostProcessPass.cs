using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace MushiURPTools.PostProcessing
{
    /// <summary>
    /// Base class for a simple post-processing render pass
    /// </summary>
    [System.Serializable]
    public abstract class SimplePostProcessPass : ScriptableRenderPass
    {
        protected RTHandle cameraColorTarget;
        protected RTHandle cameraDepthTarget;

        private bool applyInEditor;

        protected SimplePostProcessPass(bool applyInEditor, RenderPassEvent renderPassEvent)
        {
            this.applyInEditor = applyInEditor;
            this.renderPassEvent = renderPassEvent;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Get the camera target's RT descriptor (may be needed to create temporary RTs)
            RenderTextureDescriptor cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;

            var renderer = renderingData.cameraData.renderer;
            
            cameraColorTarget = renderer.cameraColorTargetHandle;
            cameraDepthTarget = renderer.cameraDepthTargetHandle;
            
            CreateTemporaryRT(cmd, cameraTargetDescriptor);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            CleanupTemporaryRT(cmd);
        }


        /// <summary>
        /// Create any temporary render textures needed
        /// </summary>
        public abstract void CreateTemporaryRT(CommandBuffer cmd, RenderTextureDescriptor descriptor);
        
        /// <summary>
        /// Cleanup temporary render textures 
        /// </summary>
        public abstract void CleanupTemporaryRT(CommandBuffer cmd);

        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
    	    // Skip rendering in scene view if necessary
            if(!applyInEditor && renderingData.cameraData.isSceneViewCamera)
                return;
            
            if (CheckMissingDependencies())
            {
                return;
            }
            
            CommandBuffer cmd = CommandBufferPool.Get("Custom Post Processing Pass");
            cmd.Clear();
            
            // Perform post processing
            PerformCustomPostProcessing(cmd, VolumeManager.instance.stack);

            // Execute the commands and cleanup
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        protected abstract void PerformCustomPostProcessing(CommandBuffer cmd, VolumeStack postProcessVolumeStack);

        protected abstract bool CheckMissingDependencies();

}
}
