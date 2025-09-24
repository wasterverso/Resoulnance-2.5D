Shader "Custom/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size", Range(0,10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offset = _OutlineSize / _ScreenParams.xy;

                fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
                if (tex.a == 0)
                {
                    // verificar arredores (offset nos 4 lados)
                    float alpha = 0;
                    alpha += tex2D(_MainTex, i.uv + float2(offset.x, 0)).a;
                    alpha += tex2D(_MainTex, i.uv - float2(offset.x, 0)).a;
                    alpha += tex2D(_MainTex, i.uv + float2(0, offset.y)).a;
                    alpha += tex2D(_MainTex, i.uv - float2(0, offset.y)).a;

                    if (alpha > 0) return _OutlineColor;
                }
                return tex;
            }
            ENDCG
        }
    }
}
