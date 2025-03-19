Shader "Custom/DissolveGlow"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _MetallicGlossMap ("Metallic", 2D) = "black" {}
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0.0
        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
        _RimColor ("Rim Color", Color) = (0, 0, 1, 1) // 預設藍色邊緣光
        _RimPower ("Rim Power", Range(0.1, 8)) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows addshadow

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _MetallicGlossMap;
        sampler2D _DissolveTex;
        float _DissolveAmount;
        fixed4 _EmissionColor;
        fixed4 _RimColor;
        float _RimPower;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float dissolve = tex2D(_DissolveTex, IN.uv_MainTex).r;
            clip(dissolve - _DissolveAmount);

            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            o.Metallic = tex2D(_MetallicGlossMap, IN.uv_MainTex).r;

            // **發光效果**
            o.Emission = _EmissionColor.rgb;

            // **邊緣光 (Rim Light)**
            float rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            rim = pow(rim, _RimPower);
            o.Emission += _RimColor.rgb * rim;
        }
        ENDCG
    }
}