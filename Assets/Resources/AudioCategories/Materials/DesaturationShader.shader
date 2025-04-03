Shader "Custom/DesaturationShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _Saturation ("Saturation", Range(0, 1)) = 0.2 // Control how much color remains
    }
    SubShader
    {
        Tags {"Queue"="Overlay" }
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

            struct Interpolator
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Saturation;

            Interpolator vert (appdata v)
            {
                Interpolator o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (Interpolator i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float gray = dot(col.rgb, float3(0.3, 0.59, 0.11)); // Calculate grayscale
                col.rgb = lerp(float3(gray, gray, gray), col.rgb, _Saturation); // Blend with saturation
                return col;
            }
            ENDCG
        }
    }
}