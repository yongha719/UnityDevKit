﻿Shader"Custom/ParticleRenderer" {
    Properties {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01, 3.0)) = 1.0
        _Brightness ("Particle Brightness", Range(0.1, 2.0)) = 1.0
    }

Category{
            Tags
{
"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane"
}
Blend SrcAlpha

OneMinusSrcAlpha
        ColorMask

RGB
        Cull

Off
        Lighting

Off
        ZWrite

Off

        SubShader
{
            Pass {

    CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile_particles

#include "UnityCG.cginc"

sampler2D _MainTex;
fixed4 _TintColor;

struct appdata_t
{
    float4 vertex : POSITION;
    fixed4 color : COLOR;
    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 vertex : POSITION;
    fixed4 color : COLOR;
    float2 texcoord : TEXCOORD0;
    float4 projPos : TEXCOORD1;
                    UNITY_VERTEX_OUTPUT_STEREO
};

float4 _MainTex_ST;
float _InvFade;
float _Brightness;

v2f vert(appdata_t v)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.color = v.color;
    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
    return o;
}

                UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

fixed4 frag(v2f i) : SV_Target
{
    return i.color * tex2D(_MainTex, i.texcoord) * i.color.a * _Brightness;
}
                ENDCG
        }
    }
}
}
