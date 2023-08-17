Shader "Unlit/WarpDrive"
{
    // Full WarpDrive effect with Colors and Emissions
    Properties
    {
        [Header(BASE Settings)]
         _Color ("Color", Color) = (1, 1, 1, 1)
        _VortexFade ("Vortex Fade", Range(1, 2)) = 1
        [HDR] _EmissionColor("Color", Color) = (0,0,0)
        
        [Header(WARP Settings)]
        _WarpSize ("Warp Size", Range(0, 0.5)) = 0.1
        _WarpDepth ("Warp Depth", Range(0, 2)) = 0.3
        _WarpSpeed ("Warp Speed", Range(0, 5)) = 1
        _WarpTileX ("Warp Tile X", Range(1, 10)) = 10
        _WarpTileY ("Warp Tile Y", Range(1, 10)) = 10
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "LightMode" = "UniversalForward"
        }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv_mask : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 uv_mask : TEXCOORD1;
            };

            float4 _Color;
            float4 _EmissionColor;
            float _VortexFade;
            float _WarpSize;
            float _WarpDepth;
            float _WarpSpeed;
            float _WarpTileX;
            float _WarpTileY;
            
            float warp_tex(float2 uv)
            {
                float left_wall = step(uv.x, _WarpSize);
                float right_wall = step(1 - uv.x, _WarpSize);
                left_wall += right_wall;

                float top_gradient = smoothstep(uv.y, uv.y - _WarpDepth, 0.5);
                float bottom_gradient = smoothstep(1 - uv.y, 1 - uv.y - _WarpDepth, 0.5);
                float cut = clamp(top_gradient + bottom_gradient, 0, 1);

                return clamp((1 - left_wall) * (1 - cut), 0, 1);
            }

            float unity_noise_randomValue2D(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv_mask = v.uv_mask;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                i.uv.x *= _WarpTileX;
                i.uv.y *= _WarpTileY;
                i.uv.y += (_Time.y * _WarpSpeed);

                float2 uvFrac = float2(frac(i.uv.x), frac(i.uv.y));
                float2 id = floor(i.uv);

                float y = 0;
                float x = 0;
                fixed4 col = 0;

                for (y = -1; y <= 1; y++)
                {
                    for (x = -1; x <= 1; x++)
                    {
                        float2 offset = float2(x, y);
                        float noise = unity_noise_randomValue2D(id + offset);
                        float size = frac(noise * 123.32);
                        float warp = warp_tex(uvFrac - offset - float2(noise, frac(noise * 56.21)));
                        col += warp * size;
                    }
                }
                // --- Color and Emission added!!
                float vFade = abs(i.uv_mask.y - 0.5) * _VortexFade;
                col.rgb = (clamp(col.rgb - vFade, 0, 1) * _Color.rgb) * _EmissionColor;
                col.a = clamp(col.a - vFade, 0, 1) * _Color.a;
                
                return col;
            }
            ENDHLSL
        }
    }
}