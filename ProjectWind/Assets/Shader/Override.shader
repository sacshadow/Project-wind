Shader "Custom/Override" {
	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_Color2 ("Main Color2", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MainTex2 ("Base2 (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Geometry+1" "IgnoreProjector"="True" "RenderType"="Geometry"}
		LOD 200
		Cull Back
		
		Pass{
			ZWrite Off
			ZTest GEqual
			//Lighting Off
			
			
			Color[_Color2]
			
			SetTexture [_MainTex] {
				combine texture * primary, texture
			}
		}
		
		Pass{
			ZWrite On
			//ZTest Less 
			Lighting On
			
			Material {
                Diffuse [_Color]
                Ambient [_Color]
            }
			
			SetTexture [_MainTex2] {
				combine texture * primary, texture
			}
		}
		
		
		
	}

	FallBack "Diffuse"
}
