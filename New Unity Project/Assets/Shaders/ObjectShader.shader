// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'



Shader "Custom/ObjectShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_DissolveColor("Dissolve Edge Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DissolveTex("Dissolve Texture", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_DissolveSize ("Dissolve Size", Range(0,1)) = 0.0
		_DissolveEdge("Dissolve Edge Size", Range(0,1)) = 0.0
		_OutlineSize("Outline Thickness", Range(0,1)) = 0.0
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		struct Input {
			float3 customData;
			float2 uv_MainTex;
			float2 uv_DissolveTex;
			float3 viewDir;
		};
		sampler2D _MainTex;
		sampler2D _DissolveTex;
		
		float _DissolveEdge;
		float _DissolveSize;
		float _OutlineSize;
		float4 _OutlineColor;
		float4 _DissolveColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.customData = (v.normal);
			o.customData = mul(unity_ObjectToWorld,fixed4(o.customData,0));
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half test = tex2D(_DissolveTex,IN.uv_DissolveTex).rgb -_DissolveSize;
			clip(test);
			
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) ;//* _Color;
			o.Albedo = c.rgb;
			//	o.Albedo *= IN.customData;
			float intensity = abs(dot(normalize(IN.customData),normalize(IN.viewDir)));
			if(intensity < 0) 
				intensity = 0;
			//	o.Albedo.rgb = float3(intensity,intensity,intensity);
			if(intensity < _OutlineSize) {
				o.Albedo = _OutlineColor;
			}

			if(test < _DissolveEdge && _DissolveSize > 0 && _DissolveSize < 1 ) {
				o.Emission = _DissolveColor;
			}
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
