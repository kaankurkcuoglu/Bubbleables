Shader "BubbleMetallic"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,0.5)
        _RainbowSpeed ("Rainbow Speed", Float) = 1.0
        _AnimationStrength ("Animation Strength", Float) = 0.1
        _RefractionStrength ("Refraction Strength", Float) = 0.02
        _MainTex ("Texture", 2D) = "white" {}
        _LerpColorFrom ("Lerp Color From", Color) = (0,0,0,0)
        _LerpColorTo ("Lerp Color To", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _REQUIRE_DYNAMIC_INPUTS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            CBUFFER_START(Instancing)
             float4 _BaseColor;
             float _RainbowSpeed;
             float _AnimationStrength;
             float _RefractionStrength;
             float4 _MainTex_ST;
            float4 _LerpColorFrom;
            float4 _LerpColorTo;
            CBUFFER_END
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float noise(float3 p)
            {
                return frac(sin(dot(p, float3(12.9898,78.233, 45.164))) * 43758.5453);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
            
                float3 animatedPosition = IN.positionOS;
                float time = _Time.y * _RainbowSpeed;
                float n = noise(animatedPosition + time);
                animatedPosition += IN.normalOS * n * _AnimationStrength * 0.01;

                OUT.positionHCS = TransformObjectToHClip(animatedPosition);
                OUT.worldPos = TransformObjectToWorld(animatedPosition);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 baseColor = _BaseColor;
                
                float rainbow = sin(_Time.z *  _RainbowSpeed);
                float3 rainbowColor = lerp(_LerpColorFrom.rgb, _LerpColorTo.rgb, rainbow);
           
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                float3 viewDir = normalize(_WorldSpaceCameraPos - IN.worldPos);
                float3 refracted = refract(viewDir, IN.worldNormal, _RefractionStrength);
                
                half4 finalColor = tex * half4((rainbowColor + refracted) * _BaseColor, baseColor.a);

                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "Transparent/Diffuse"
}
