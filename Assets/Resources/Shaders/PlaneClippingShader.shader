Shader "Custom/PlaneClippingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PlanePosition ("Plane Position", Vector) = (0, 0, 0, 0)
        _PlaneNormal ("Plane Normal", Vector) = (0, 1, 0, 0)
        _CrossSectionColor ("Cross-Section Color", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard

        sampler2D _MainTex;
        float4 _PlanePosition;
        float4 _PlaneNormal;
        float4 _CrossSectionColor;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Calculate the distance from the plane
            float d = dot(IN.worldPos - _PlanePosition.xyz, _PlaneNormal.xyz);

            if (d > 0)
            {
                // The fragment is on the positive side of the plane; discard it
                clip(-1);
            }
            else if (abs(d) < 0.01)
            {
                // The fragment is on the plane; render the cross-section color
                o.Albedo = _CrossSectionColor.rgb;
                o.Alpha = _CrossSectionColor.a;
            }
            else
            {
                // Render the normal texture
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
