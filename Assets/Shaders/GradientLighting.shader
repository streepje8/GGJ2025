// This shader draws a texture on the mesh.
Shader "GGJ/GradientLighting"
{
    Properties
    { 
        _Gradient("Gradients", 2D) = "white"
        _Glow("Glow", float) = 0.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            TEXTURE2D(_Gradient);
            SAMPLER(sampler_Gradient);
            float _Glow;

            CBUFFER_START(UnityPerMaterial)
                float4 _Gradient_ST;
            CBUFFER_END

            Varyings vert(Attributes i)
            {
                Varyings o;
                o.position = TransformObjectToHClip(i.vertex.xyz);
                o.worldNormal = TransformObjectToWorldNormal(i.normal);
                o.uv = TRANSFORM_TEX(i.uv, _Gradient);
                
                float4 worldPos = mul(unity_ObjectToWorld, i.vertex);
                float3 hitPos = worldPos.xyz / worldPos.w;
                o.viewDir = normalize(_WorldSpaceCameraPos - hitPos);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float dotWithCamera = dot(i.worldNormal, -i.viewDir) * 0.5f + 0.5f;
                float light = dot(GetMainLight().direction, i.worldNormal) * 0.5f + 0.5f;
                half4 color = SAMPLE_TEXTURE2D(_Gradient, sampler_Gradient, float2(light,i.uv.y));
                return color + dotWithCamera * dotWithCamera * _Glow;
            }
            ENDHLSL
        }
    }
}