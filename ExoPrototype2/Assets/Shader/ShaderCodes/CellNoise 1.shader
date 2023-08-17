Shader "Custom/URPCellNoiseLit"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _CellSize ("Cell Size", Range(0.1, 10)) = 1
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
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
                float3 noiseOffset = float3(0.1, 0.3, 0.7);
                float3 noisePos = i.worldPos * _NoiseScale + noiseOffset;
                float3 cellColor = cellNoise(noisePos, _CellSize);
                fixed4 finalColor = fixed4(cellColor , 1);
                finalColor.a = _BaseColor.a;
                return finalColor;
            }
            ENDCG
        }
    }
}