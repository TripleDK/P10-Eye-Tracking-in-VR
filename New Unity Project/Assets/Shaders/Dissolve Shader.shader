Shader "Unlit/Dissolve Shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DissolveTexture ("Dissolve Texture", 2D) = "white"{}
		_DissolveY("Current Y of dissolve effect", float) = 0
		_DissolveSize("Size of the dissolve effect", float) = 2
		_StartingY("Starting Point of the effect", float) = -10
		_Color("Color of Dissolve", Color) = (0,0,1,0)
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
				float4 color : Color;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _DissolveTexture;
			float _DissolveY;
			float _DissolveSize;
			float _StartingY;
			float _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.color = _Color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float transition = _DissolveY - i.worldPos.y;
				clip(_StartingY + (transition + (tex2D(_DissolveTexture,i.uv))* _DissolveSize));

				fixed4 col = tex2D(_MainTex, i.uv) + i.color;
				return col;
			}
			ENDCG
		}
	}
}
