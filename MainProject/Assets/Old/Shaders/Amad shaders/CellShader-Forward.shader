Shader "Custom/CellShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Smooth ("Smoothing", Range(0,1)) = 0.55
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf CelShadingForward
		#pragma target 3.0

		half _Smooth;
		half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten)
		{
			half NdotL = dot(s.Normal, lightDir);

			NdotL = smoothstep(0, _Smooth, NdotL);

			half4 c;

			c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
			c.a = s.Alpha;

			return c;
		}

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
		 	o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
		}

		ENDCG
	}
	FallBack "Diffuse"
}
