Shader "Unlit/ShaderTemplate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            Interpolator vert (MeshData v)
            {
                Interpolator o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1; //remap 0,1 to 1,-1
                return o;
            }

            fixed4 frag (Interpolator i) : SV_Target
            {
                //float dist = length(i.uv) - 0.3; // length calculates Euclidean dist from centre of UV space(0,0). radial CIRCLE
                float dist = i.uv.x - 0.3; // fade on x axis
                //return step(0, dist); // a<=b, step is a threshold check?
               return float4(dist.xxx,0);
            }
            ENDCG
        }
    }
}
