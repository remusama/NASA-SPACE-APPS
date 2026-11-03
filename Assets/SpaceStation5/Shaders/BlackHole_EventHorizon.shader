Shader "SpaceStation5/BlackHole_EventHorizon"
{
    Properties
    {
        _HorizonColor ("Event Horizon Void Color", Color) = (0.0, 0.0, 0.0, 1.0)
        [HDR] _PhotonRingColor ("Photon Sphere Ring Color", Color) = (8.0, 6.5, 3.5, 1.0)
        _RingSharpness ("Photon Ring Sharpness", Range(1, 60)) = 28.0
        _RingWidth ("Photon Ring Position", Range(0.7, 0.99)) = 0.88
        _LensingGlow ("Gravitational Lensing Glow", Range(0, 10)) = 3.5
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "Queue" = "Geometry+50" 
            "RenderPipeline" = "UniversalPipeline" 
        }

        ZWrite On
        Cull Back

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
                float4 _HorizonColor;
                float4 _PhotonRingColor;
                float _RingSharpness;
                float _RingWidth;
                float _LensingGlow;
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

                // Silhouette limb factor: 1.0 at outer grazing edge, 0.0 at center
                float NdotV = saturate(dot(normal, viewDir));
                float limb = 1.0 - NdotV;

                // 1. Einstein Photon Ring: Intense razor-thin ring at r = 1.5 Rs
                float ringIntensity = pow(saturate((limb - _RingWidth) / (1.0 - _RingWidth)), _RingSharpness);

                // 2. Gravitational Lensing Halo around the event horizon boundary
                float lensingHalo = pow(limb, 6.0) * _LensingGlow;

                // 3. Central Schwarzschild Event Horizon: Absolute void
                float3 finalColor = _HorizonColor.rgb;
                
                // Add blazing photon sphere ring
                finalColor += _PhotonRingColor.rgb * (ringIntensity * 6.0 + lensingHalo * 0.4);

                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}
