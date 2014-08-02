Shader "Particles/Multiply" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend OneMinusDstColor One
	
	// ---- Fragment program cards
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			
			float4 _MainTex_ST;

			

			fixed4 frag (v2f_img i) : COLOR
			{
				 
				half4 prev = tex2D(_MainTex, i.uv);

				
				return prev * .2;
				
				
			}
			ENDCG 
		}
	} 	
	
	// ---- Dual texture cards
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
			SetTexture [_MainTex] {
				constantColor (1,1,1,1)
				combine previous lerp (previous) constant
			}
		}
	}
	
	// ---- Single texture cards (does not do particle colors)
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				constantColor (1,1,1,1)
				combine texture lerp(texture) constant
			}
		}
	}

	
}
}
