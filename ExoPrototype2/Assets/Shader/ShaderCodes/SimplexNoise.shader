Shader "Custom/URPCellNoiseLit"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _CellSize ("Cell Size", Range(0.1, 10)) = 1
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1
        _Specular ("Specular", Range(0, 1)) = 0.5
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
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
            float _Specular;
            float _Smoothness;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float wglnoise_mod289(float x)
            {
                return x - floor(x / 289) * 289;
            }

            float3 wglnoise_permute(float3 x)
            {
                return wglnoise_mod289((x * 34 + 1) * x);
            }

            float2 wglnoise_fade(float2 t)
            {
                return t * t * t * (t * (t * 6 - 15) + 10);
            }

           float3 SimplexNoiseGrad(float2 v)
{
    const float C1 = (3 - sqrt(3)) / 6;
    const float C2 = (sqrt(3) - 1) / 2;

    // First corner
    float2 i  = floor(v + dot(v, C2));
    float2 x0 = v -   i + dot(i, C1);

    // Other corners
    float2 i1 = x0.x > x0.y ? float2(1, 0) : float2(0, 1);
    float2 x1 = x0 + C1 - i1;
    float2 x2 = x0 + C1 * 2 - 1;

    // Permutations
    i = wglnoise_mod289(i); // Avoid truncation effects in permutation
    float3 p = wglnoise_permute(    i.y + float3(0, i1.y, 1));
           p = wglnoise_permute(p + i.x + float3(0, i1.x, 1));

    // Gradients: 41 points uniformly over a unit circle.
    // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
    float3 phi = p / 41 * 3.14159265359 * 2;
    float2 g0 = float2(cos(phi.x), sin(phi.x));
    float2 g1 = float2(cos(phi.y), sin(phi.y));
    float2 g2 = float2(cos(phi.z), sin(phi.z));

    // Compute noise and gradient at P
    float3 m  = float3(dot(x0, x0), dot(x1, x1), dot(x2, x2));
    float3 px = float3(dot(g0, x0), dot(g1, x1), dot(g2, x2));

    m = max(0.5 - m, 0);
    float3 m3 = m * m * m;
    float3 m4 = m * m3;

    float3 temp = -8 * m3 * px;
    float2 grad = m4.x * g0 + temp.x * x0 +
                  m4.y * g1 + temp.y * x1 +
                  m4.z * g2 + temp.z * x2;

    return 99.2 * float3(grad, dot(m4, px));
}

            /*float3 lighting(float3 baseColor, float3 normal, float3 viewDir)
            {
                float3 lightDir = normalize(float3(0.5, 1, 0.5));
                float3 halfDir = normalize(lightDir + viewDir);
                float NdotL = max(0, dot(normal, lightDir));
                float NdotH = max(0, dot(normal, halfDir));
                float3 diffuse = baseColor * NdotL;
                float3 specular = pow(NdotH, _Specular) * _Specular;
                return diffuse + specular;
            }*/

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = 0;
                float3 noise = SimplexNoiseGrad(i.worldPos.xy * _NoiseScale);
                col.rgb += noise.rgb;

                return col;
            }
            ENDCG
        }
    }
}