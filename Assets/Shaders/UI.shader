Shader "Unlit/UI"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MaskTex("Texture", 2D) = "white" {}
        _Fill("Fill Color", Color) = (1,1,1,1)
        _Progress("Progress", Range(0.0, 1.0)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MaskTex;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MaskTex_ST;
            float _Progress;
            float4 _Fill;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 mask = tex2D(_MaskTex, i.uv);
                if (col.a < 0.5) discard;
                if (mask.r > 0)
                {
                    if (i.uv.y < _Progress)
                    {
                        return col * _Fill;
                    }
                }
                return col;
            }
            ENDCG
        }
    }
}
