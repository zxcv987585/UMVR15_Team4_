Shader "Custom/BossDissolve"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _DeathTex("Death Texture", 2D) = "black" {} // 新增的紋理
        _DissolveAmount("Dissolve Amount", Range(0,1)) = 0 // 控制浮現程度
        _Alpha("Alpha", Range(0,1)) = 1.0
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }
            LOD 100
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On

            Pass
            {
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
                    UNITY_FOG_COORDS(1)
                };

                sampler2D _MainTex;
                sampler2D _DeathTex;
                float _DissolveAmount;
                float _Alpha;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    UNITY_TRANSFER_FOG(o, o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 baseTex = tex2D(_MainTex, i.uv);
                    fixed4 deathTex = tex2D(_DeathTex, i.uv);

                    // 透過 DissolveAmount 進行插值，讓死亡紋理慢慢浮現
                    fixed4 finalColor = lerp(baseTex, deathTex, _DissolveAmount);
                    finalColor.a *= _Alpha; // 保持透明度
                    UNITY_APPLY_FOG(i.fogCoord, finalColor);

                    return finalColor;
                }
                ENDCG
            }
        }
}
