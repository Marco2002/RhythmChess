Shader "Custom/ShadowShader" {
	 Properties {
	     _Color ( "Tint", Color ) = ( 0, 0, 0, 0.15 )
	 }
	 
	 SubShader {
	    Tags { "Queue"="Transparent"
	    	"IgnoreProjector" = "True"
          	"RenderType" = "TransparentCutout"
          	"PreviewType" = "Plane"
          	"CanUseSpriteAtlas" = "True" }
	     
		Pass {
		    Stencil {
		        Ref 4
		        Comp NotEqual
		        Pass Replace
		    }
		    Cull Off
      		Lighting Off
      		ZWrite Off

		     Blend SrcAlpha OneMinusSrcAlpha     
	 
			 CGPROGRAM
			 #pragma vertex vert
			 #pragma fragment frag
			 #include "UnityCG.cginc"
			 
			 uniform sampler2D _MainTex;
			 
			 struct v2f {
			     half4 pos : POSITION;
			     half2 uv : TEXCOORD0;
			     fixed4 color : COLOR;
			 };

			 fixed4 _Color;

			 v2f vert(appdata_img v) {
			     v2f o;
			     o.pos = UnityObjectToClipPos (v.vertex);
			     half2 uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );
			     o.uv = uv;
			     o.color = _Color;
			     return o;
			 }

			 half4 frag (v2f i) : COLOR {
				return _Color;
			}
			 ENDCG
		}

	}
 
	Fallback off
}