Shader "Custom/LambertEmissionShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Emission("Emission", Color) = (1,1,1,1)
		_Height("Height", Range(0,10)) = 0.0
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd noshadow
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		fixed4 _Color;
		float4 _Emission;
		float _Height;

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 color = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = color.rgb;
			o.Emission = _Emission.xyz;
			o.Alpha = color.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
