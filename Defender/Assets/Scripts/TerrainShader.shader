Shader "Custom/TerrainShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
_Tile1 ("Tile1 (RGB)", 2D) = "white" {}
_Tile2 ("Tile2 (RGB)", 2D) = "white" {}
_Tile3 ("Tile3 (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
sampler2D _Tile1;
sampler2D _Tile2;
sampler2D _Tile3;
        struct Input
        {
            float2 uv_MainTex;
float2 uv_Tile1;
float2 uv_Tile2;
float2 uv_Tile3;
        };


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
fixed4 Tile1 = tex2D (_Tile1, IN.uv_Tile1);
fixed4 Tile2 = tex2D (_Tile2, IN.uv_Tile2);
fixed4 Tile3 = tex2D (_Tile3, IN.uv_Tile3);
            o.Albedo = c.r * Tile1 + c.g * Tile2 + c.b * Tile3;
o.Smoothness = 0;
o.Metallic = 1;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
