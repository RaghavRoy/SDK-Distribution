Shader "Jio/SurfaceShader"
{
    Properties
    {
        [NoScaleOffset] _Background ("Background Cubemap", Cube) = "grey" {}
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        ZWrite Off
        ColorMask RGBA

        Pass
        {
            BlendOp Add, Max
            Blend SrcAlpha OneMinusSrcAlpha, One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
     
            };

            samplerCUBE _Background;
            fixed4 _Color;
            fixed4 _StaticColor;
            sampler2D _MainTex;
            float4 _ClipRect;
            float4 _MainTex_ST;
            uniform float _staticValue;

            v2f vert(appdata_t v)
            {
                v2f OUT;
  
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = lerp(texCUBE(_Background, -WorldSpaceViewDir(IN.worldPosition)), _StaticColor, _staticValue);
                half4 sprite = tex2D(_MainTex, IN.texcoord);
                color.a *= sprite.a * IN.color.a;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}
