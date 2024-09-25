Shader "Custom/WaterShader" 
{
    Properties 
    {
        _TessellationFactor("Tessellation", Range(0.0, 1.0)) = 1.0
        _TessellationBias("Tessellation Bias", Float) = 0.0
        _CullingTolerance("Culling Tolerance", Float) = 0.1
        _Smoothing("Smoothing", Float) = 1.0
        _Steepness("Steepness", Float) = 0.5
        _OverallHeightAdjust("Overall Height Adjust", Float) = 0.5
        _WaveA ("Wave A (dir, steepness, wavelength)", Vector) = (1,0,0.5,10)
		_WaveB ("Wave B (dir, steepness, wavelength)", Vector) = (0,1,0.25,20)
		_WaveC ("Wave C (dir, steepness, wavelength)", Vector) = (1,1,0.15,10)
        _WaterColor("Water Color", Color) = (0, 0, 1, 1)
        _SpecularIntensity("Specular Intensity", Float) = 1.0
        _NormalIntensity("Normal Intensity", Float) = 1.0
        _WaterFogColor ("Water Fog Color", Color) = (0, 0, 0, 0)
		_WaterFogDensity ("Water Fog Density", Range(0, 2)) = 0.1
        _RefractionIntensity("Refraction Intensity", Range(0, 1)) = 0.25
        _EdgeFoamAmount("Edge Foam Amount", Float) = 5.0
        _TopBottomGradientColor("Top -> Bottom Gradient Color", Color) = (1, 1, 1, 1)
        _PerlinNoise("Perlin Noise", 2D) = "white" {}
        _PerlinNoiseTiling("Perlin Noise Tiling", Float) = 5.0
        _FoamColor("Foam Color", Color) = (1, 1, 1, 1)
        [Normal] _DetailNormal("Detail Normal", 2D) = "" {}
        _DetailNormalIntensity("Detail Normal Intensity", Float) = 1
    }
    SubShader 
    {
        Tags 
        { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalRenderPipeline"
        }
        
        Pass
        {
            ZWrite Off
            
            HLSLPROGRAM

            #pragma target 5.0

            #pragma vertex Vertex
            #pragma hull Hull
            #pragma domain Domain
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _TessellationFactor;
                float _TessellationBias;
                float _CullingTolerance;
                float _Smoothing;
                float _Steepness;
                float _OverallHeightAdjust;
                float4 _WaveA;
                float4 _WaveB;
                float4 _WaveC;
                float4 _WaterColor;
                float _SpecularIntensity;
                float _NormalIntensity;
                float3 _WaterFogColor;
                float _WaterFogDensity;
                float _RefractionIntensity;
                float _EdgeFoamAmount;
                float3 _TopBottomGradientColor;
                sampler2D _PerlinNoise;
                sampler2D _CameraOpaqueTexture;
                sampler2D _DetailNormal;
                float _DetailNormalIntensity;
                float _PerlinNoiseTiling;
                float4 _FoamColor;
                float4 _CameraDepthTexture_TexelSize;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct TessellationControlPoint
            {
                float3 positionWS : INTERNALTESSPOS;
                float4 positionCS : SV_POSITION;
                float3 normalWS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TessellationControlPoint Vertex(Attributes input)
            {
                TessellationControlPoint output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);

                output.positionWS = posInputs.positionWS;
                output.positionCS = posInputs.positionCS;
                
                output.normalWS = normalInputs.normalWS;

                output.uv = input.uv.xy;

                return output;
            }

            [domain("tri")]
            [outputcontrolpoints(3)]
            [outputtopology("triangle_cw")]
            [patchconstantfunc("PatchConstantFunction")] // Our Patch Constant Function
            [partitioning("integer")]
            TessellationControlPoint Hull(
                InputPatch<TessellationControlPoint, 3> patch, // Input triangle
                uint id : SV_OutputControlPointID) // Vertex index on the triangle
            {
                return patch[id];
            }

            // Linear normal interpolation
            #define NUM_BEZIER_CONTROL_POINTS 7

            struct TessellationFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
                float3 bezierPoints[NUM_BEZIER_CONTROL_POINTS] : BEZIERPOS;
            };

            float3 CalculateBezierControlPoint(
                float3 p0PositionWS,
                float3 aNormalWS,
                float3 p1PositionWS,
                float3 bNormalWS)
            {
                float w = dot(p1PositionWS - p0PositionWS, aNormalWS);
                return (p0PositionWS * 2 + p1PositionWS - w * aNormalWS) / 3.0F;
            }

            void CalculateBezierControlPoints(
                inout float3 bezierPoints[NUM_BEZIER_CONTROL_POINTS],
                float3 p0PositionWS,
                float3 p0NormalWS,
                float3 p1PositionWS,
                float3 p1NormalWS,
                float3 p2PositionWS,
                float3 p2NormalWS)
            {
                bezierPoints[0] = CalculateBezierControlPoint(p0PositionWS, p0NormalWS, p1PositionWS, p1NormalWS);
                bezierPoints[1] = CalculateBezierControlPoint(p1PositionWS, p1NormalWS, p0PositionWS, p0NormalWS);
                bezierPoints[2] = CalculateBezierControlPoint(p1PositionWS, p1NormalWS, p2PositionWS, p2NormalWS);
                bezierPoints[3] = CalculateBezierControlPoint(p2PositionWS, p2NormalWS, p1PositionWS, p1NormalWS);
                bezierPoints[4] = CalculateBezierControlPoint(p2PositionWS, p2NormalWS, p0PositionWS, p0NormalWS);
                bezierPoints[5] = CalculateBezierControlPoint(p0PositionWS, p0NormalWS, p2PositionWS, p2NormalWS);
                
                float3 avgBezier = 0;

                [unroll] for (int i = 0; i < 6; i++)
                {
                    avgBezier += bezierPoints[i];
                }
                
                avgBezier /= 6.0;
                float3 avgControl = (p0PositionWS + p1PositionWS + p2PositionWS) / 3.0;
                bezierPoints[6] = avgBezier + (avgBezier - avgControl) / 2.0;
            }

            bool IsOutOfBounds(float3 p, float3 lower, float3 higher)
            {
                return p.x < lower.x || p.x > higher.x ||
                       p.y < lower.y || p.y > higher.y ||
                       p.z < lower.z || p.z > higher.z;
            }

            bool IsPointOutOfFrustum(float4 positionCS)
            {
                float3 culling = positionCS.xyz;
                float w = positionCS.w;

                float3 lowerBounds = float3(
                    -w - _CullingTolerance,
                    -w - _CullingTolerance,
                    -w * UNITY_RAW_FAR_CLIP_VALUE - _CullingTolerance
                );
                float3 higherBounds = float3(
                    w + _CullingTolerance,
                    w + _CullingTolerance,
                    w + _CullingTolerance
                );

                return IsOutOfBounds(culling, lowerBounds, higherBounds);
            }

            // Returns true if the points in this triangle 
            // are wound counter-clockwise.
            bool ShouldBackFaceCull(
                float4 p0PositionCS, 
                float4 p1PositionCS,
                float4 p2PositionCS)
            {
                float3 point0 = p0PositionCS.xyz / p0PositionCS.w;
                float3 point1 = p1PositionCS.xyz / p1PositionCS.w;
                float3 point2 = p2PositionCS.xyz / p2PositionCS.w;

                // Known view direction in clip-space, so we only need
                // to test the z coordinate.
            #if UNITY_REVERSED_Z
                return cross(point1 - point0, point2 - point0).z < -_CullingTolerance;
            #else // Test is reversed in OpenGL
                return cross(point1 - point0, point2 - point0).z > _CullingTolerance;
            #endif
            }

            bool ShouldClipPatch(
                float4 p0PositionCS,
                float4 p1PositionCS,
                float4 p2PositionCS)
            {
                bool allOutside = IsPointOutOfFrustum(p0PositionCS) &&
                    IsPointOutOfFrustum(p1PositionCS) &&
                    IsPointOutOfFrustum(p2PositionCS);

                return allOutside;
            }

            // Barycentric interpolation as a function
            float3 BarycentricInterpolate(float3 bary, float3 a, float3 b, float3 c)
            {
                return bary.x * a + bary.y * b + bary.z * c;
            }

            float2 BarycentricInterpolate2D(float3 bary, float2 a, float2 b, float2 c)
            {
                return bary.x * a + bary.y * b + bary.z * c;
            }

            float3 PhongProjectedPosition(
                float3 flatPositionWS,
                float3 cornerPositionWS,
                float3 normalWS)
            {
                return flatPositionWS - dot(
                    flatPositionWS - cornerPositionWS,
                    normalWS
                ) * normalWS;
            }

            float3 CalculatePhongPosition(
                float3 bary,
                float3 p0PositionWS,
                float3 p0NormalWS,
                float3 p1PositionWS,
                float3 p1NormalWS,
                float3 p2PositionWS,
                float3 p2NormalWS)
            {
                float3 flatPositionWS = BarycentricInterpolate(
                    bary,
                    p0PositionWS,
                    p1PositionWS,
                    p2PositionWS
                );
                float3 smoothedPosiitonWS =
                    bary.x * PhongProjectedPosition(flatPositionWS, p0PositionWS, p0NormalWS) + 
                    bary.y * PhongProjectedPosition(flatPositionWS, p1PositionWS, p1NormalWS) + 
                    bary.z * PhongProjectedPosition(flatPositionWS, p2PositionWS, p2NormalWS);
                
                return smoothedPosiitonWS;
            }

            float3 CalculateBezierPosition(
                float3 bary,
                float smoothing,
                float3 bezierPoints[NUM_BEZIER_CONTROL_POINTS],
                float3 p0PositionWS,
                float3 p1PositionWS,
                float3 p2PositionWS)
            {
                float3 flatPositionWS = BarycentricInterpolate(
                    bary,
                    p0PositionWS,
                    p1PositionWS,
                    p2PositionWS
                );
                float3 smoothedPositionWS =
                    p0PositionWS * (bary.x * bary.x * bary.x) +
                    p1PositionWS * (bary.y * bary.y * bary.y) +
                    p2PositionWS * (bary.z * bary.z * bary.z) +
                    bezierPoints[0] * (3 * bary.x * bary.x * bary.y) +
                    bezierPoints[1] * (3 * bary.y * bary.y * bary.x) +
                    bezierPoints[2] * (3 * bary.y * bary.y * bary.z) +
                    bezierPoints[3] * (3 * bary.z * bary.z * bary.y) +
                    bezierPoints[4] * (3 * bary.z * bary.z * bary.x) +
                    bezierPoints[5] * (3 * bary.x * bary.x * bary.z) +
                    bezierPoints[6] * (6 * bary.x * bary.y * bary.z);
                
                return lerp(flatPositionWS, smoothedPositionWS, smoothing);
            }

            float EdgeTesselationFactor(
                float scale, 
                float bias,
                float3 p0PositionWS,
                float3 p1PositionWS)
            {
                float length = distance(p0PositionWS, p1PositionWS);
                float distanceToCamera = distance(
                    GetCameraPositionWS(),
                    (p0PositionWS + p1PositionWS) * 0.5F
                );
                float factor = length / (scale * distanceToCamera * distanceToCamera);

                return max(1, factor + bias);
            }

            // Runs once per triangle, or "patch"
            // Runs in parallel to the hull function
            TessellationFactors PatchConstantFunction(
                InputPatch<TessellationControlPoint, 3> patch)
            {
                UNITY_SETUP_INSTANCE_ID(patch[0]);

                TessellationFactors f;
                
                // Test if the patch is currently in the view
                if (ShouldClipPatch(patch[0].positionCS, patch[1].positionCS, patch[2].positionCS))
                {
                    // Cull patch
                    f.edge[0] = f.edge[1] = f.edge[2] = f.inside = 0;
                }
                else
                {
                    f.edge[0] = EdgeTesselationFactor(
                        _TessellationFactor * 0.01F, 
                        _TessellationBias, 
                        patch[1].positionWS, 
                        patch[2].positionWS
                    );
                    f.edge[1] = EdgeTesselationFactor(
                        _TessellationFactor * 0.01F, 
                        _TessellationBias, 
                        patch[2].positionWS, 
                        patch[0].positionWS
                    );
                    f.edge[2] = EdgeTesselationFactor(
                        _TessellationFactor * 0.01F, 
                        _TessellationBias, 
                        patch[0].positionWS, 
                        patch[1].positionWS
                    );

                    f.inside = (f.edge[0] + f.edge[1] + f.edge[2]) / 3.0F;

                    CalculateBezierControlPoints(
                        f.bezierPoints,
                        patch[0].positionWS,
                        patch[0].normalWS,
                        patch[1].positionWS,
                        patch[1].normalWS,
                        patch[2].positionWS,
                        patch[2].normalWS
                    );
                }

                return f;
            }

            ////////////////
            // DOMAIN
            ////////////////

            struct Interpolators
            {
                float3 normalWS : TEXCOORD1;
                float3 normalTS : TEXCOORD5;
                float3 positionWS : TEXCOORD2;
                float4 positionCS : SV_POSITION;
                float3 positionVS : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
                float2 uv : TEXCOORD0;
                float3 color : TEXCOORD6;
            };

            #define PI 3.141592654F

            float3 CalculateGerstnerWave(
                float4 wave,
                float3 pos,
                inout float3 tangent,
                inout float3 binormal)
            {
                float steepness = wave.z;
		        float wavelength = wave.w;
		        float k = 2 * PI / wavelength;
			    float c = sqrt(9.8 / k);
			    float2 d = normalize(wave.xy);
			    float f = k * (dot(d, pos.xz) - c * _Time.y);
			    float a = steepness / k;
                
                tangent += float3(
				    -d.x * d.x * (steepness * sin(f)),
				    d.x * (steepness * cos(f)),
				    -d.x * d.y * (steepness * sin(f))
			    );
                
			    binormal += float3(
				    -d.x * d.y * (steepness * sin(f)),
				    d.y * (steepness * cos(f)),
				    -d.y * d.y * (steepness * sin(f))
			    );
                
			    return float3(
				    d.x * (a * cos(f)),
				    a * sin(f) * _OverallHeightAdjust,
				    d.y * (a * cos(f))
			    );
            }

            #define BARYCENTRIC_INTERPOLATE(fieldName) \
                patch[0].fieldName * barycentricCoordinates.x + \
                patch[1].fieldName * barycentricCoordinates.y + \
                patch[2].fieldName * barycentricCoordinates.z

            [domain("tri")]
            Interpolators Domain(
                TessellationFactors factors,
                OutputPatch<TessellationControlPoint, 3> patch,
                float3 barycentricCoordinates : SV_DomainLocation)
            {
                Interpolators output;

                // Setup instancing and stereoscopic support
                UNITY_SETUP_INSTANCE_ID(patch[0]);
                UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 positionWS = CalculateBezierPosition(
                    barycentricCoordinates,
                    _Smoothing,
                    factors.bezierPoints,
                    patch[0].positionWS,
                    patch[1].positionWS,
                    patch[2].positionWS
                );
                float3 normalWS = BARYCENTRIC_INTERPOLATE(normalWS);

                float3 tangent = float3(1.0F, 0.0F, 0.0F);
                float3 binormal = float3(0.0F, 0.0F, 1.0F);

                float3 posAdd = 0.0F;

                posAdd += CalculateGerstnerWave(
                    _WaveA,
                    positionWS,
                    tangent,
                    binormal
                );
                posAdd += CalculateGerstnerWave(
                    _WaveB,
                    positionWS,
                    tangent,
                    binormal
                );
                posAdd += CalculateGerstnerWave(
                    _WaveC,
                    positionWS,
                    tangent,
                    binormal
                );

                positionWS += posAdd;

                float3 newNormal = normalize(cross(binormal, tangent));
                normalWS = lerp(normalWS, newNormal, _OverallHeightAdjust + (_NormalIntensity - 1.0F));

                output.positionCS = TransformWorldToHClip(positionWS);
                output.normalWS = normalWS;

                float3x3 tangentToWorld = float3x3(tangent, binormal, normalWS);
                tangentToWorld = transpose(tangentToWorld);

                output.normalTS = TransformWorldToTangent(normalWS, tangentToWorld, true);
                
                output.positionWS = positionWS;
                output.positionVS = TransformWorldToView(positionWS);
                output.screenPos = ComputeScreenPos(output.positionCS);
                output.color = (posAdd / 3.5F) + 0.25F;

                output.uv = BarycentricInterpolate2D(
                    barycentricCoordinates,
                    patch[0].uv,
                    patch[1].uv,
                    patch[2].uv
                );

                return output;   
            }

            ////////////////
            // FRAGMENT
            ////////////////

            float ObjectSurroundingFoam(float4 screenPos, float2 objUV)
            {
                float2 uv = screenPos.xy / screenPos.w;

                float fragmentEyeDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);
                float2 rawDepth = SampleSceneDepth(uv);
                float sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                float depthDiff = (sceneEyeDepth - fragmentEyeDepth);

                float foam = 1.0F - saturate(depthDiff / _EdgeFoamAmount);

                return saturate(smoothstep(0.0F, 0.5F, foam)) * _FoamColor.a;
            }

            float3 ColorBelowWater(float4 screenPos, float3 normal)
            {
                float2 uv = (screenPos.xy + (normal.xy * _RefractionIntensity)) / screenPos.w;
                
                float fragmentEyeDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);
                float2 rawDepth = SampleSceneDepth(uv);
                float sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                float depthDiff = (sceneEyeDepth - fragmentEyeDepth);

                if (depthDiff < 0.15)
                {
		            uv = screenPos.xy / screenPos.w;
		            #if UNITY_UV_STARTS_AT_TOP
			            if (_CameraDepthTexture_TexelSize.y < 0)
			            {
				            uv.y = 1 - uv.y;
			            }
		            #endif
                    rawDepth = SampleSceneDepth(uv);
                    sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
		            depthDiff = sceneEyeDepth - fragmentEyeDepth;
	            }
                
                float3 backgroundColor = tex2D(_CameraOpaqueTexture, uv).rgb;
                float fogFactor = exp2(-_WaterFogDensity * (depthDiff / 20.0F));
                
                return lerp(_WaterFogColor, backgroundColor, fogFactor);                
            }
            
            float4 Fragment(Interpolators input) : SV_Target
            {
                Light mainLight = GetMainLight();

                // Sample normal map
                
                float3 normalData = ((tex2D(_DetailNormal, input.uv * 3.0F - (_Time.xy * 0.05F)) * 2.0F) - 2.0F)
                    * _DetailNormalIntensity;
                
                float3 L = mainLight.direction;
                float3 N = input.normalWS;

                float3 NdotL = dot(N, L);
                float3 color = lerp(
                    float3(1.0F, 0.0F, 0.0F),
                    float3(0.0F, 0.0F, 1.0F),
                    input.positionWS.y
                );

                // Specular Lighting
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.positionWS);
                float3 reflectDir = reflect(-L, input.normalWS);
                float specular = pow(max(dot(viewDir, reflectDir), 0.0F), 32) * _SpecularIntensity;

                // Depth Fog

                float3 underwaterColor = ColorBelowWater(
                    input.screenPos,
                    (input.normalTS + normalData)
                );
                
                float edgeFoam = ObjectSurroundingFoam(input.screenPos, input.uv);

                float3 gradient = lerp(
                    _TopBottomGradientColor,
                    1.0F,
                    saturate(input.color.g)
                );

                float3 finalColor = ((_WaterColor * NdotL * gradient) + specular + underwaterColor + edgeFoam);
                //finalColor = lerp(finalColor, 1.0F, dot(_WorldSpaceCameraPos, input.positionWS));
                
                return float4(
                    finalColor,
                    1.0F
                );
            }
            
            ENDHLSL
        }
    }
}