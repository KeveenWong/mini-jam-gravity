using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class HeightFog : MonoBehaviour
{
    public Color fogColor = Color.white;
    public float fogHeight = 10f;
    public float fogThickness = 2f;
    public float maxFogStrength = 1f;
    
    private Material fogMaterial;
    private Camera attachedCamera;

    void OnEnable()
    {
        // Ensure we have the required components
        attachedCamera = GetComponent<Camera>();
        
        if (GraphicsSettings.currentRenderPipeline == null)
        {
            Debug.LogError("This effect requires the Universal Render Pipeline.");
            enabled = false;
            return;
        }

        // Load the shader and create the material
        var shader = Shader.Find("Custom/HeightFog");
        if (shader == null)
        {
            Debug.LogError("Could not find HeightFog shader. Ensure it's in a Resources folder or properly included in your build.");
            enabled = false;
            return;
        }
        
        fogMaterial = new Material(shader);

        // Configure the camera
        var cameraData = attachedCamera.GetUniversalAdditionalCameraData();
        if (cameraData != null)
        {
            cameraData.renderPostProcessing = true;
            // Ensure we're rendering depth
            attachedCamera.depthTextureMode |= DepthTextureMode.Depth;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (fogMaterial == null || attachedCamera == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // Update material properties
        fogMaterial.SetColor("_FogColor", fogColor);
        fogMaterial.SetFloat("_FogHeight", fogHeight);
        fogMaterial.SetFloat("_FogThickness", fogThickness);
        fogMaterial.SetFloat("_MaxFogStrength", maxFogStrength);
        
        // Calculate and set camera matrices
        Matrix4x4 cameraToPerspective = GL.GetGPUProjectionMatrix(attachedCamera.projectionMatrix, false);
        Matrix4x4 worldToCamera = attachedCamera.worldToCameraMatrix;
        Matrix4x4 cameraToWorld = worldToCamera.inverse;
        
        fogMaterial.SetMatrix("_CameraToWorld", cameraToWorld);
        
        // Apply the effect
        Graphics.Blit(source, destination, fogMaterial);
    }

    void OnDisable()
    {
        if (fogMaterial != null)
        {
            DestroyImmediate(fogMaterial);
        }
    }
}