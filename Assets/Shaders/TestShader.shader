Shader "Unlit/testShader"
{
    Properties //Input data
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _Value("Value", Float) = 1.0 // Define property
    }
    SubShader
    {
        Tags { "RenderType"p="Opaque" }
        //LOD 100

        Pass {

            CGPROGRAM
            //#pragma vertex vert
            //#pragma fragment frag

            #include "UnityCG.cginc" //includes another file, in this case, built in Unity functions. Useful, keep.
            float _Value;

            struct MeshData // name of struct, can change to ur liking
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            /*v2f vert (MeshData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }*/
            ENDCG
        }
    }
}
