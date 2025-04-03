Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        _ColorA("Color A", Color) = (1,0,0,1)
        _ColorB("Color B", Color) = (0,1,0,1)
        _Health ("Health", Range(0,1)) = 1
         _BorderSize ("Border Size", Range(0,0.5)) = 0.1
        //_HealthStart("Color Start", Range(0,1)) = 1
        //_HealthEnd("Color End", Range(0,1)) = 0 
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        
        Pass
        {
            ZWrite Off
            //src * srcAlpha + dst * (1-srcAlpha) 
            Blend SrcAlpha OneMinusSrcAlpha //Alpha Blending
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            float4 _ColorA; 
            float4 _ColorB; 
            //float4 _Health;
            //float _HealthEnd;
            
            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normals : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Interpolator
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float maxHealth : TEXCOORD2;
                float minHealth : TEXCOORD3;
            };

            sampler2D _MainTex;
            float _Health;
            float _BorderSize;
            
            Interpolator vert (MeshData v)
            {
                Interpolator o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                return o;
            }

             float InverseLerp ( float a, float b, float v)
            {
                return (v-a)/(b-a);
            }
            
            float4 frag (Interpolator i) : SV_Target
            {
                //rounded corner clipping
                float2 coords = i.uv;
                coords.x *= 8; // before, coords.x[0,1], after, coords.x[0,8], segmented for health bar
                float2 pointOnLineSeg = float2(clamp(coords.x, 0.5, 7.5), 0.5); //centre line in health bar, clamped at edges 
                float sdf = distance(coords, pointOnLineSeg) * 2 - 1;
                clip(-sdf);
                float borderSDF = sdf + _BorderSize;
                float pd = fwidth(borderSDF); //screen space partial derivative, rate of change/ anti aliasing?
                float borderMask = step(0, -borderSDF);
                //return float4(borderMask.xxx,1);
                
                float healthbarMask = _Health > i.uv.x; //the t value, masking health depending on range
                float3 healthbarColor = tex2D(_MainTex, float2(_Health, i.uv.y));

                //float flash = cos(_Time.y  * 4) * 0.3 + 1; //flashing FX formula. _Time.y: current time that has passed in seconds
                //return float4(flash.xxx, 1); //flash.RGB, A
                if (_Health < 0.2) // when under 20% life
                {
                    float flash = cos(_Time.y  * 4) * 0.3 + 1; //flash formula
                    healthbarColor *= flash; //only healthbar flashes.
                }
                
                return float4(healthbarColor * healthbarMask * borderMask, 1); //retains saturation, looks better than +.
                /*
                //return float4(1,0,0, i.uv.x); //horizontal fade
                
                float tHealthColor = saturate(InverseLerp(0.2, 0.8, _Health)); // wherever hp is 0.2, t=0, wherever hp is 0.8, t=1
                float3 healthbarColor = lerp(float3(1,0,0), float3(0,1,0), tHealthColor); //colour changes with value
                
                //float bgColor = float3(0,0,0); //black color, irrelavant during transparency
                float healthbarMask = _Health > i.uv.x; //the t value, masking health depending on range
                //float healthbarMask2 = _Health > floor(i.uv.x * 8)/8; //health bar with SECTIONS.

                //clip(healthbarMask - 0.5); //make bg transparent without TRANSPARENT blend
                
                //float3 outputColor = lerp(bgColor, healthbarColor, healthbarMask);
                
                return float4(healthbarColor, healthbarMask); 
                return healthbarMask;
                return float4(i.uv, 0, 0);
                */
                return float4(healthbarColor * healthbarMask, 1);
                //return col;
            }
            ENDCG
        }
    }
}
