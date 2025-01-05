Shader "Custom/HeightFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (1,1,1,1)
        _FogHeight ("Fog Height", Float) = 10
        _FogThickness ("Fog Thickness", Float) = 2
        _MaxFogStrength ("Max Fog Strength", Float) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 viewVector : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            float4 _FogColor;
            float _FogHeight;
            float _FogThickness;
            float _MaxFogStrength;
            float4x4 _CameraToWorld;

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                // Transform to clip space for screen position
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                
                // Calculate view vector for reconstructing world position
                float4 clipPos = output.positionCS;
                float4 viewPos = mul(UNITY_MATRIX_I_P, clipPos);
                output.viewVector = viewPos.xyz / viewPos.w;
                
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // Get the depth value for this pixel
                float2 screenUV = input.positionCS.xy / _ScaledScreenParams.xy;
                float depth = LoadSceneDepth(screenUV);
                
                // Reconstruct world position from depth
                float3 viewPos = input.viewVector * LinearEyeDepth(depth, _ZBufferParams);
                float3 worldPos = mul(_CameraToWorld, float4(viewPos, 1.0)).xyz;
                
                // Calculate fog based on world height
                float heightDiff = worldPos.y - _FogHeight;
                float fogFactor = saturate(exp(-heightDiff * _FogThickness));
                fogFactor = min(fogFactor, _MaxFogStrength);
                
                // Blend between original color and fog color
                return lerp(col, _FogColor, fogFactor);
            }
            ENDHLSL
        }
    }
}