// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Gradient" {
    Properties {
        _direction("Direction", Range(0, 1)) = 0
        _color1 ("Color 1", Color) = (1, 0.5, 0.5, 1)
        _color2 ("Color 2", Color) = (0.5, 1, 1, 1)
        _color3 ("Color 3", Color) = (0.5, 1, 1, 1)
        _threshold ("Threshold", Range(0, 1)) = 0
        _smoothness ("Smoothness", Range(0, 1)) = 0
        _GradientTexture("Gradient Texture", 2D) = "white" {}
    }
    SubShader {
        Pass {        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            float4 _color1;
            float4 _color2;
            float4 _color3; 
            float4 _finalColor;
            float _direction;
            float _threshold;
            float _smoothness;
            sampler2D _GradientTexture;

            struct appdata {
                float4 vertex : POSITION;
                float4 tex : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 vertex : TEXCOORD;
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata v, v2f o) {
                o.pos = UnityObjectToClipPos(v.vertex);
                o.vertex = v.tex;
                return o;
            };

            half4 frag(v2f i) : COLOR {

                //Color transition from image
                //_finalColor.rgb = tex2D(_GradientTexture, float2(_direction, 0)).rgb;

                // Color Gradient Calculation
                // Calculate Smoothstep t for the color transition type (Sharp or smooth transition --> 0.4 to 0.6 are tested variables
                float t = smoothstep(0.4, 0.6, _smoothness);
                // Transition from color 1 to color 2 to 3 defines the amount of the color 2 that is then manipulated by the smoothstep t
                float transition = smoothstep(_threshold - t, _threshold + t, abs(i.vertex.y - 0.5));

                // Color Alpha is 1
				_finalColor.a = 1;

                // Based on this git: https://gist.github.com/goodmorningcmdr/09f15417f103f91f89994983ce2dfd3f
                _finalColor.r = ((_color1.r * i.vertex.y * _direction + _color1.r * i.vertex.x * (1 - _direction)) * transition)
                                + ((_color2.r * (1 - i.vertex.y) * _direction + _color2.r * i.vertex.x * (1 - _direction)) * (1 - transition))
                                + ((_color3.r * (1 - abs(i.vertex.y + 0.5)) * _direction + _color3.r * i.vertex.x * (1 - _direction)) * (1 - t));
                                 
                _finalColor.g = ((_color1.g * i.vertex.y * _direction + _color1.g * i.vertex.x * (1 - _direction)) * transition)
                                + ((_color2.g * (1 - i.vertex.y) * _direction + _color2.g * i.vertex.x * (1 - _direction)) * (1 - transition))
                                + ((_color3.g * (1 - abs(i.vertex.y + 0.5)) * _direction + _color3.g * i.vertex.x * (1 - _direction)) * (1 - t));
                
                _finalColor.b = ((_color1.b * i.vertex.y * _direction + _color1.b * i.vertex.x * (1 - _direction)) * transition)
                                + ((_color2.b * (1 - i.vertex.y) * _direction + _color2.b * i.vertex.x * (1 - _direction)) * (1 - transition))
                                + ((_color3.b * (1 - abs(i.vertex.y + 0.5)) * _direction + _color3.b * i.vertex.x * (1 - _direction)) * (1 - t));
                
                return _finalColor;
                
            };
            ENDCG
        }
    } 
    FallBack "Diffuse"
}