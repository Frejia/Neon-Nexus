Shader "Unlit/StarGateGradient" {
    
    // This shader variant adds a Gradient Color to the StarGate Warp Effect
    Properties {
        
        [Header(BASE Settings)]
         _BaseColor ("BaseColor", Color) = (1, 1, 1, 1)
        _VortexFade ("Vortex Fade", Range(1, 2)) = 1
        _EmissionStrength ("Emission Strength", Range(0, 50)) = 10
        
        [Header(WARP Settings)]
        _WarpSize ("Warp Size", Range(0, 0.5)) = 0.1
        _WarpDepth ("Warp Depth", Range(0, 2)) = 0.3
        _WarpSpeed ("Warp Speed", Range(0, 5)) = 1
        _WarpTileX ("Warp Tile X", Range(1, 10)) = 10
        _WarpTileY ("Warp Tile Y", Range(1, 10)) = 10
        
        [Header(Gradient Settings)]
        _direction("Direction", Range(0, 1)) = 0
        [HDR] _color1 ("Color 1", Color) = (1, 0.5, 0.5, 1)
        [HDR] _color2 ("Color 2", Color) = (0.5, 1, 1, 1)
        [HDR] _color3 ("Color 3", Color) = (0.5, 0.5, 0.4, 1)
        _threshold ("Threshold", Range(0, 1)) = 0
        _smoothness ("Smoothness", Range(0, 1)) = 0
        
        //EmissionGradient
        //[HDR] _Emiscolor1 ("Color 1", Color) = (1, 0.5, 0.5, 1)
        //[HDR] _Emiscolor2 ("Color 2", Color) = (0.5, 1, 1, 1)
    }
    SubShader {
          Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "LightMode" = "UniversalForward"
        }
        LOD 100
          
        Pass {        
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _BaseColor;
            float _EmissionStrength;
            float _VortexFade;
            float _WarpSize;
            float _WarpDepth;
            float _WarpSpeed;
            float _WarpTileX;
            float _WarpTileY;
            float4 _color1;
            float4 _color2;
            float4 _color3;
            float4 _finalColor;
            float _direction;
            float _threshold;
            float _smoothness;
            
            
            struct appdata {
                float4 vertex : POSITION;
                float4 tex : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float2 uv_mask : TEXCOORD1;
            };

            struct v2f {
                float2 uv : TEXCOORD1;
                float2 uv_mask : TEXCOORD1;
                float4 vertex : TEXCOORD;
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            
            v2f vert(appdata v, v2f o) {
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv_mask = v.uv_mask;
                o.vertex = v.tex;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            };

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
            
            half4 frag(v2f i) : SV_Target {
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

                // --- GRADIENT!!!
                float t = smoothstep(0.4, 0.6, _smoothness);
                float transition = smoothstep(_threshold - t, _threshold + t, abs(i.vertex.y - 0.5));

                
                 _finalColor.r = ((_color1.r * i.vertex.y * _direction + _color1.r * i.vertex.x * (1 - _direction)) * transition)
                                + ((_color2.r * (1 - i.vertex.y) * _direction + _color2.r * i.vertex.x * (1 - _direction)) * (1 - transition))
                                + ((_color3.r * (1 - abs(i.vertex.y + 0.5)) * _direction + _color3.r * i.vertex.x * (1 - _direction)) * (1 - t));
                                 
                _finalColor.g = ((_color1.g * i.vertex.y * _direction + _color1.g * i.vertex.x * (1 - _direction)) * transition)
                                + ((_color2.g * (1 - i.vertex.y) * _direction + _color2.g * i.vertex.x * (1 - _direction)) * (1 - transition))
                                + ((_color3.g * (1 - abs(i.vertex.y + 0.5)) * _direction + _color3.g * i.vertex.x * (1 - _direction)) * (1 - t));
                
                _finalColor.b = ((_color1.b * i.vertex.y * _direction + _color1.b * i.vertex.x * (1 - _direction)) * transition)
                                + ((_color2.b * (1 - i.vertex.y) * _direction + _color2.b * i.vertex.x * (1 - _direction)) * (1 - transition))
                                + ((_color3.b * (1 - abs(i.vertex.y + 0.5)) * _direction + _color3.b * i.vertex.x * (1 - _direction)) * (1 - t));
                
                float vFade = abs(i.uv_mask.y - 0.5) * _VortexFade;
                col.rgb = (clamp(col.rgb - vFade, 0, 1) * _finalColor.rgb) * _EmissionStrength;
                col.a = clamp(col.a - vFade, 0, 1) * _BaseColor.a;
                
                return col;
                
            };
            ENDCG
        }
    } 
    FallBack "WarpDrive"
}