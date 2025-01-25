Shader "Custom/WaterStylizedShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _ReflectionStrength ("Reflection Strength", Range(0,1)) = 0.5
        _WaveDirection ("Wave Direction", Vector) = (1,0,0,0)
        _WaveScale ("Wave Scale", Float) = 0.1
        _ReflectionDirection ("Reflection Direction", Vector) = (0,1,0,0)
        _DepthCoefficient ("Depth Coefficient", Float) = 1.0
        _VibrationStrength ("Vibration Strength", Float) = 0.05
        _VibrationFrequency ("Vibration Frequency", Float) = 10.0
        _DissolveThreshold ("Dissolve Threshold", Range(0,1)) = 0.5
        _DissolveEdgeWidth ("Dissolve Edge Width", Range(0,0.5)) = 0.1
        _FogDepth ("Fog Depth", Float) = 20.0
        _FogColor1 ("Fog Color1", Color) = (0.5921569, 0.6784314, 0.6, 1)
        _FogColor2 ("Fog Color2", Color) = (0.3254902, 0.3490196, 0.3098039, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _NoiseTex;
            float4 _BaseColor;
            float _ReflectionStrength;
            float4 _WaveDirection;
            float _WaveScale;
            float4 _ReflectionDirection;
            float _DepthCoefficient;
            float _VibrationStrength;
            float _VibrationFrequency;
            float _DissolveThreshold;
            float _DissolveEdgeWidth;
            float _FogDepth;
            float4 _FogColor1;
            float4 _FogColor2;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float viewSpaceZ : TEXCOORD2;
                float time : TEXCOORD3;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);

                float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                OUT.viewDir = viewDir;

                float3 viewPos = TransformWorldToView(worldPos);
                OUT.viewSpaceZ = -viewPos.z;

                OUT.time = _Time.y;

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float time = IN.time;

                float vibration = sin(time * _VibrationFrequency) * _VibrationStrength;
                float2 vibratedUV = IN.uv + float2(vibration, 0.0);

                float noise = tex2D(_NoiseTex, vibratedUV * _WaveScale).r;

                float3 reflectionNormal = normalize(_ReflectionDirection.xyz + float3(noise * _WaveScale, noise * _WaveScale, noise * _WaveScale));
                float3 reflection = reflect(IN.viewDir, reflectionNormal) * _ReflectionStrength;

                float wave = dot(reflection, normalize(_WaveDirection.xyz));
                float4 color = _BaseColor;
                color.rgb += wave;

                float linearDepth = IN.viewSpaceZ * _DepthCoefficient;
                color.rgb *= saturate(linearDepth / _FogDepth);
                float dissolveFactor = noise;
                float edge = smoothstep(_DissolveThreshold - _DissolveEdgeWidth, _DissolveThreshold + _DissolveEdgeWidth, dissolveFactor);
                color.a *= edge;

                float fogFactor = saturate(linearDepth / _FogDepth);
                float4 fogColor = lerp(_FogColor1, _FogColor2, fogFactor);
                color.rgb = lerp(color.rgb, fogColor.rgb, fogFactor);
                
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
