Shader "Custom/StarGateNoise"
{
    // This shader Variation changes the simple noise used for the shader to a cellnoise
    Properties
    {
       
        [Header(BASE Settings)]
         _BaseColor ("BaseColor", Color) = (1, 1, 1, 1)            // Base Color for Debug (not used here)
        _VortexFade ("Vortex Fade", Range(1, 2)) = 1                 // Vortex Fade on edges of warp and lines --> culling
        _EmissionStrength ("Emission Strength", Range(0, 50)) = 10  // change Emission Strength, more color impact here than in older version
        
        [Header(WARP Settings)]
        _WarpSize ("Warp Size", Range(0, 0.5)) = 0.1                // Warp Size line width --> makes lines overlap, general size
        _WarpDepth ("Warp Depth", Range(0, 2)) = 0.3                // Warp Size Line length
        _WarpSpeed ("Warp Speed", Range(0, 5)) = 1                  // Animation Speed
        _WarpTileX ("Warp Tile X", Range(1, 10)) = 10               // uv line width --> stretch lines
        _WarpTileY ("Warp Tile Y", Range(1, 10)) = 10               // uv line height --> stretch lines, used to animate, more intervals
        
        [Header(GRADIENT Settings)]
        _direction("Direction", Range(0, 1)) = 0                    // Direction of Gradient --> See Gradient Shader
        [HDR] _color1 ("Color 1", Color) = (1, 0.5, 0.5, 1)      // Color 1 for Gradient
        [HDR] _color2 ("Color 2", Color) = (0.5, 1, 1, 1)       // Color 2 for Gradient
        [HDR] _color3 ("Color 3", Color) = (0.5, 0.5, 0.4, 1)   // Color 3 for Gradient
        _threshold ("Threshold", Range(0, 1)) = 0                // Threshold Amount for Color 2 --> See Gradient Shader
        _smoothness ("Smoothness", Range(0, 1)) = 0              // Smoothness to Interpolate Colors --> See Gradient Shader
        
        [Header(NOISE Settings)]
        _CellSize ("Cell Size", Range(0.1, 10)) = 1             // Additional Cell Shader, cell size
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1         // General Noise Scale
        _NoiseSpeed ("Noise Speed", Range(0, 0.1)) = 0          // Animation Speed for Individual Noise
      
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "LightMode" = "UniversalForward"
        }
        Lod 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD1;
                float2 uv_mask : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD2;
                float2 uv : TEXCOORD1;
                float2 uv_mask : TEXCOORD1;
                
            };

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
            
            float _CellSize;
            float _NoiseScale;
            float _NoiseSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                o.uv_mask = v.uv_mask;
                return o;
            }

            float hash(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p += dot(p, p + 29.3);
                return frac(p.x * p.y * p.z);
            }
            
            float cellNoise(float3 position, float cellSize)
            {
                float speed = (_NoiseSpeed * _Time.y) / 10000;
                float3 p = floor(position / cellSize) * cellSize;
                float3 f = frac(position / cellSize);
                f = f * f * (3.0 - 2.0 * f);
                float a = hash(p + speed );
                float b = hash(p + float3(cellSize, 0, 0) + speed);
                float c = hash(p + float3(0, cellSize, 0) + speed );
                float d = hash(p + float3(cellSize, cellSize, 0) + speed);
                float ab = lerp(a, b, f.y);
                float cd = lerp(c, d, f.y);
                return lerp(ab, cd, f.x);
            }

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

            fixed4 frag (v2f i) : SV_Target
            {
                // Color Gradient
                float t = smoothstep(0.4, 0.6, _smoothness);
                float transition = smoothstep(_threshold - t, _threshold + t, abs(i.uv.y - 0.5));

                
                 _finalColor.r = ((_color1.r * i.uv.y * _direction + _color1.r * i.uv.x * (1 - _direction)) * transition)
                                + ((_color2.r * (1 - i.uv.y) * _direction + _color2.r * i.uv.x * (1 - _direction)) * (1 - transition))
                                + ((_color3.r * (1 - abs(i.uv.y + 0.5)) * _direction + _color3.r * i.uv.x * (1 - _direction)) * (1 - t));
                                 
                _finalColor.g = ((_color1.g * i.uv.y * _direction + _color1.g * i.uv.x * (1 - _direction)) * transition)
                                + ((_color2.g * (1 - i.uv.y) * _direction + _color2.g * i.uv.x * (1 - _direction)) * (1 - transition))
                                + ((_color3.g * (1 - abs(i.uv.y + 0.5)) * _direction + _color3.g * i.uv.x * (1 - _direction)) * (1 - t));
                
                _finalColor.b = ((_color1.b * i.uv.y * _direction + _color1.b * i.uv.x * (1 - _direction)) * transition)
                                + ((_color2.b * (1 - i.uv.y) * _direction + _color2.b * i.uv.x * (1 - _direction)) * (1 - transition))
                                + ((_color3.b * (1 - abs(i.uv.y + 0.5)) * _direction + _color3.b * i.uv.x * (1 - _direction)) * (1 - t));
                
                
                
                // Stripe Bending - manipulates gradient too
                 i.uv.x *= _WarpTileX;
                // i.uv.y *= _WarpTileY;
                
                //Animation over Time
                i.uv.y += (_Time.y * _WarpSpeed);
                float2 uvFrac = float2(frac(i.uv.x), frac(i.uv.y));
                float2 id = floor(i.uv);
                
                // --- NEW NOISE!
                float4 col = 0;
                
                float3 noiseOffset = float3(0.1, 0.3, 0.7);
                float3 noisePos = i.worldPos * _NoiseScale + noiseOffset;
               
                float noise = cellNoise(noisePos, _CellSize);
                col += noise;

                // Noise Greyscale
                float4 noiseColor = 0;
                //noiseColor.rgb = float3(noise, noise, noise);
                // Apply Gradient to Noise Color
                noiseColor.rgb = lerp(_finalColor, _color3.rgb, noiseColor.rgb);
                
                
                float warp = warp_tex(uvFrac - float2(noise, lerp(noise, 0, _VortexFade)));
                col += warp ;
                // cry
                
                //Apply Fade and Color
                float vFade = abs(i.uv_mask.y - 0.5) * _VortexFade;
                col.rgb = (clamp(col.rgb - vFade, 0, 1) * noiseColor.rgb) * _EmissionStrength;
                col.a = clamp(col.a - noise, 0, 1) * _BaseColor.a;
                // Cry more because fuck the gradient so much
                
                return col;
            }
            ENDCG
        }
    }
}