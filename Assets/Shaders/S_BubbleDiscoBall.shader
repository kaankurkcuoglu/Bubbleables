Shader "BubbleDiscoBall"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _FresnelColor("Fresnel Color", Color) = (0.4, 0.7, 1, 1)
        _FresnelPower("Fresnel Power", Range(0.1, 10)) = 2.0

        _ReflectionTex("Reflection Texture", 2D) = "white" {}
        _ReflectionStrength("Reflection Strength", Range(0,1)) = 0.5

        _HeightMap("Height Map (R)", 2D) = "white" {}
        _HeightScale("Height Scale", Range(0,1)) = 0.1

        _DistortionSpeed("Distortion Speed", Range(0,10)) = 1.0
        _RotationSpeed("UV Rotation Speed", Range(0,10)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #define UNITY_PI 3.14159265359

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalOS   : NORMAL;    
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 worldPos    : TEXCOORD1;
                float3 normalWS    : TEXCOORD2;
            };

            TEXTURE2D(_HeightMap);
            SAMPLER(sampler_HeightMap);

            TEXTURE2D(_ReflectionTex);
            SAMPLER(sampler_ReflectionTex);

            float4 _BaseColor;
            float4 _FresnelColor;
            float  _FresnelPower;

            float  _ReflectionStrength;

            float  _HeightScale;
            float  _DistortionSpeed;
            float  _RotationSpeed;

            float2 SphericalUV(float3 dir)
            {
                // dir normalize olmalı
                float2 uv;
                float longitude = atan2(dir.z, dir.x);  
                float latitude  = asin(dir.y);           

                uv.x = 0.5 + (longitude / (2.0 * UNITY_PI));
                uv.y = 0.5 - (latitude  / UNITY_PI);

                return uv;
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float rotation   = _Time.y * _RotationSpeed;
                float2 rotatedUV = float2(
                    cos(rotation)*IN.uv.x - sin(rotation)*IN.uv.y,
                    sin(rotation)*IN.uv.x + cos(rotation)*IN.uv.y
                );

                float heightValue = SAMPLE_TEXTURE2D_LOD(_HeightMap, sampler_HeightMap, rotatedUV, 0).r;
                heightValue *= _HeightScale * 0.01;

                float3 normalOS = normalize(IN.normalOS);

                float3 posOS = IN.positionOS.xyz;

                posOS += normalOS * heightValue;

                OUT.positionHCS = TransformObjectToHClip(posOS);

                OUT.uv = rotatedUV;

                OUT.worldPos = TransformObjectToWorld(posOS);
                OUT.normalWS = normalize(TransformObjectToWorldNormal(normalOS));

                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.worldPos);

                float NdotV = dot(IN.normalWS, viewDir);
                float fresnelTerm = pow(1.0 - saturate(NdotV), _FresnelPower);
                float3 fresnelColor = _FresnelColor.rgb * fresnelTerm;

                float3 reflectDir = reflect(-viewDir, IN.normalWS);

                float2 reflectionUV = SphericalUV(normalize(reflectDir));

                float3 reflectionSample = SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, reflectionUV).rgb;
                float3 reflectionColor  = reflectionSample * _ReflectionStrength;

                float3 baseCol = _BaseColor.rgb;

                float3 finalColor = baseCol + fresnelColor + reflectionColor;
                float alpha = _BaseColor.a;

                return float4(finalColor, alpha);
            }

            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/Fallback"
}
