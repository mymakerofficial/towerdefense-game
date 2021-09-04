Shader "Outline/Custom" {
	Properties {
		//Color for the albedo
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		//Color of the outline
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		//Width of the outline
		_Outline ("Outline width", Range (0, 1)) = .1
		//Texture to use
		_MainTex ("Base (RGB)", 2D) = "white" { }
	}
 
	CGINCLUDE
	#include "UnityCG.cginc"

	//Standard stuff, don't touch it or it complains
	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
 
	struct v2f {
		float4 pos : POSITION;
		float4 color : COLOR;
	};

	//Outline width
	uniform float _Outline;
	//Outline color
	uniform float4 _OutlineColor;
 
	v2f vert(appdata v) {
		// just make a copy of incoming vertex data but scaled according to normal direction
		v2f o;

		//Grow and flip the vertex
		v.vertex *= -( 1 + _Outline);
	
		o.pos = UnityObjectToClipPos(v.vertex);

		//Use desired color
		o.color = _OutlineColor;

		//Yeet
		return o;
	}
	ENDCG
 
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True"}
		Cull Back
		CGPROGRAM
		#pragma surface surf Lambert
		 
		sampler2D _MainTex;
		fixed4 _Color;
		 
		struct Input {
			float2 uv_MainTex;
		};
		 
		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG

		Pass {
			Name "OUTLINE"
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True"}
			Cull Back
			ZWrite Off
			//ZTest Less
			Offset 1, 1
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			//Do nothing to the fragment
			half4 frag(v2f i) :COLOR { return i.color; }
			ENDCG
		}
	}
 
	Fallback "Diffuse"
}