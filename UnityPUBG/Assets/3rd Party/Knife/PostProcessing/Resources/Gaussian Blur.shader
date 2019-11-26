Shader "Knife/Post Processing/Gaussian Blur" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Pass
		{
			ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			
		    struct appdata_t
            {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};   
             
  			v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;
                
                return OUT;
            }

			uniform sampler2D _MainTex;
			float4 _u_Scale;

			float4 frag (v2f in1) : SV_TARGET
			{
				const float2 gaussFilter[7] =
				{
					-3.0,	0.015625,
					-2.0,	0.09375,
					-1.0,	0.234375,
					0.0,	0.3125,
					1.0,	0.234375,
					2.0,	0.09375,
					3.0,	0.015625
				};

				float4 camera = float4(0,0,0,0);
				float4 tex = tex2D(_MainTex, in1.uv).rgba;

				float sum = 0.0f;
				for (int i = 0; i < 7; i++) {
					float2 filter = float2(gaussFilter[i].x, gaussFilter[i].x);	
					float4 color = tex2D(_MainTex, in1.uv + filter*_u_Scale.xy);

					float alpha = gaussFilter[i].y * tex.a;
					sum += alpha;
					camera.rgb += color.rgb * alpha;
				}

				camera.rgb /= sum;
				return float4(camera.rgb, tex.a);
	
			}
			
			ENDCG
		}
		
	}
}