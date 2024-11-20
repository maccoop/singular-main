Shader "Custom/CrossSectionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SectionPlanePos ("Section Plane Position", Vector) = (0, 0, 0, 0)
        _SectionPlaneNormal ("Section Plane Normal", Vector) = (0, 1, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _SectionPlanePos;
            float4 _SectionPlaneNormal;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate distance from the point to the plane
                float dist = dot(i.worldPos - _SectionPlanePos.xyz, _SectionPlaneNormal.xyz);

                // Discard the pixel if it's on the wrong side of the plane
                if (dist > 0)
                    discard;

                // Sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
