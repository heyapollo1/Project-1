Shader "Unlit/TexturedShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} //white, black, gray, bump, normal map colour
        _Pattern ("Pattern", 2D) = "white" {}
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

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 worldCoords : TEXCOORD1;
            };

            struct Interpolator
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex; // important
            float4 _MainTex_ST; // optional, _ST contains offset, scale values.
            sampler2D _Pattern;
            
            Interpolator vert (MeshData v)
            {
                Interpolator o;
                o.worldPos = mul(UNITY_MATRIX_M, float4( v.vertex.xyz, 1 )); // object to world
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); //optional, enables tiling and offset in properties
                o.uv.x += _Time.y * 0.1; 
                return o;
            }

            fixed4 frag (Interpolator i) : SV_Target
            {
                float2 topDownProjection = i.worldPos.xy; //Vector2. projected top down, means vert is stretched weird
                float4 col = tex2D( _MainTex, topDownProjection);//terrain
                return col;
            }
            ENDCG
        }
    }
}
