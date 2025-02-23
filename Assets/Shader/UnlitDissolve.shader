Shader "Custom/UnlitDissolve"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0.0
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _EdgeWidth ("Edge Width", Range(0,0.2)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha // 透明度控制

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _DissolveTex;
            float _DissolveAmount;
            float4 _EdgeColor;
            float _EdgeWidth;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dissolve = tex2D(_DissolveTex, i.uv).r;
                float edge = smoothstep(_DissolveAmount, _DissolveAmount + _EdgeWidth, dissolve);
                
                if (dissolve < _DissolveAmount)
                    discard; // 直接丟棄像素，產生消融效果

                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = lerp(_EdgeColor.rgb, col.rgb, edge); // 邊緣發光效果
                return col;
            }
            ENDCG
        }
    }
}