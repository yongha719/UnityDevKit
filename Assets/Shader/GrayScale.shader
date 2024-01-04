Shader"Custom/GrayScale"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _GrayScaleValue("Gray Scale", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Standard

sampler2D _MainTex;
fixed _GrayScaleValue;
        
struct Input
{
    float2 uv_MainTex;
};
        
void surf(Input IN, inout SurfaceOutputStandard o)
{
                    // Albedo comes from a texture tinted by color
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                    
    float grayscale = (c.r + c.g + c.b) / 3;
    o.Emission = lerp(c, grayscale, _GrayScaleValue);
}
        ENDCG
    }
Fallback"Diffuse"
}
