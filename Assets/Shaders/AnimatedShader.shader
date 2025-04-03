Shader "Unlit/testShader"
{
    Properties //Input data
    {
        //_MainTex ("Texture", 2D) = "white" {}
        //_Value("Value", Float) = 1.0 // Define property
        _ColorA("Color A", Color) = (1,1,1,1) // Define property
        _ColorB("Color B", Color) = (1,1,1,1)
        _ColorStart("Color Start", Range(0,1)) = 0 // Range is slider
        _ColorEnd("Color End", Range(0,1)) = 0
        _Opacity ("Opacity", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" } // tag to inform the render pipeline of what type this is.for transparent objects
        Tags { "Queue" = "Transparent" } //changes the render order. order that things are drawn in.
        
        Pass {
            Cull Off
            ZWrite Off //Turns off depth buffer
            Blend One One // Additive
            
            CGPROGRAM // HLSL
            #pragma vertex vert
            #pragma fragment frag

            #define TAU 6.28318530718
            
            #include "UnityCG.cginc" //Takes another file pastes into your shader, in this case, built in Unity functions. Useful, keep.
            float4 _ColorA; //Getting value from properties in shader.
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;
            float _Opacity;
            
            struct MeshData // per-vertex mesh data
            {
                float4 vertex : POSITION; // local space vertex position
                float3 normals : NORMAL; // local space normal direction
                float4 uv0 : TEXCOORD0; // uv0 coordinates, where to map 2d texture on 3d object, unfolding, unwrap, packing.
            };

            struct Interpolators //V2F. Lerp between vertices. Barycentric Interpolation.
            {
                float4 vertex : SV_POSITION; //clip space position, ALWAYS NEEDS TO BE SET< NOT OPTIONAl
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 lengthProperties : TEXCOORD2;
                float4 justSomeValues : TEXCOORD3;
            };

            Interpolators vert( MeshData v )
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex); // converts local space to clip space. FOLLOWS TRANSFORM
                o.normal = mul((float3x3)unity_ObjectToWorld, v.normals); //manual typing of UnityObjectToWorldNormal function.
                o.uv = v.uv0; //pass through.
                return o;
            }

            float InverseLerp ( float a, float b, float v)
            {
                return (v-a)/(b-a);
            }
            
            fixed4 frag (Interpolators i) : SV_Target //Target of fragment shader is the SV_Target
            {
                float xOffset = cos(i.uv.x * TAU * 3) * 0.06; //zigzag pattern offset code
                float t = cos((i.uv.y + xOffset - _Time.y * 0.1) * TAU * 2) * 0.8 + 0.6; //animated with Time! +Time and -Time, reverses direction of anim. fully opaque render
                t *= 1-i.uv.y;// 1- flips direction of fade to dark

                //float topBottomRemover = (abs(i.normal.y) < 0.999);
                //float waves = t * topBottomRemover;

                float4 gradient = lerp(_ColorA, _ColorB, i.uv.y);
                return gradient * t * _Opacity;
                
                //return waves; //using normal of surface, if vector is pointing almost or eniterly up/down, remove.removes top and bottom of cylinder mesh for exmplae.
                return t;
            }
            ENDCG
        }
    }
}
