Shader "Custom/Combined Shader" {
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_OutlineColor("Outline color", Color) = (0,0,0,1)
		_OutlineWidth("Width of shader outline", Range(1.0,2.0)) = 1.02
		_DissolveColor("Dissolve Edge Color", Color) = (1,1,1,1)
		_DissolveTex("Dissolve Texture", 2D) = "white" {}
		_DissolveSize ("Dissolve Size", Range(0,1)) = 0.0
		_DissolveEdge("Dissolve Edge Size", Range(0,1)) = 0.0
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
	
		CGINCLUDE
		#include "UnityCG.cginc"
	
		sampler2D _MainTex;
		sampler2D _DissolveTex;

	

		float _OutlineWidth;
		fixed4 _OutlineColor;
		float _DissolveEdge;
		float _DissolveSize;
		float4 _DissolveColor;
		

		struct appdata
		{
			float4 vertex : POSITION;
			float3 normal: NORMAL;
		};

		struct v2f
		{
			float4 pos : POSITION;
			float4 color : COLOR;
			float3 normal: NORMAL;
		};

		v2f vert(appdata v)
		{
			v2f o;
			v.vertex.xyz *= _OutlineWidth;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.color = _OutlineColor;
			return o;
		}
		ENDCG

		Pass // render the outline 
		{
			ZWrite Off 

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			half4 frag(v2f i) : COLOR
			{
				return _OutlineColor;
			}
			ENDCG
		}

		Pass // Normal render
		{
			ZWrite On

			Material
			{
				Diffuse[_Color]
				Ambient[_Color]
			}

			Lighting On 

			SetTexture[_MainTex]
			{
				ConstantColor[_Color]
			}

			SetTexture[_MainTex]
			{
				Combine previous * primary DOUBLE
			}
		}
	
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		/*sampler2D _MainTex;
		sampler2D _DissolveTex;*/

		struct Input 
		{
			float2 uv_MainTex;
			float3 customData;
			float2 uv_DissolveTex;
			float3 viewDir;
		};

		/*float _DissolveEdge;
		float _DissolveSize;
		float4 _DissolveColor;*/

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half test = tex2D(_DissolveTex,IN.uv_DissolveTex).rgb -_DissolveSize;
			clip(test);
			
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			//	o.Albedo *= IN.customData;
			float intensity = abs(dot(normalize(IN.customData),normalize(IN.viewDir)));
			if(intensity < 0) 
			{
				intensity = 0;
			}

			if(test < _DissolveEdge && _DissolveSize > 0 && _DissolveSize < 1 ) 
			{
				o.Emission = _DissolveColor;
			}
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
