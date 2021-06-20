Shader "Nanolabo/Two-Sided Cutout"
{
    Properties
    {
        _MainTex ("Albedo, Metallic", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _BumpMap ("Normal", 2D) = "bump"{}
		_BumpScale("Scale", Range(0, 2)) = 1
        _Cutoff ("Cutoff", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        Cull Off
        LOD 200
        
        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows alphatest:_Cutoff
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        half _BumpScale;
        half4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * _Color.rgb;
            o.Alpha = c.a;
            o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
        }

        ENDCG
    } 
    FallBack "Diffuse"
}