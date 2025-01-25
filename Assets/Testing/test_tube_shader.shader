// This shader draws a texture on the mesh.
Shader "Testing/TestTubeShader"
{
    // The _BaseMap variable is visible in the Material's Inspector, as a field
    // called Base Map.
    Properties
    { 
        _BaseMap("Base Map", 2D) = "white"
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

            float3 _BulgePosition;
            float _BulgeAmount;
            float _BulgeDistance;
            float _MinBulgeDistance;
            float _MaxBulgeDistance;

            struct Attributes
            {
                float4 positionOS   : POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
                float3 normal       : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
            };

            // This macro declares _BaseMap as a Texture2D object.
            TEXTURE2D(_BaseMap);
            // This macro declares the sampler for the _BaseMap texture.
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                // The following line declares the _BaseMap_ST variable, so that you
                // can use the _BaseMap variable in the fragment shader. The _ST 
                // suffix is necessary for the tiling and offset function to work.
                float4 _BaseMap_ST;
            CBUFFER_END

            float mapRange(float value, float fromMin, float fromMax, float toMin, float toMax)
            {
                float output = toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
                return output;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 position = IN.positionOS.xyz;
                float3 normal = TransformObjectToWorldNormal(IN.normal);

                float disToBulge = distance(position, _BulgePosition);
                float bulge = mapRange(disToBulge, _MinBulgeDistance, _MaxBulgeDistance, 1, 0);
                bulge = clamp(bulge, 0, 1);
                //bulge = (1 - (1 - bulge) * (1 - bulge));
                position += normal * _BulgeAmount * bulge;
                OUT.positionHCS = TransformObjectToHClip(position);
                // The TRANSFORM_TEX macro performs the tiling and offset
                // transformation.
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // The SAMPLE_TEXTURE2D marco samples the texture with the given
                // sampler.
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                return color;
            }
            ENDHLSL
        }
    }
}