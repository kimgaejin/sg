Shader "Custom/AlphaTest" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range (0,1)) = 0.5
	}

	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Lighting Off
		Cull Off
		//ZWrite Off
		Alphatest Greater [_Cutoff]
		Blend SrcAlpha OneMinusSrcAlpha 

		Pass {
		SetTexture [_MainTex] {
				combine texture
			}
		}
	}
}
