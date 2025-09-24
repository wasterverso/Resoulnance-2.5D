Shader "Custom/Sprite_2in1"
{
    Properties
    {
        [MainTexture] _BaseMap ("Sprite Texture", 2D) = "white" {}
        _BaseColor ("Tint", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        // Outline extra
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size", Range(0,10)) = 1
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "RenderPipeline"="UniversalPipeline"
            "CanUseSpriteAtlas"="True"
        }
        LOD 200

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        struct Attributes
        {
            float4 positionOS   : POSITION;
            float2 uv           : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionHCS  : SV_POSITION;
            float2 uv           : TEXCOORD0;
            float3 positionWS   : TEXCOORD1;
            float3 normalWS     : TEXCOORD2;
        };

        TEXTURE2D(_BaseMap);
        SAMPLER(sampler_BaseMap);

        float4 _BaseMap_ST;
        float4 _BaseColor;
        float _Cutoff;

        // Outline
        float4 _OutlineColor;
        float _OutlineSize;

        Varyings Vert(Attributes IN)
        {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
            OUT.normalWS = float3(0,0,-1); // normal fixa para sprites
            OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
            return OUT;
        }

        half4 Frag(Varyings IN) : SV_Target
        {
            float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;

            // --- OUTLINE ---
            float2 pixelSize = _OutlineSize / _ScreenParams.xy;

            if (tex.a <= _Cutoff)
            {
                float alpha = 0;
                alpha += SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv + float2(pixelSize.x, 0)).a;
                alpha += SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv - float2(pixelSize.x, 0)).a;
                alpha += SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv + float2(0, pixelSize.y)).a;
                alpha += SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv - float2(0, pixelSize.y)).a;

                if (alpha > 0) return _OutlineColor;
                clip(tex.a - _Cutoff); // descarta fora do contorno
            }
            // ----------------

            return tex;
        }
        ENDHLSL

        // Passo principal (respeita transparência e Order in Layer)
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            ENDHLSL
        }

        // Passo extra para projetar sombra
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            ZWrite On
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            struct ShadowAttributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct ShadowVaryings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            ShadowVaryings ShadowVert(ShadowAttributes IN)
            {
                ShadowVaryings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(positionWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            float4 ShadowFrag(ShadowVaryings IN) : SV_Target
            {
                float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                clip(tex.a - _Cutoff);
                return 0;
            }

            #pragma vertex ShadowVert
            #pragma fragment ShadowFrag
            ENDHLSL
        }
    }
}
