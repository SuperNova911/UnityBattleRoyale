Shader "Knife/Post Processing/Outline Effect" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#pragma shader_feature DEPTH_CULLING
			#pragma shader_feature CORNER_OUTLINES

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _OutlineSource;
			sampler2D _OutlineDepth;

			struct v2f 
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert(appdata_img v)
			{
			   	v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				
			   	return o;
			}

			float _LineThicknessX;
			float _LineThicknessY;
			float _FillAmount;
			int _DrawOnlyVisiblePart;
			int _DepthDiffSign;
			float _OutlineGlobalIntensity;
			uniform float4 _MainTex_TexelSize;
			sampler2D _CameraDepthTexture;
			float _overGlow;

			half4 frag (v2f input) : COLOR
			{	
				const float thershold = .01;

				half4 originalPixel = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(input.uv, _MainTex_ST));
				if(_OutlineGlobalIntensity == 0)
					return originalPixel;

				float2 uv = input.uv;
				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						uv.y = 1 - uv.y;
				#endif
				
				float4 outlineSource = tex2D(_OutlineSource, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST));
						
				half4 outline = 0;
				bool hasOutline = false;

				half4 sample1 = tex2D(_OutlineSource, uv + float2(_LineThicknessX,0.0));
				half4 sample2 = tex2D(_OutlineSource, uv + float2(-_LineThicknessX,0.0));
				half4 sample3 = tex2D(_OutlineSource, uv + float2(.0,_LineThicknessY));
				half4 sample4 = tex2D(_OutlineSource, uv + float2(.0,-_LineThicknessY));

				#if DEPTH_CULLING
					half4 depth = Linear01Depth(tex2D(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST)));
					half4 outlineDepth = Linear01Depth(tex2D(_OutlineDepth, UnityStereoScreenSpaceUVAdjust(input.uv, _MainTex_ST)));
					float diff = depth.r - outlineDepth.r;
					diff *= _DepthDiffSign;

					diff = smoothstep(diff, sign(diff), 1);
					
					outlineSource = lerp(outlineSource, outlineSource * diff, _DrawOnlyVisiblePart);

					half dosample1 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(_LineThicknessX,0.0)).r);
					half dosample2 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(-_LineThicknessX,0.0)).r);
					half dosample3 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(.0,_LineThicknessY)).r);
					half dosample4 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(.0,-_LineThicknessY)).r);

					half dsample1 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(_LineThicknessX,0.0)).r);
					half dsample2 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(-_LineThicknessX,0.0)).r);
					half dsample3 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(.0,_LineThicknessY)).r);
					half dsample4 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(.0,-_LineThicknessY)).r);
				
					half diff1 = dsample1 - dosample1;
					half diff2 = dsample2 - dosample2;
					half diff3 = dsample3 - dosample3;
					half diff4 = dsample4 - dosample4;
						
					diff1 *= _DepthDiffSign;
					diff2 *= _DepthDiffSign;
					diff3 *= _DepthDiffSign;
					diff4 *= _DepthDiffSign;
				
					diff1 = smoothstep(diff1, sign(diff1), 1);
					diff2 = smoothstep(diff2, sign(diff2), 1);
					diff3 = smoothstep(diff3, sign(diff3), 1);
					diff4 = smoothstep(diff4, sign(diff4), 1);
				
					sample1 = sample1 * diff1;
					sample2 = sample2 * diff2;
					sample3 = sample3 * diff3;
					sample4 = sample4 * diff4;
				#endif

				//bool outside = outlineSource.a < thershold;

				#if CORNER_OUTLINES
					half4 sample5 = tex2D(_OutlineSource, uv + float2(_LineThicknessX, _LineThicknessY));
					half4 sample6 = tex2D(_OutlineSource, uv + float2(-_LineThicknessX, -_LineThicknessY));
					half4 sample7 = tex2D(_OutlineSource, uv + float2(_LineThicknessX, -_LineThicknessY));
					half4 sample8 = tex2D(_OutlineSource, uv + float2(-_LineThicknessX, _LineThicknessY));
					
					#if DEPTH_CULLING
						half4 dosample5 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(_LineThicknessX, _LineThicknessY)).r);
						half4 dosample6 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(-_LineThicknessX, -_LineThicknessY)).r);
						half4 dosample7 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(_LineThicknessX, -_LineThicknessY)).r);
						half4 dosample8 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(-_LineThicknessX, _LineThicknessY)).r);

						half4 dsample5 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(_LineThicknessX, _LineThicknessY)).r);
						half4 dsample6 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(-_LineThicknessX, -_LineThicknessY)).r);
						half4 dsample7 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(_LineThicknessX, -_LineThicknessY)).r);
						half4 dsample8 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(-_LineThicknessX, _LineThicknessY)).r);
						
						half diff5 = dsample5 - dosample5;
						half diff6 = dsample6 - dosample6;
						half diff7 = dsample7 - dosample7;
						half diff8 = dsample8 - dosample8;
						
						diff5 *= _DepthDiffSign;
						diff6 *= _DepthDiffSign;
						diff7 *= _DepthDiffSign;
						diff8 *= _DepthDiffSign;

						diff5 = smoothstep(diff5, sign(diff5), 1);
						diff6 = smoothstep(diff6, sign(diff6), 1);
						diff7 = smoothstep(diff7, sign(diff7), 1);
						diff8 = smoothstep(diff8, sign(diff8), 1);

						sample5 = sample5 * diff5;
						sample6 = sample6 * diff6;
						sample7 = sample7 * diff7;
						sample8 = sample8 * diff8;
					#endif

					if (sample1.a > thershold || sample2.a > thershold || sample3.a > thershold || sample4.a > thershold || sample5.a > thershold || sample6.a > thershold || sample7.a > thershold || sample8.a > thershold)
					{
						outline = max(sample1 * sample1.a, sample2 * sample2.a);
						outline = max(outline, sample3 * sample3.a);
						outline = max(outline, sample4 * sample4.a);
						outline = max(outline, sample5 * sample5.a);
						outline = max(outline, sample6 * sample6.a);
						outline = max(outline, sample7 * sample7.a);
						outline = max(outline, sample8 * sample8.a);
						hasOutline = true;
					}

					outline = lerp(outline, outline * _FillAmount, outlineSource.a);
				#else
					if (sample1.a > thershold || sample2.a > thershold || sample3.a > thershold || sample4.a > thershold)
					{
						outline = max(sample1 * sample1.a, sample2 * sample2.a);
						outline = max(outline, sample3 * sample3.a);
						outline = max(outline, sample4 * sample4.a);
						hasOutline = true;
					}
					
					outline = lerp(outline, outline * _FillAmount, outlineSource.a);
				#endif
				
				outline *= _overGlow;
				if (hasOutline)
				{
					float4 addPixel = lerp(0, originalPixel, outlineSource.a);
					float4 outlineValue = lerp(originalPixel, addPixel + outline, clamp((outline.r + outline.g + outline.b + outline.a), 0, 1));
					return lerp(originalPixel, outlineValue, _OutlineGlobalIntensity);
				}
				else
					return originalPixel;
			}
			
			ENDCG
		}
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#pragma shader_feature DEPTH_CULLING
			#pragma shader_feature CORNER_OUTLINES
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _OutlineSource;
			sampler2D _OutlineDepth;

			struct v2f 
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert(appdata_img v)
			{
			   	v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				
			   	return o;
			}

			float _LineThicknessX;
			float _LineThicknessY;
			float _FillAmount;
			int _DrawOnlyVisiblePart;
			float _OutlineGlobalIntensity;
			uniform float4 _MainTex_TexelSize;
			sampler2D _CameraDepthTexture;
			int _DepthDiffSign;

			half4 frag (v2f input) : COLOR
			{	
				const float thershold = .01;

				half4 originalPixel = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(input.uv, _MainTex_ST));
				if(_OutlineGlobalIntensity == 0)
					return originalPixel;

				float2 uv = input.uv;
				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						uv.y = 1 - uv.y;
				#endif
				
				float4 outlineSource = tex2D(_OutlineSource, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST));
						
				half4 outline = 0;
				bool hasOutline = false;

				half4 sample1 = tex2D(_OutlineSource, uv + float2(_LineThicknessX,0.0));
				half4 sample2 = tex2D(_OutlineSource, uv + float2(-_LineThicknessX,0.0));
				half4 sample3 = tex2D(_OutlineSource, uv + float2(.0,_LineThicknessY));
				half4 sample4 = tex2D(_OutlineSource, uv + float2(.0,-_LineThicknessY));

				#if DEPTH_CULLING
					half4 depth = Linear01Depth(tex2D(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST)));
					half4 outlineDepth = Linear01Depth(tex2D(_OutlineDepth, UnityStereoScreenSpaceUVAdjust(input.uv, _MainTex_ST)));
					float diff = depth.r - outlineDepth.r;
					diff *= _DepthDiffSign;

					diff = smoothstep(diff, sign(diff), 1);
					
					outlineSource = lerp(outlineSource, outlineSource * diff, _DrawOnlyVisiblePart);

					half dosample1 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(_LineThicknessX,0.0)).r);
					half dosample2 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(-_LineThicknessX,0.0)).r);
					half dosample3 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(.0,_LineThicknessY)).r);
					half dosample4 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(.0,-_LineThicknessY)).r);

					half dsample1 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(_LineThicknessX,0.0)).r);
					half dsample2 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(-_LineThicknessX,0.0)).r);
					half dsample3 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(.0,_LineThicknessY)).r);
					half dsample4 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(.0,-_LineThicknessY)).r);
				
					half diff1 = dsample1 - dosample1;
					half diff2 = dsample2 - dosample2;
					half diff3 = dsample3 - dosample3;
					half diff4 = dsample4 - dosample4;
						
					diff1 *= _DepthDiffSign;
					diff2 *= _DepthDiffSign;
					diff3 *= _DepthDiffSign;
					diff4 *= _DepthDiffSign;
				
					diff1 = smoothstep(diff1, sign(diff1), 1);
					diff2 = smoothstep(diff2, sign(diff2), 1);
					diff3 = smoothstep(diff3, sign(diff3), 1);
					diff4 = smoothstep(diff4, sign(diff4), 1);
				
					sample1 = sample1 * diff1;
					sample2 = sample2 * diff2;
					sample3 = sample3 * diff3;
					sample4 = sample4 * diff4;
				#endif

				bool outside = outlineSource.a < thershold;

				#if CORNER_OUTLINES
					half4 sample5 = tex2D(_OutlineSource, uv + float2(_LineThicknessX, _LineThicknessY));
					half4 sample6 = tex2D(_OutlineSource, uv + float2(-_LineThicknessX, -_LineThicknessY));
					half4 sample7 = tex2D(_OutlineSource, uv + float2(_LineThicknessX, -_LineThicknessY));
					half4 sample8 = tex2D(_OutlineSource, uv + float2(-_LineThicknessX, _LineThicknessY));
					
					#if DEPTH_CULLING
						half4 dosample5 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(_LineThicknessX, _LineThicknessY)).r);
						half4 dosample6 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(-_LineThicknessX, -_LineThicknessY)).r);
						half4 dosample7 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(_LineThicknessX, -_LineThicknessY)).r);
						half4 dosample8 = Linear01Depth(tex2D(_OutlineDepth, uv + float2(-_LineThicknessX, _LineThicknessY)).r);

						half4 dsample5 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(_LineThicknessX, _LineThicknessY)).r);
						half4 dsample6 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(-_LineThicknessX, -_LineThicknessY)).r);
						half4 dsample7 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(_LineThicknessX, -_LineThicknessY)).r);
						half4 dsample8 = Linear01Depth(tex2D(_CameraDepthTexture, uv + float2(-_LineThicknessX, _LineThicknessY)).r);
						
						half diff5 = dsample5 - dosample5;
						half diff6 = dsample6 - dosample6;
						half diff7 = dsample7 - dosample7;
						half diff8 = dsample8 - dosample8;
						
						diff5 *= _DepthDiffSign;
						diff6 *= _DepthDiffSign;
						diff7 *= _DepthDiffSign;
						diff8 *= _DepthDiffSign;

						diff5 = smoothstep(diff5, sign(diff5), 1);
						diff6 = smoothstep(diff6, sign(diff6), 1);
						diff7 = smoothstep(diff7, sign(diff7), 1);
						diff8 = smoothstep(diff8, sign(diff8), 1);

						sample5 = sample5 * diff5;
						sample6 = sample6 * diff6;
						sample7 = sample7 * diff7;
						sample8 = sample8 * diff8;
					#endif

					if (sample1.a > thershold || sample2.a > thershold || sample3.a > thershold || sample4.a > thershold || sample5.a > thershold || sample6.a > thershold || sample7.a > thershold || sample8.a > thershold)
					{
						outline = max(sample1 * sample1.a, sample2 * sample2.a);
						outline = max(outline, sample3 * sample3.a);
						outline = max(outline, sample4 * sample4.a);
						outline = max(outline, sample5 * sample5.a);
						outline = max(outline, sample6 * sample6.a);
						outline = max(outline, sample7 * sample7.a);
						outline = max(outline, sample8 * sample8.a);
						hasOutline = true;
					}

					outline = lerp(outline, outline * _FillAmount, outlineSource.a);
				#else
					if (sample1.a > thershold || sample2.a > thershold || sample3.a > thershold || sample4.a > thershold)
					{
						outline = max(sample1 * sample1.a, sample2 * sample2.a);
						outline = max(outline, sample3 * sample3.a);
						outline = max(outline, sample4 * sample4.a);
						hasOutline = true;
					}
					
					outline = lerp(outline, outline * _FillAmount, outlineSource.a);
				#endif

				if (hasOutline)
				{
					//float4 outlineValue = lerp(outline, 0, _FillAmount);
					float4 outlineValue = lerp(0, outline, clamp((outline.r + outline.g + outline.b + outline.a), 0, 1));
					return lerp(0, outlineValue, _OutlineGlobalIntensity);
				}
				else
					return float4(0,0,0,0);
			}
			
			ENDCG
		}
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _OutlineSource;

			struct v2f 
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert(appdata_img v)
			{
			   	v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				
			   	return o;
			}

			sampler2D _CameraDepthTexture;
			uniform float4 _MainTex_TexelSize;
			
			float _overGlow;
			float _hardness;

			float blendHardness(float value, float hardness)
			{
				return 1 - hardness * (value - 1) * hardness * (value - 1);
			}

			float4 frag (v2f input) : COLOR
			{	
				float2 uv = input.uv;
				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						uv.y = 1 - uv.y;
				#endif

				float4 originalPixel = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(input.uv, _MainTex_ST));
				float4 outlineSource = tex2D(_OutlineSource, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST));
				
				float4 outlineValue = outlineSource * _overGlow * 5;
				float4 addPixel = lerp(0, originalPixel, outlineSource.a);
				return lerp(originalPixel, addPixel + outlineValue, clamp(blendHardness(outlineSource.a, _hardness), 0, 1));
			}
			
			ENDCG
		}

	} 

	FallBack "Diffuse"
}