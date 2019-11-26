Shader "Knife/Post Processing/Outline"
{
    Properties
    {
		[HDR][PerRendererData]_OutlineColor ("Color", Color) = (1,1,1,1)
		_DepthOverlap ("Depth Overlap", Range(-1000, -1)) = -15
    }
    SubShader
    { 
		ZTest Less
		Offset -0.5, [_DepthOverlap]
        Tags { "RenderType" = "Opaque" }
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float4 finalColor : TEXCOORD;
            };
			
			float4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.finalColor = _OutlineColor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.finalColor;
            }
            ENDCG
        }
    }
}
