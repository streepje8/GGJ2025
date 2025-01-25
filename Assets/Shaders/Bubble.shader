Shader "GGJ/GradientLighting"
{
    Properties
    {
        [HDR] _Tint("Bubble Tint", Color) = (1,1,1,1)
        _Power("IOR", Float) = 0
        _Specular("Specular", Range(0.0, 1.0)) = 0.95
        _Noise("Noise Texture", 2D) = "white"
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        ZWrite Off
        Lighting Off
        Fog { Mode Off }

        Blend SrcAlpha OneMinusSrcAlpha 
        
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

            TEXTURE2D(_Noise);
            SAMPLER(sampler_Noise);

            CBUFFER_START(UnityPerMaterial)
                float4 _Noise_ST;
                float _Specular;
                float _Power;
                float4 _Tint;
            CBUFFER_END
            
            Varyings vert(Attributes i)
            {
                Varyings o;
                o.position = TransformObjectToHClip(i.vertex.xyz);
                o.worldNormal = TransformObjectToWorldNormal(i.normal);
                o.uv = TRANSFORM_TEX(i.uv, _Noise);
                float4 worldPos = mul(unity_ObjectToWorld, i.vertex);
                float3 hitPos = worldPos.xyz / worldPos.w;
                o.viewDir = normalize(_WorldSpaceCameraPos - hitPos);
                return o;
            }

            float fresnel(float3 normal, float3 viewDir, float power)
            {
                return pow((1.0 - saturate(dot(normalize(normal), normalize(viewDir)))), power);
            }
            
            half4 frag(Varyings i) : SV_Target
            {
                float Maow = fresnel(i.worldNormal, i.viewDir, _Power);
                float valA = saturate(lerp(0.1,1, Maow / 0.395));
                float NdotL = dot(i.worldNormal, GetMainLight().direction);
                if (NdotL < _Specular) NdotL = 0;
                float specular = saturate((NdotL - _Specular) / (1.0f - _Specular));
                //return float4(specular, specular, specular, 1);
                half4 color = lerp(float4(1,1,1,0), SAMPLE_TEXTURE2D(_Noise, sampler_Noise, i.uv + float2(_Time.y, _Time.y * 0.2) * 0.1),valA);
                return lerp(color, float4(1,1,1,1), saturate(specular)) * _Tint;
            }
            ENDHLSL
        }
    }
}