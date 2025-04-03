Shader "Unlit/OffsetShader"
{
    Properties //Input data
    {
        //_MainTex ("Texture", 2D) = "white" {}
        //_Value("Value", Float) = 1.0 // Define property
        _ColorA("Color A", Color) = (1,1,1,1) // Define property
        _ColorB("Color B", Color) = (1,1,1,1)
        _ColorStart("Color Start", Range(0,1)) = 0 // Range is slider
        _ColorEnd("Color End", Range(0,1)) = 0
        _WaveAmp("Wave Amplitude", Range(0,0.2)) = 1
    }
    SubShader
    {
        //subshader tags
        Tags { "RenderType" = "Opaque" }
        Tags { "Queue" = "Geometry" } //changes the render order. order that things are drawn in.
        
        Pass {
            CGPROGRAM // HLSL
            #pragma vertex vert
            #pragma fragment frag

            #define TAU 6.28318530718
            
            #include "UnityCG.cginc" //Takes another file pastes into your shader, in this case, built in Unity functions. Useful, keep.
            float4 _ColorA; //Getting value from properties in shader.
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;
            float _WaveAmp;
            
            //Automatically filled out by Unity?
            struct MeshData // per-vertex mesh data
            {
                float4 vertex : POSITION; // local space vertex position
                float3 normals : NORMAL; // local space normal direction
                float4 uv0 : TEXCOORD0; // uv0 coordinates, where to map 2d texture on 3d object, unfolding, unwrap, packing.
            };

            struct Interpolators //V2F. Lerp between vertices. Barycentric Interpolation.
            {
                //TEXCOORD0 is Not the same as uv in MeshData! Semantics, not coordinates.
                float4 vertex : SV_POSITION; //clip space position, ALWAYS NEEDS TO BE SET< NOT OPTIONAl
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 lengthProperties : TEXCOORD2;
                float4 justSomeValues : TEXCOORD3;
            };
            
            float GetWave(float2 uv)
            {
                float2 uvsCentered = uv * 2 - 1;
                float radialDistance = length(uvsCentered);
                //return float4 (radialDistance.xxx, 1); // centered coordinates, circular pattern, radial
                float wave = cos((radialDistance - _Time.y * 0.1) * TAU * 2) * 0.5 + 0.5; //animated with Time! +Time and -Time, reverses direction of anim. fully opaque render
                wave *= 1 - radialDistance; //ripples fade out at ends.
                return wave; //radial animation, hypnosis    
            }
            
            Interpolators vert( MeshData v )
            {
                Interpolators o;
                //float wave = cos((v.uv0.y - _Time.y * 0.1) * TAU * 2); // for each vert not pixel/fragment
                //float wave2 = cos((v.uv0.x - _Time.y * 0.1) * TAU * 2);
                v.vertex.y = GetWave(v.uv0);
                //v.vertex.y = wave * wave2;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normals);
                o.uv = v.uv0; //pass through.
                return o;//Wave/water texture aniamtion!
            }

            float InverseLerp ( float a, float b, float v)
            {
                return (v-a)/(b-a);
            }

            float4 frag (Interpolators i) : SV_Target //Target of fragment shader is the SV_Target
            {
                //float wave = cos((i.uv.y - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5; //animated with Time! +Time and -Time, reverses direction of anim. fully opaque render
                //return wave;

                float2 uvsCentered = i.uv * 2 - 1;
                float radialDistance = length(uvsCentered);  // radial coordinate
                //return float4 (radialDistance.xxx, 1); // centered coordinates, circular pattern, radial
                float wave = cos((radialDistance - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5; //animated with Time! +Time and -Time, reverses direction of anim. fully opaque render
                wave *= 1 - radialDistance; //ripples fade out at ends.
                return wave; //radial animation, hypnosis
                //return gradient * wave; //animated pattern + gradient + transparency
            }
            ENDCG
        }
    }
}
