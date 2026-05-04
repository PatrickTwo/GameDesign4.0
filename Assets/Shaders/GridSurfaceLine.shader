Shader "GameDesign4/Grid/GridSurfaceLine"
{
    Properties
    {
        _GridLineColor ("Grid Line Color", Color) = (0.0, 0.82, 1.0, 0.65)
        _GridFillColor ("Grid Fill Color", Color) = (0.0, 0.0, 0.0, 0.0)
        _GridWidth ("Grid Width", Float) = 100.0
        _GridHeight ("Grid Height", Float) = 100.0
        _GridLineThickness ("Grid Line Thickness", Range(0.001, 0.2)) = 0.02
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "ForwardUnlit"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _GridLineColor;
                float4 _GridFillColor;
                float _GridWidth;
                float _GridHeight;
                float _GridLineThickness;
            CBUFFER_END

            /// <summary>
            /// 顶点阶段：仅负责传递 UV 与齐次裁剪空间坐标。
            /// </summary>
            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            /// <summary>
            /// 片元阶段：在整张 Quad 上按 UV 直接绘制规则网格线。
            /// </summary>
            half4 Frag(Varyings input) : SV_Target
            {
                float2 gridUv = input.uv * float2(max(_GridWidth, 1.0), max(_GridHeight, 1.0));
                float2 cellEdgeDistance = min(frac(gridUv), 1.0 - frac(gridUv));
                float2 edgeAntiAlias = fwidth(gridUv);

                // 靠近单元边界的位置绘制线条，其余区域使用底色。
                float lineX = 1.0 - smoothstep(_GridLineThickness, _GridLineThickness + edgeAntiAlias.x, cellEdgeDistance.x);
                float lineY = 1.0 - smoothstep(_GridLineThickness, _GridLineThickness + edgeAntiAlias.y, cellEdgeDistance.y);
                float lineMask = saturate(max(lineX, lineY));

                half4 finalColor = lerp(_GridFillColor, _GridLineColor, lineMask);
                return finalColor;
            }
            ENDHLSL
        }
    }
}
