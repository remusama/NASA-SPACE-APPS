Shader "SpaceStation5/EarthAtmosphere"
{
    Properties
    {
        [HDR] _AtmosphereColorDay ("Day Rayleigh Scatter Color", Color) = (0.22, 0.65, 1.0, 1.0)
        [HDR] _AtmosphereColorSunset ("Sunset / Terminator Color", Color) = (1.0, 0.45, 0.15, 1.0)
        [HDR] _AtmosphereColorNight ("Night Rim Ambient Color", Color) = (0.04, 0.08, 0.22, 1.0)
        _RimPower ("Atmospheric Rim Exponent", Range(0.5, 10.0)) = 4.2
        _RimIntensity ("Rim Glow Multiplier", Float) = 2.8
        _SunDirection ("Sun Illumination Vector", Vector) = (0.7, 0.6, -0.3, 0.0)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent+50" 
            "RenderPipeline" = "UniversalPipeline" 
        }

        Blend SrcAlpha One
        ZWrite Off
        Cull Front // Render backfaces for atmospheric halo depth, or Cull Back

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _AtmosphereColorDay;
                float4 _AtmosphereColorSunset;
                float4 _AtmosphereColorNight;
                float _RimPower;
                float _RimIntensity;
                float4 _SunDirection;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(input.normalWS);
                float3 viewDir = normalize(GetCameraPositionWS() - input.positionWS);
                float3 sunDir = normalize(_SunDirection.xyz);

                // Fresnel limb factor (1.0 at outer limb)
                float NdotV = saturate(dot(normal, viewDir));
                float limb = 1.0 - NdotV;
                float rimGlow = pow(limb, _RimPower);

                // Solar illumination alignment on the atmosphere
                float NdotL = dot(normal, sunDir);
                float dayFactor = saturate(NdotL * 1.5 + 0.1);
                float terminatorFactor = saturate(1.0 - abs(NdotL) * 3.0);

                // Blend between day electric cyan, sunset amber/rose, and night indigo
                float3 atmosColor = lerp(_AtmosphereColorNight.rgb, _AtmosphereColorDay.rgb, dayFactor);
                atmosColor = lerp(atmosColor, _AtmosphereColorSunset.rgb, terminatorFactor * 0.7);

                float3 finalRGB = atmosColor * rimGlow * _RimIntensity;
                float alpha = rimGlow * (0.3 + 0.7 * dayFactor);

                return float4(finalRGB, alpha);
            }
            ENDHLSL
        }
    }
}
