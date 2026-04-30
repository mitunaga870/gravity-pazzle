Shader "Unlit/ReversedMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _Color ("Color", Color) = (1, 1, 1, 1)
        
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        // 透明度情報保持
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                float4 color : COLOR; // 頂点カラーを追加
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;

                float4 color : COLOR; // 頂点カラーをフラグメントシェーダーに渡す
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;

            float4 _Center;
            float _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color; // 頂点カラーとプロパティの色を掛け合わせる
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * i.color; // テクスチャカラーに頂点カラーを掛け合わせる

                float aspect = _ScreenParams.x / _ScreenParams.y; // 画面のアスペクト比

                // 反転マスク処理 - マスク内は透明、マスク外は通常のテクスチャカラー
                float2 cur = i.uv;
                cur.x *= aspect; // アスペクト比を考慮してX座標を調整
                float2 center = _Center.xy;
                center.x *= aspect; // アスペクト比を考慮してX座標を調整
                float dist = distance(cur, center);
                if (dist < _Radius)
                {
                    discard; // マスク内は描画しない（透明）
                }
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
