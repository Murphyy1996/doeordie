Shader "Custom/CellShaderPalletSwap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OverlayTex ("Color Mask", 2D) = "red" {}
		_Color1 ("Main", Color) = (1,0,0,1)
		_Color2 ("Secondary", Color) = (0,1,0,1)
		_Color3 ("Trim", Color) = (0,0,1,1)
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
		sampler2D _OverlayTex;
		fixed4 _Color1;
		fixed4 _Color2;
		fixed4 _Color3;

		sampler2D _NormalMap;


		struct Input
		{
			float2 uv_MainTex;
			float2 uv_OverlayTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			float4 c;
			float4 mask;

			c = tex2D(_MainTex, IN.uv_MainTex);
			mask = tex2D(_OverlayTex, IN.uv_OverlayTex);

			float4 dc = (_Color1 * mask.r) + (_Color2 * mask.g) + (_Color3 * mask.b);

			float m = max(mask.r,mask.g);
			m = max(m, mask.b);

			c = lerp(c, dc, m * c.a);
		 	o.Albedo = c;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
