Shader "Custom/BossDissolve"
{
    Properties
    {
        _Brightness("Brightness", Range(0.5, 2.0)) = 1.2
        _MainTex("Base Texture", 2D) = "white" {}
        _DeathTex("Death Texture", 2D) = "black" {}
        _DissolveAmount("Dissolve Amount", Range(0,1)) = 0
        _Alpha("Alpha", Range(0,1)) = 1.0
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
            LOD 100
            ZWrite On

            CGINCLUDE
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1) // [新增] FOG 變數
            };

            sampler2D _MainTex;
            sampler2D _DeathTex;
            float _DissolveAmount;
            float _Alpha;
            float _Brightness;

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                UNITY_TRANSFER_FOG(o, o.pos); // [新增] 傳遞 FOG 數據
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 deathCol = tex2D(_DeathTex, i.uv);

                // 如果 DissolveAmount 接近 1，顯示 DeathTex
                col = lerp(col, deathCol, _DissolveAmount);
                col.rgb *= _Brightness;
                // [新增] 套用 Unity 內建霧效果
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG

            Pass {
                Tags { "LightMode" = "ForwardBase" }
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog // [確保 FOG 可用]
                ENDCG
            }

            // Shadow Caster 讓 Shader 可以投影影子
            Pass {
                Tags { "LightMode" = "ShadowCaster" }
                CGPROGRAM
                #pragma vertex vertShadow
                #pragma fragment fragShadow
                #include "UnityCG.cginc"

                struct v2fShadow {
                    V2F_SHADOW_CASTER;
                };

                v2fShadow vertShadow(appdata_base v) {
                    v2fShadow o;
                    TRANSFER_SHADOW_CASTER(o)
                    return o;
                }

                float4 fragShadow(v2fShadow i) : SV_Target {
                    return 0;
                }
                ENDCG
            }
        }
}