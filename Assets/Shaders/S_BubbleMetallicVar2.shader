Shader "SphereShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0,1)) = 0.5
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Transparency ("Transparency", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD0;
                float4 color : COLOR;
            };

            fixed4 _Color;
            float _Metallic;
            float _Smoothness;
            float _Transparency;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 pos = normalize(i.worldPos);

                float pattern1 = sin(pos.x * 10.0 + _Time.y) * 0.5 + 0.5;
                float pattern2 = cos(pos.y * 10.0 + _Time.y) * 0.5 + 0.5;
                float pattern3 = sin(pos.z * 10.0 + _Time.y) * 0.5 + 0.5;

                float combinedPattern = (pattern1 + pattern2 + pattern3) / 3.0;
                fixed3 finalColor = _Color.rgb * combinedPattern * (_Metallic + 0.1);
                float alpha = _Transparency * combinedPattern * i.color;

                return fixed4(finalColor, alpha);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
