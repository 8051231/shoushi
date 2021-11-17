// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/DottedLineShader1"
{
    Properties
    {
        _RepeatCount("Repeat Count", float) = 5
        _DotSize("Dot Size", float) = 0.5
		_Offset("Offset", float) = 0
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _RepeatCount;
            float _DotSize;
			float _Offset;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;              
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv.x = (o.uv.x + _Offset) * _RepeatCount;
                o.color = v.color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.x = fmod(i.uv.x, 1.0f);
                float r = (i.uv.x - 0.5f) * 2.0f;

                fixed4 color = i.color;
                // color.a *= saturate((_DotSize - r) * 255.0f);
				color.a *= step(0, _DotSize - r);

                return color;
            }
            ENDCG
        }
    }
}