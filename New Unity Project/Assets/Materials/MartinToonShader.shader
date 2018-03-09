// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MartinToonShader" {
	 Properties {
   	  _MainTex ("Main Texture", 2D) = "white" {}
	  _Color ("Diffuse Color", Color) = (1,1,1,1) 
      _UnlitColor ("Unlit Diffuse Color", Color) = (0.5,0.5,0.5,1) 
      _DiffuseThreshold ("Threshold for Diffuse Colors", Range(0,1)) 
         = 0.1 
      _OutlineColor ("Outline Color", Color) = (0,0,0,1)
      _LitOutlineThickness ("Lit Outline Thickness", Range(0,1)) = 0.1
      _UnlitOutlineThickness ("Unlit Outline Thickness", Range(0,1)) 
         = 0.4
      _SpecColor ("Specular Color", Color) = (1,1,1,1) 
      _Shininess ("Shininess", Float) = 10
	  _DissolveColor("Dissolve Edge Color", Color) = (1,1,1,1)
	  _DissolveTex("Dissolve Texture", 2D) = "white" {}
	  _DissolveSize ("Dissolve Size", Range(0,1)) = 0.0
	  _DissolveEdge("Dissolve Edge Size", Range(0,1)) = 0.0
	  _ColorFiltering("Color Filter", Range(0,255)) = 0
   }
   SubShader {
      Pass {      
         Tags { "LightMode" = "ForwardBase" } 
            // pass for ambient light and first light source
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
         uniform float4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
 
         // User-specified properties
         uniform float4 _Color; 
         uniform float4 _UnlitColor;
         uniform float _DiffuseThreshold;
         uniform float4 _OutlineColor;
         uniform float _LitOutlineThickness;
         uniform float _UnlitOutlineThickness;
         uniform float4 _SpecColor; 
         uniform float _Shininess;
		 uniform float _DissolveEdge;
		 uniform float _DissolveSize;
		 uniform float4 _DissolveColor;
		 uniform float _ColorFiltering;
		 sampler2D _DissolveTex;
		 sampler2D _MainTex;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
            float3 normalDir : TEXCOORD1;
			float4 tex : TEXCOORD2;
         };
 
	
        int RoundUp(int numToRound, int multiple){
    		if (multiple == 0)
        		return numToRound;

		    int remainder = abs(numToRound) % multiple;
    		if (remainder == 0)
        		return numToRound;

		    if (numToRound < 0)
        		return -(abs(numToRound) - remainder);
    		else
        		return numToRound + multiple - remainder;
			
			//return floor(numToRound / (1 / multiple)) * (1 / multiple);
		}	

         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 			output.tex = input.texcoord;

            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject; 
 
            output.posWorld = mul(modelMatrix, input.vertex);
            output.normalDir = normalize(
               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            //Dissolve
			half test = tex2D(_DissolveTex,input.tex.xy).rgb -_DissolveSize;
			clip(test);
			
			float3 normalDirection = normalize(input.normalDir);
 
            float3 viewDirection = normalize(
               _WorldSpaceCameraPos - input.posWorld.xyz);
            float3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = 
                  _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 			 //Main Texture color
  			float3 mainColor = tex2D(_MainTex, input.tex.xy).rgb;
 			mainColor.r = RoundUp(mainColor.r*255, _ColorFiltering)/255.0;
  			mainColor.g = RoundUp(mainColor.g*255, _ColorFiltering)/255.0;
			mainColor.b = RoundUp(mainColor.b*255, _ColorFiltering)/255.0;
  

            // default: unlit 
            float3 fragmentColor = _UnlitColor.rgb * mainColor/2; 
            // low priority: diffuse illumination
            if (attenuation 
               * max(0.0, dot(normalDirection, lightDirection)) 
               >= _DiffuseThreshold)
            {
               fragmentColor = _LightColor0.rgb * _Color.rgb * mainColor/2; 
            }
 
            // higher priority: outline
            if (dot(viewDirection, normalDirection) 
               < lerp(_UnlitOutlineThickness, _LitOutlineThickness, 
               max(0.0, dot(normalDirection, lightDirection))))
            {
               fragmentColor = _LightColor0.rgb * _OutlineColor.rgb * mainColor/2; 
            }
 
            // highest priority: highlights
            if (dot(normalDirection, lightDirection) > 0.0 
               // light source on the right side?
               && attenuation *  pow(max(0.0, dot(
               reflect(-lightDirection, normalDirection), 
               viewDirection)), _Shininess) > 0.5) 
               // more than half highlight intensity? 
            {
               fragmentColor = _SpecColor.a 
                  * _LightColor0.rgb * _SpecColor.rgb
                  + (1.0 - _SpecColor.a) * fragmentColor;
            }
 			if(test < _DissolveEdge &&_DissolveSize > 0 && _DissolveSize < 1) {
				fragmentColor = _DissolveColor;
			}
            return float4(fragmentColor, 1.0);
         }
         ENDCG
      }
 
      Pass {      
         Tags { "LightMode" = "ForwardAdd" } 
            // pass for additional light sources
         Blend SrcAlpha OneMinusSrcAlpha 
            // blend specular highlights over framebuffer
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
         uniform float4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
 
         // User-specified properties
         uniform float4 _Color; 
         uniform float4 _UnlitColor;
         uniform float _DiffuseThreshold;
         uniform float4 _OutlineColor;
         uniform float _LitOutlineThickness;
         uniform float _UnlitOutlineThickness;
         uniform float4 _SpecColor; 
         uniform float _Shininess;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
            float3 normalDir : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject;
 
            output.posWorld = mul(modelMatrix, input.vertex);
            output.normalDir = normalize(
               mul(float4(input.normal, 0.0), modelMatrixInverse).rgb);
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
         
		    float3 normalDirection = normalize(input.normalDir);
 
            float3 viewDirection = normalize(
               _WorldSpaceCameraPos - input.posWorld.rgb);
            float3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = 
                  _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
            float4 fragmentColor = float4(0.0, 0.0, 0.0, 0.0);
            if (dot(normalDirection, lightDirection) > 0.0 
               // light source on the right side?
               && attenuation *  pow(max(0.0, dot(
               reflect(-lightDirection, normalDirection), 
               viewDirection)), _Shininess) > 0.5) 
               // more than half highlight intensity? 
            {
               fragmentColor = 
                  float4(_LightColor0.rgb, 1.0) * _SpecColor;
            }
            return fragmentColor;
         }
         ENDCG
      }
   } 
   Fallback "Specular"
}