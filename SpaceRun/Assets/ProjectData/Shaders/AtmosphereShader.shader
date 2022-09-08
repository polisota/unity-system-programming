Shader "Custom/AtmosphereShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}

        _AtmosphereRadius("AtmosphereRadius", Range(1,1.5)) = 1.0
        _PlanetRadius("PlanetRadius", Range(0,1)) = 1.0
        _Ambient("Ambient", float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
            #pragma surface surf Standard fullforwardshadows
            #pragma target 3.0

            sampler2D _MainTex;

            struct Input
            {
                float2 uv_MainTex;
            };

            float _Ambient;
            float4 _Color;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                float4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color + _Ambient * _Color;
                o.Albedo = c.rgb;                
                o.Alpha = c.a;
            }
            ENDCG


        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
            LOD 200
            Cull Back
            Blend One One

            CGPROGRAM

            #pragma surface surf Standard vertex:vert
            #include <UnityPBSLighting.cginc>

            struct Input
            {
                float2 uv_MainTex;
                float3 worldPos;
                float3 centre;
            };

            float _AtmosphereRadius;
            float _PlanetRadius;

            void vert(inout appdata_full v, out Input o)
            {
                UNITY_INITIALIZE_OUTPUT(Input,o);
                v.vertex.xyz += v.normal * (_AtmosphereRadius - _PlanetRadius);
                o.centre = mul(unity_ObjectToWorld, half4(0,0,0,1));
            }

            void surf(Input IN, inout SurfaceOutputStandard o) {}
            ENDCG
    }           
}
