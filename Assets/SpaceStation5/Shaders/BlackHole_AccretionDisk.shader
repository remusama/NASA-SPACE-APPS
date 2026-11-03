Shader "SpaceStation5/BlackHole_AccretionDisk"
{
    Properties
    {
        _MainTex ("Accretion Plasma Texture", 2D) = "white" {}
        [HDR] _CoreColor ("Core Incandescent Color", Color) = (5.0, 4.5, 3.5, 1.0)
        [HDR] _InnerColor ("Inner Plasma Color", Color) = (6.0, 2.8, 0.6, 1.0)
        [HDR] _OuterColor ("Outer Dust Color", Color) = (2.5, 0.45, 0.08, 1.0)
        _SwirlSpeed ("Vortex Swirl Speed", Float) = 0.5
        _DopplerIntensity ("Relativistic Doppler Beaming", Range(0, 1.5)) = 0.85
        _EmissionBoost ("HDR Emission Multiplier", Float) = 4.5
        _TurbulenceFreq ("Turbulence Frequency", Float) = 10.0
        _TurbulenceSpeed ("Turbulence Speed", Float) = 0.6
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent+200" 
            "RenderPipeline" = "UniversalPipeline" 
            "IgnoreProjector" = "True"
        }

        Blend One One // Additive for blazing incandescent plasma brilliance!
        ZWrite Off
        Cull Off

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
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionOS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _CoreColor;
                float4 _InnerColor;
                float4 _OuterColor;
                float _SwirlSpeed;
                float _DopplerIntensity;
                float _EmissionBoost;
                float _TurbulenceFreq;
                float _TurbulenceSpeed;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.positionOS = input.positionOS.xyz;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            // Pseudo-random noise functions
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            float4 frag(Varyings input) : SV_Target
            {
                float time = _Time.y;

                // 1. True 3D Object Space Polar Coordinates:
                // Event Horizon radius is ~4.9 in the Blender model.
                // Outermost accretion disk radius is ~12.2.
                float rObj = length(input.positionOS.xz);
                float angleObj = atan2(input.positionOS.z, input.positionOS.x);

                // Smooth normalized radius from inner event horizon to outer rim [0.0, 1.0]
                float rNorm = saturate((rObj - 4.4) / 7.8);
                float angleNorm = frac(angleObj / 6.2831853 + 0.5);

                // 2. Differential Keplerian Swirl Velocity:
                // Plasma orbits significantly faster closer to the singularity (v ~ 1 / sqrt(r))
                float speed = _SwirlSpeed / max(0.3, sqrt(max(0.05, rNorm)));
                float currentAngle = frac(angleNorm + time * speed * 0.12);

                // 3. Multi-Octave Turbulent Swirling Plasma Veins
                float2 noiseUV = float2(currentAngle * _TurbulenceFreq, rNorm * 3.5 + time * _TurbulenceSpeed);
                float n = noise(noiseUV) * 0.55 + noise(noiseUV * 2.5) * 0.3 + noise(noiseUV * 5.0) * 0.15;

                // 4. Sample Vortex Texture using dynamic polar and Cartesian coords
                float2 polarUV = float2(currentAngle, rNorm);
                float4 polarTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, polarUV);

                float2 cartUV = float2(0.5 + cos(currentAngle * 6.2831853) * (0.1 + rNorm * 0.4),
                                      0.5 + sin(currentAngle * 6.2831853) * (0.1 + rNorm * 0.4));
                float4 cartTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, cartUV);

                float texLum = max(0.45, (polarTex.r + cartTex.r + polarTex.g + cartTex.g) * 0.5);

                // 5. Relativistic Doppler Beaming (Einstein General Relativity)
                // Approaching plasma is blueshifted and amplified in intensity
                float doppler = sin(currentAngle * 6.2831853);
                float dopplerMult = max(0.3, 1.0 + doppler * _DopplerIntensity);
                float3 dopplerColor = (doppler > 0.0) ? float3(1.25, 1.15, 1.05) : float3(1.0, 0.75, 0.55);

                // 6. High-Energy Temperature Radial Gradient
                // Inner edge = Blinding white-hot core
                // Mid zone = Fiery incandescent gold/amber
                // Outer zone = Deep crimson plasma filaments
                float3 plasmaColor = lerp(_CoreColor.rgb, _InnerColor.rgb, smoothstep(0.0, 0.35, rNorm));
                plasmaColor = lerp(plasmaColor, _OuterColor.rgb, smoothstep(0.35, 0.95, rNorm));

                // 7. Accretion Shock Flare Modulation
                float flare = 0.75 + 0.5 * n;

                // Soft radial edge fade at inner ISCO boundary and outer disc edge
                float edgeFade = smoothstep(0.0, 0.12, rNorm) * smoothstep(1.0, 0.82, rNorm);

                float3 finalColor = plasmaColor * texLum * dopplerMult * dopplerColor * flare * edgeFade * _EmissionBoost;

                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}
