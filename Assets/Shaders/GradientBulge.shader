Shader "GGJ/GradientLightingBulge"
{
    Properties
    { 
        _Gradient("Gradients", 2D) = "white"
        _BulgeAmount("Bulge Amount", float) = 1.0
        _MinBulgeDistance("Min Bulge Distance", float) = 1.0
        _MaxBulgeDistance("Max Bulge Distance", float) = 5.0
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
            };

            TEXTURE2D(_Gradient);
            SAMPLER(sampler_Gradient);

            CBUFFER_START(UnityPerMaterial)
                float4 _Gradient_ST;
            CBUFFER_END

            float3 _BulgePositionA;
            float3 _BulgePositionB;
            float _BulgeAmount;
            float _BulgeDistance;
            float _MinBulgeDistance;
            float _MaxBulgeDistance;

            float mapRange(float value, float fromMin, float fromMax, float toMin, float toMax)
            {
                float output = toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
                return output;
            }
            
            Varyings vert(Attributes i)
            {
                Varyings o;
                o.worldNormal = TransformObjectToWorldNormal(i.normal);
                float4 worldPos = mul(unity_ObjectToWorld, i.vertex);
                float3 position = worldPos.xyz / worldPos.w;
                float disToBulge = min(distance(position, _BulgePositionA), distance(position, _BulgePositionB));
                float bulge = mapRange(disToBulge, _MinBulgeDistance, _MaxBulgeDistance, 1, 0);
                bulge = clamp(bulge, 0, 1);
                position += o.worldNormal * _BulgeAmount * bulge;
                o.position = TransformWorldToHClip(position);
                o.uv = TRANSFORM_TEX(i.uv, _Gradient);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float light = dot(GetMainLight().direction, i.worldNormal) * 0.5f + 0.5f;
                half4 color = SAMPLE_TEXTURE2D(_Gradient, sampler_Gradient, float2(light,i.uv.y));
                return color;
            }
            ENDHLSL
        }
    }
}