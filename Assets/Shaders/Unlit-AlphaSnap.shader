// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/Transparent Snap" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_AltTex("Alterate (RGB) Trans (A)", 2D) = "white" {}
	_Blend("Blend", Range(0, 1)) = 0.0
	_Cutoff ("Alpha cutoff", Range(0, 1)) = 1.0
	[MaterialToggle] PixelSnap ("Pixel snap", Float) = 1
}
SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 100

	Lighting Off

	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _AltTex;
			fixed _Blend;
			fixed _Cutoff;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				#ifdef PIXELSNAP_ON
					o.vertex = UnityPixelSnap (o.vertex);
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				fixed4 alt = tex2D(_AltTex, i.texcoord);
				col = lerp(col, alt, _Blend);
				clip(col.a - _Cutoff);
				return col;
			}
		ENDCG
	}
}

}
