Shader "CustomTerrain"
{
    Properties
    {
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 1.0
        _SplatScale ("Splat Scale", Float) = 1.0
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _SplatColor1 ("Splat Color 1", Color) = (1,0,0,1)
        _LightDir ("Light Direction", Vector) = (0,1,0,0)
        [HDR]_Color("Color", Color) = (1,1,1,1)
        _LightPower("LightPower", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            float _NoiseScale;
            float _SplatScale;
            float4 _Color1;
            float4 _SplatColor1;
            float4 _LightDir;
            float4 _Color;
            float _LightPower;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            float2 offset0 = float2(-1,  1);
            float2 offset1 = float2( 0,  1);
            float2 offset2 = float2( 1,  1);
            float2 offset3 = float2(-1,  0);
            float2 offset4 = float2( 0,  0);
            float2 offset5 = float2( 1,  0);
            float2 offset6 = float2(-1, -1);
            float2 offset7 = float2( 0, -1);
            float2 offset8 = float2( 1, -1);

            float blurWeight = 1.0 / 9.0;

            half4 frag (Varyings IN) : SV_Target
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos - IN.worldPos);
                float scaledNoiseUV = _NoiseScale;

                float blurredR_Noise =
                    SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv + offset0 / scaledNoiseUV).r * blurWeight +
                    SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv + offset1 / scaledNoiseUV).r * blurWeight +
                    SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv + offset2 / scaledNoiseUV).r * blurWeight +
                    SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv + offset3 / scaledNoiseUV).r * blurWeight +
                    SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv + offset4 / scaledNoiseUV).r * blurWeight +
                    SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv + offset5 / scaledNoiseUV).r * blurWeight +
                    SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv + offset6 / scaledNoiseUV).r * blurWeight +
                    SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv + offset7 / scaledNoiseUV).r * blurWeight +
                    SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv + offset8 / scaledNoiseUV).r * blurWeight;

                float splatValueNoise = blurredR_Noise;
                float weight1 = 1 - step(0.01, splatValueNoise);
                float4 noiseSplatColor = _Color1 * weight1;
                
                float4 splatSplatColor = _SplatColor1;
                float4 combinedColor = noiseSplatColor * splatSplatColor;

                float3 lightDir = normalize(_LightDir.xyz);
                float lighting = saturate(dot(viewDir, lightDir));

                combinedColor.rgb = lerp(combinedColor.rgb, combinedColor.rgb * lighting * _LightPower, weight1);

                float2 noiseUV2 = IN.uv * (_NoiseScale * 2.0);
                float additionalNoise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV2).g * 0.1;
                combinedColor.rgb += additionalNoise;


                return combinedColor * _Color * _SplatScale;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}