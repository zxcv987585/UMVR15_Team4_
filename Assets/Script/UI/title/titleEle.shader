Shader "Unlit/titleEle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanSpeed ("Scan Speed", Range(0.1, 10)) = 2.0
        _ScanWidth ("Scan Width", Range(0.01, 0.5)) = 0.1
        _ScanIntensity ("Scan Intensity", Range(0, 2)) = 1.5
        _ScanInterval ("Scan Interval", Range(0.5, 10)) = 3.0
        _Alpha ("Alpha", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _ScanSpeed;
            float _ScanWidth;
            float _ScanIntensity;
            float _ScanInterval;
            float _Alpha;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float _TimeValue;
            float getScanEffect(float2 uv)
            {
                float timeMod = fmod(_Time.y, _ScanInterval) / _ScanInterval;
                float scanPos = (timeMod * _ScanSpeed) - 0.2;
                float dist = abs(uv.x - scanPos);
                float scanEffect = smoothstep(_ScanWidth, 0, dist) * _ScanIntensity;
                return scanEffect;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float scan = getScanEffect(i.uv);
                col.rgb += scan;
                col.a *= _Alpha;
                return col;
            }
            ENDCG
        }
    }
}
