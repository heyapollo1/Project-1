Shader "Unlit/TexturedShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} //white, black, gray, bump, normal map colour
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolator
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex; // important
            float4 _MainTex_ST; // optional, _ST contains offset, scale values.
            
            Interpolator vert (MeshData v)
            {
                Interpolator o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); //optional, enables tiling and offset in properties
                o.uv.x += _Time.y * 0.1; //Panning, animated UV, like rivers
                return o;
            }

            fixed4 frag (Interpolator i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
