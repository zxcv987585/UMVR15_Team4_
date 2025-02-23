Shader "Custom/DissolveStandard"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _MetallicGlossMap ("Metallic", 2D) = "black" {}
        _DissolveTex ("Dissolve Texture", 2D) = "white" {} // 新增消融貼圖
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0.0 // 控制消融程度
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _MetallicGlossMap;
        sampler2D _DissolveTex;
        float _DissolveAmount;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            o.Metallic = tex2D(_MetallicGlossMap, IN.uv_MainTex).r;

            // 加入 Dissolve 效果
            float dissolve = tex2D(_DissolveTex, IN.uv_MainTex).r;
            clip(dissolve - _DissolveAmount);
        }
        ENDCG
    }
}
