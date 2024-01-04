Shader"Custom/AlbedoShader" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" { }
    }

    SubShader {
        Tags {"Queue" = "Overlay" }
LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

struct Input
{
    float2 uv_MainTex;
};

sampler2D _MainTex;

void surf(Input IN, inout SurfaceOutput o)
{
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

    o.Albedo = c.rgb;
}
        ENDCG
    }

FallBack"Diffuse"
}
