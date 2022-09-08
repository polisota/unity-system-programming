Shader "Custom/RingShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _DensityMap ("Density Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _MinimumRenderDistance ("Minimum Render Distance", Float) = 10
        _MaximumFadeDistance ("Maximum Fade Distance", Float) = 20
        _InnerRingDiameter ("Inner Ring Diameter", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent"}
        LOD 200
        CULL OFF

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _DensityMap;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _MinimumRenderDistance;
        float _MaximumFadeDistance;
        float _InnerRingDiameter;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float distance = length(_WorldSpaceCameraPos - IN.worldPos);

            float2 position = float2((0.5 - IN.uv_MainTex.x) * 2, (0.5 - IN.uv_MainTex.y) * 2);
            float ringDistanceFromCenter = sqrt(position.x * position.x + position.y * position.y);

            clip(ringDistanceFromCenter - _InnerRingDiameter);
            clip(1 - ringDistanceFromCenter);
            clip(distance - _MinimumRenderDistance);

            fixed opacity = clamp((distance - _MinimumRenderDistance) / (_MaximumFadeDistance - _MinimumRenderDistance), 0, 1);

            fixed4 density = tex2D(_DensityMap, float2(clamp((ringDistanceFromCenter - _InnerRingDiameter) / (1 - _InnerRingDiameter), 0, 1), 0.5));
            fixed3 color = fixed3(position.x, position.y, density.a);
            o.Albedo = color;
            o.Metallic = _Metallic * opacity;
            o.Smoothness = _Glossiness * opacity;
            o.Alpha = opacity *density.a;
           
        }
        ENDCG
    }
    FallBack "Diffuse"
}
