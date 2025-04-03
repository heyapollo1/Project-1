
Shader "Unlit/OutlineShader"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _OutlineColorA ("Outline Color A", Color) = (1,1,1,1)
        _OutlineColorB ("Outline Color B", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Float) = 1
        _ShimmerSpeed ("Shimmer Speed", Float) = 0.5
        _ShimmerIntensity ("Shimmer Intensity", Float) = 0.5
        _ColorShiftSpeed ("Color Shift Speed", Float) = 0.5
        _WavyDistortion ("Wavy Distortion Intensity", Float) = 0.05
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _OutlineColorA;
            float4 _OutlineColorB;
            float _OutlineThickness;
            float _ShimmerSpeed;
            float _ShimmerIntensity;
            float _ColorShiftSpeed;
            float _WavyDistortion;
            float4 _MainTex_ST;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float alpha = tex2D(_MainTex, i.uv).a;
                float thickness = _OutlineThickness / 100.0;

                float2 offsets[8] = {
                    float2(-thickness, 0), float2(thickness, 0),
                    float2(0, -thickness), float2(0, thickness),
                    float2(-thickness, -thickness), float2(-thickness, thickness),
                    float2(thickness, -thickness), float2(thickness, thickness)
                };

                float outline = 0;
                for (int j = 0; j < 8; j++)
                {
                    float2 offset = offsets[j];

                    // 🌊 Apply wavy distortion to the outline edge
                    float wave = sin((i.uv.x + i.uv.y + _Time.y * 2.0 + j) * 10.0) * (_WavyDistortion / 100);
                    offset += float2(wave, wave);

                    outline += tex2D(_MainTex, i.uv + offset).a;
                }

                float baseAlpha = alpha;
                float outlineAlpha = saturate(outline - baseAlpha);

                float4 baseColor = tex2D(_MainTex, i.uv);

                float tColor = sin(_Time.y * _ColorShiftSpeed) * 0.5 + 0.5;
                float4 animatedOutlineColor = lerp(_OutlineColorA, _OutlineColorB, tColor);

                float shimmer = sin((i.uv.x + i.uv.y + _Time.y * _ShimmerSpeed) * 20.0) * 0.5 + 0.5;
                animatedOutlineColor.rgb *= lerp(1.0, 1.0 + _ShimmerIntensity, shimmer);
                
                float4 finalColor = lerp(animatedOutlineColor, baseColor, baseAlpha);
                finalColor.a = max(baseAlpha, outlineAlpha);
                return finalColor;
            }
            ENDCG
        }
    }
}