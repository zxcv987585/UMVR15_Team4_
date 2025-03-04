// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/UnlitDissolveGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)

        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0.0

        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)

        _RimColor ("Rim Color", Color) = (0, 0, 1, 1)
        _RimPower ("Rim Power", Range(0.1, 8)) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            sampler2D _DissolveTex;
            float4 _DissolveTex_ST;
            float _DissolveAmount;

            float4 _EmissionColor;

            float4 _RimColor;
            float _RimPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                float dissolveValue = tex2D(_DissolveTex, i.uv).r;
                
                float edgeWidth = 0.1; // 可調整柔邊寬度
                float dissolveEdge = smoothstep(_DissolveAmount - edgeWidth, _DissolveAmount, dissolveValue);
                
                if (dissolveEdge <= 0) discard;

                // 邊緣光
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float rim = pow(1.0 - saturate(dot(i.worldNormal, viewDir)), _RimPower);
                col.rgb += _RimColor.rgb * rim;

                // 發光 + 消融邊緣強化
                col.rgb += _EmissionColor.rgb * dissolveEdge;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Texture"
}
