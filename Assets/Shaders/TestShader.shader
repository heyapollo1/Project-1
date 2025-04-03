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
        //_Scale("UV Scale", Float) = 1
        //_Offset("UV Offset", Float) = 0
    }
    SubShader
    {
        //subshader tags
        //Tags { "RenderType" = "Opaque" }
        Tags { "RenderType" = "Transparent" } // tag to inform the render pipeline of what type this is.for transparent objects
        Tags { "Queue" = "Transparent" } //changes the render order. order that things are drawn in.
        //LOD 100 //level of detail, unimportant
        
        Pass {
            // pass tags
            //Cull Front //Culls front instead of default back.
            Cull Off
            ZWrite Off //Turns off depth buffer
            //ZTest LEqual //depth buffer visualizer. LEqual is default, it means if depth of object is <= depth already written into depth buffer, show it.
            //ZTest Always //ignores depth buffer, wont be blocked.
            //ZTest GEqual // only shows where it is being blocked.
            Blend One One // Additive
            //Blend DstColor Zero // Multiply
            
            CGPROGRAM // HLSL
            #pragma vertex vert
            #pragma fragment frag

            #define TAU 6.28318530718
            
            #include "UnityCG.cginc" //Takes another file pastes into your shader, in this case, built in Unity functions. Useful, keep.
            float4 _ColorA; //Getting value from properties in shader.
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;
            //float _Scale; //float?
            //float _Offset;
            
            //Automatically filled out by Unity?
            struct MeshData // per-vertex mesh data
            {
                float4 vertex : POSITION; // local space vertex position
                float3 normals : NORMAL; // local space normal direction
                //float4 tangent : TANGENT; // tangent direction (xyz) tangent sing (w)
                //float4 color : COLOR; //float(4): RGBA, A is Alpha
                float4 uv0 : TEXCOORD0; // uv0 coordinates, where to map 2d texture on 3d object, unfolding, unwrap, packing.
                //float4 uv1 : TEXCOORD1; // uv1 coordinates lightmap coordinates
                //float4 uv2 : TEXCOORD1; // uv2 coordinates lightmap coordinates
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

            Interpolators vert( MeshData v )
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex); // converts local space to clip space. FOLLOWS TRANSFORM
                //o.normal = v.normals;//justpass through, Local space/model space.
                //o.normal = UnityObjectToWorldNormal(v.normals); //world space normals, not local(lighting?).
                o.normal = mul((float3x3)unity_ObjectToWorld, v.normals); //manual typing of UnityObjectToWorldNormal function.
                //o.normal = mul(v.normals, (float3x3)unity_ObjectToWorld); //worse variant of abovve method.

                o.uv = v.uv0; //pass through.
                //o.uv = v.uv0 * _Scale;
                //o.uv = (v.uv0 + _Offset) * _Scale;
                //o.uv = (v.uv0 + _Offset) * _Scale;
                
                //If you want something to cover entire screen, dont need, render in clip space. IE Post processors, ambience, colour grading.
                return o;
            }

            //bool 0 1, true/false
            //int
            //float (32 bit float), use everywhere
            //half (16 bit float)
            //fixed (lower precision)
            //float4 -> half4 -> fixed4
            //float4x4 -> half4x4 (C#: Matrix 4x4)

            float InverseLerp ( float a, float b, float v)
            {
                return (v-a)/(b-a);
            }
            
            fixed4 frag (Interpolators i) : SV_Target //Target of fragment shader is the SV_Target
            {
                float xOffset = cos(i.uv.x * TAU * 8) * 0.02; //zigzag pattern offset code
                float t = cos((i.uv.y + xOffset - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5; //animated with Time! +Time and -Time, reverses direction of anim. fully opaque render
                //t *= i.uv.y;
                t *= 1-i.uv.y;// 1- flips direction of fade to dark

                float topBottomRemover = (abs(i.normal.y) < 0.999);
                float waves = t * topBottomRemover;

                float4 gradient = lerp(_ColorA, _ColorB, i.uv.y);
                return gradient * waves; //animated pattern + gradient + transparency
                //return waves; //using normal of surface, if vector is pointing almost or eniterly up/down, remove.removes top and bottom of cylinder mesh for exmplae.
                return t;
            }
            ENDCG
        }
            /*fixed4 frag (Interpolators i) : SV_Target //Target of fragment shader is the SV_Target
            {
                //float t = abs(frac(i.uv.x * 5) * 2 - 1); // triangle wave..
                //float t = cos(i.uv.x * 25); //repeating pattern
                
                //IMPORTANT
                //float t = cos(i.uv.x * TAU * 2)//remapping -1 to 1 to 0 to 1, vice versa. Common?

                float xOffset = cos(i.uv.x * TAU * 8) * 0.02; //zigzag pattern offset code
                float t = cos((i.uv.y + xOffset - _Time.y * 0.1) * TAU * 5) * 0.5 + 0.5; //animated with Time! +Time and -Time, reverses direction of anim. fully opaque render
                //t *= i.uv.y;
                t *= 1-i.uv.y;// 1- flips direction of fade to dark
                return t;
                
                //float2 t = cos(i.uv.xy * TAU * 2) * 0.5 + 0.5; //checkerboard pattern, xy, both x and y
                //return float4(t,0,1);
                
                
                //float4 outColor = lerp(_ColorA, _ColorB, t);
                //return outColor;
            }*/
            /*fixed4 frag (Interpolators i) : SV_Target //Target of fragment shader is the SV_Target
            {
                //float4 myValue;
                //float2 otherValue = myValue.rg; // rg can be gr to flip , swizzling. Grayscale = myValue.xxxx
                //return _Color; //RGBA, RED. 0,1,2,3
                //return float4(i.normal, 1);
                //return float4(i.uv, 0, 1); // mango colour, x + y visualization
                //return float4(i.uv.xxx, 1); // x gradient(black).
                //return float4(i.uv.yyy, 1); // y gradient(black).

                //blend  between 2 colors based on the x UV coordinates
                //float4 outColor = lerp(_ColorA, _ColorB, i.uv.x); //start color, end color, interpolator(UVs) coordinates of object
                
                //float t = InverseLerp( _ColorStart, _ColorEnd, i.uv.x);
                //float t = saturate(InverseLerp( _ColorStart, _ColorEnd, i.uv.x)); // saturate is CLAMP, if value >1, make it 1, if <0, make it 0.
                //t = frac(t); Gradient exceeds value 0/1, repeats pattern like a texture, check it clamped or not
                //return t;
                //float4 outColor = lerp(_ColorA, _ColorB, t);
                //return outColor;
                //return outColor;
            }*/
    }
}
