Shader "RainbowBubble"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _Alpha("Alpha", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #define UNITY_PI 3.14159265359

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 viewDir : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _Alpha;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                float3 worldPos = TransformObjectToWorld(IN.positionOS).xyz;
                OUT.worldPos = worldPos;
                float3 viewDirWorld = normalize(_WorldSpaceCameraPos - worldPos);
                OUT.viewDir = viewDirWorld;
                OUT.normal = normalize(TransformObjectToWorldNormal(IN.normalOS));
                return OUT;
            }

            float3 HueToRGB(float hue)
            {
                float3 rgb = abs(frac(float3(hue, hue + 1.0/3.0, hue + 2.0/3.0)) * 6.0 - 3.0);
                rgb = clamp(rgb, 0.0, 1.0);
                return rgb;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 viewDir = normalize(IN.viewDir);
                float3 refracted = reflect(viewDir, IN.normal);
                float3 refDir = refracted;
                float theta = atan2(refDir.z, refDir.x) / (2.0 * UNITY_PI) + 0.5;
                float phi = acos(refDir.y) / UNITY_PI;
                float time = _Time.y * 0.1;
                float hue = frac(theta + phi + time);
                float3 color = HueToRGB(hue);
                color *= _BaseColor.rgb;
                float alpha = _Alpha;
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }

    FallBack "Unlit/Transparent"
}
