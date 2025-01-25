Shader "WaterStylizedShader"
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
                float linearDepth : TEXCOORD2;
            };
            
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                OUT.viewDir = viewDir;
                float depth = length(_WorldSpaceCameraPos - worldPos) * _DepthCoefficient;
                OUT.linearDepth = depth;
                
                return OUT;
            }
            
            half4 frag (Varyings IN) : SV_Target
            {
                float time = _Time.y;
                float vibration = sin(time * _VibrationFrequency) * _VibrationStrength;
                float2 vibratedUV = IN.uv + float2(vibration, 0.0); 
                
                float noise = tex2D(_NoiseTex, vibratedUV * _WaveScale).r;
                float3 reflectionNormal = normalize(_ReflectionDirection.xyz + float3(noise * _WaveScale, noise * _WaveScale, noise * _WaveScale));
                
                float3 reflection = reflect(IN.viewDir, reflectionNormal) * _ReflectionStrength;
                
                float wave = dot(reflection, normalize(_WaveDirection.xyz));
                
                float4 color = _BaseColor;
                color.rgb += wave;
                color.rgb *= saturate(IN.linearDepth);
                
                float dissolveFactor = noise;
                float edge = smoothstep(_DissolveThreshold - _DissolveEdgeWidth, _DissolveThreshold + _DissolveEdgeWidth, dissolveFactor);
                color.a *= edge;
                
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}