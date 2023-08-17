Shader "Custom/StencilShader"
{
    SubShader{
        Pass{
            ColorMask 0
            Zwrite Off
            
            Stencil{
                Ref 2
                comp Always
                Pass Replace
                }
            }
        }
    }
