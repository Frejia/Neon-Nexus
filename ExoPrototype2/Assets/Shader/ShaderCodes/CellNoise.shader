Shader "Custom/URPCellNoiseLit"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _CellSize ("Cell Size", Range(0.1, 10)) = 1
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1

        _direction("Direction", Range(0, 1)) = 0
        _color1 ("Color 1", Color) = (1, 0.5, 0.5, 1)
        _color2 ("Color 2", Color) = (0.5, 1, 1, 1)
        _color3 ("Color 3", Color) = (0.5, 1, 1, 1)
        _threshold ("Threshold", Range(0, 1)) = 0
        _smoothness ("Smoothness", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD2;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                 float2 uv : TEXCOORD2;
            };

            float4 _BaseColor;
            float _CellSize;
            float _NoiseScale;
            
            float4 _color1;
            float4 _color2;
            float4 _color3; 
            float4 _finalColor;
            float _direction;
            float _threshold;
            float _smoothness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                 o.uv = v.uv;
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
                float3 p = floor(position / cellSize) * cellSize;
                float3 f = frac(position / cellSize);
                f = f * f * (3.0 - 2.0 * f);
                float a = hash(p);
                float b = hash(p + float3(cellSize, 0, 0));
                float c = hash(p + float3(0, cellSize, 0));
                float d = hash(p + float3(cellSize, cellSize, 0));
                float ab = lerp(a, b, f.x);
                float cd = lerp(c, d, f.x);
                return lerp(ab, cd, f.y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
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
                
                float3 noiseOffset = float3(0.1, 0.3, 0.7);
                float3 noisePos = i.worldPos * _NoiseScale + noiseOffset;
                float4 col = 0;
                float noise = cellNoise(noisePos, _CellSize);
                col += noise;
                
                // Noise Greyscale
                float4 noiseColor = 0;
                noiseColor.rgb = float3(noise, noise, noise);
                // Apply Gradient to Noise Color
                noiseColor.rgb = lerp(_color1, _color3, noiseColor.rgb);
                col.rgb = noiseColor.rgb;
                return col;
            }
            ENDCG
        }
    }
}