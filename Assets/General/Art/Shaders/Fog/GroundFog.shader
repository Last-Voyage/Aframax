/**************************************************************************
// File Name :          GroundFog.shader
// Author :             Miles Rogers
// Creation Date :      9/30/2024
//
// Brief Description :  
**************************************************************************/

Shader "Custom/GroundFog" 
{
    Properties 
    {
        // Max allowed tessellation (smaller is higher res)
        _TessellationFactor("Tessellation", Range(0.0, 1.0)) = 1.0
        
        // The factor for how much to adjust the tessellation
        // based on camera distance
        _TessellationBias("Tessellation Bias", Float) = 0.0
        
        // How aggressively to hide polygons when off-screen
        // (Adjust this value if you notice gaps at the edge
        // of the screen)
        _CullingTolerance("Culling Tolerance", Float) = 0.1
        
        // How much Bezier smoothing should be applied
        // after tessellation?
        _Smoothing("Smoothing", Float) = 1.0
        
        _FogColor("Fog Color", Color) = (0, 0, 0, 1)
        
        _HeightIntensity("Height Intensity", Float) = 1.0
        
        _NoiseTexture("Noise", 2D) = "white" {}
        
        _ScrollSpeed("Scroll Speed", Float) = 0.1        
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
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            HLSLPROGRAM

            #pragma target 5.0

            #pragma vertex Vertex
            #pragma hull Hull
            #pragma domain Domain
            #pragma fragment Fragment
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3 _GBUFFER_NORMALS_OCT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"

            // Unity requires a re-declaration of all
            // ShaderLab Properties in the SubShader
            // pass.
            CBUFFER_START(UnityPerMaterial)
                float _TessellationFactor;
                float _TessellationBias;
                float _CullingTolerance;
                float _Smoothing;
                float4 _FogColor;
                float _HeightIntensity;
                sampler2D _NoiseTexture;
                float _ScrollSpeed;
            CBUFFER_END

            // Vertex buffer info struct
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // Input struct for the tessellation stage
            struct TessellationControlPoint
            {
                float3 positionWS : INTERNALTESSPOS;
                float4 positionCS : SV_POSITION;
                float3 normalWS : NORMAL;
                float2 uv : TEXCOORD0;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            ////////////////
            // VERTEX STAGE
            ////////////////

            TessellationControlPoint Vertex(Attributes input)
            {
                TessellationControlPoint output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                const VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS);
                const VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);

                output.positionWS = posInputs.positionWS;
                output.positionCS = posInputs.positionCS;
                
                output.normalWS = normalInputs.normalWS;

                output.uv = input.uv.xy;

                return output;
            }

            ////////////////
            // HULL STAGE
            ////////////////

            // Determines how our triangles will get
            // tessellated later.
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

            // For linear normal interpolation and
            // smoothing of the tessellated surface
            #define NUM_BEZIER_CONTROL_POINTS 7

            // Data struct for how tessellation should be
            // performed.
            struct TessellationFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
                float3 bezierPoints[NUM_BEZIER_CONTROL_POINTS] : BEZIERPOS;
            };

            // Bezier interpolation helper function
            float3 CalculateBezierControlPoint(
                float3 p0PositionWS,
                float3 aNormalWS,
                float3 p1PositionWS,
                float3 bNormalWS)
            {
                const float w = dot(p1PositionWS - p0PositionWS, aNormalWS);
                return (p0PositionWS * 2 + p1PositionWS - w * aNormalWS) / 3.0F;
            }

            // Helps smooth out the surface with bezier interpolation
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

                const float3 avgControl = (
                    p0PositionWS + p1PositionWS + p2PositionWS
                ) / 3.0;
                
                bezierPoints[6] = avgBezier + (avgBezier - avgControl) / 2.0;
            }

            // Occlusion culling
            bool IsOutOfBounds(float3 p, float3 lower, float3 higher)
            {
                return p.x < lower.x || p.x > higher.x ||
                       p.y < lower.y || p.y > higher.y ||
                       p.z < lower.z || p.z > higher.z;
            }

            // Frustum culling
            bool IsPointOutOfFrustum(float4 positionCS)
            {
                const float3 culling = positionCS.xyz;
                const float w = positionCS.w;

                const float3 lowerBounds = float3(
                    -w - _CullingTolerance,
                    -w - _CullingTolerance,
                    -w * UNITY_RAW_FAR_CLIP_VALUE -
                    _CullingTolerance
                );
                
                const float3 higherBounds = float3(
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

            // Test if a given patch should be culled by the shader
            bool ShouldClipPatch(
                float4 p0PositionCS,
                float4 p1PositionCS,
                float4 p2PositionCS)
            {
                const bool allOutside = IsPointOutOfFrustum(p0PositionCS) &&
                    IsPointOutOfFrustum(p1PositionCS) &&
                    IsPointOutOfFrustum(p2PositionCS);

                return allOutside;
            }

            // Barycentric interpolation as a function
            float3 BarycentricInterpolate(float3 bary, float3 a, float3 b, float3 c)
            {
                return bary.x * a + bary.y * b + bary.z * c;
            }

            // Barycentric interpolation of 2D coordinates
            float2 BarycentricInterpolate2D(float3 bary, float2 a, float2 b, float2 c)
            {
                return bary.x * a + bary.y * b + bary.z * c;
            }

            // Phong interpolation helper
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

            // Calculate phong smoothing of a patch
            float3 CalculatePhongPosition(
                float3 bary,
                float3 p0PositionWS,
                float3 p0NormalWS,
                float3 p1PositionWS,
                float3 p1NormalWS,
                float3 p2PositionWS,
                float3 p2NormalWS)
            {
                const float3 flatPositionWS = BarycentricInterpolate(
                    bary,
                    p0PositionWS,
                    p1PositionWS,
                    p2PositionWS
                );
                
                float3 smoothedPositionWS =
                    bary.x * PhongProjectedPosition(
                        flatPositionWS,
                        p0PositionWS,
                        p0NormalWS) + 
                    bary.y * PhongProjectedPosition(
                        flatPositionWS,
                        p1PositionWS,
                        p1NormalWS) + 
                    bary.z * PhongProjectedPosition(
                        flatPositionWS,
                        p2PositionWS,
                        p2NormalWS
                );
                
                return smoothedPositionWS;
            }

            // Calculate bezier smoothing of a patch
            float3 CalculateBezierPosition(
                float3 bary,
                float smoothing,
                float3 bezierPoints[NUM_BEZIER_CONTROL_POINTS],
                float3 p0PositionWS,
                float3 p1PositionWS,
                float3 p2PositionWS)
            {
                const float3 flatPositionWS = BarycentricInterpolate(
                    bary,
                    p0PositionWS,
                    p1PositionWS,
                    p2PositionWS
                );

                const float3 smoothedPositionWS =
                    p0PositionWS * (bary.x * bary.x * bary.x) +
                    p1PositionWS * (bary.y * bary.y * bary.y) +
                    p2PositionWS * (bary.z * bary.z * bary.z) +
                    bezierPoints[0] * (3 * bary.x * bary.x * bary.y) +
                    bezierPoints[1] * (3 * bary.y * bary.y * bary.x) +
                    bezierPoints[2] * (3 * bary.y * bary.y * bary.z) +
                    bezierPoints[3] * (3 * bary.z * bary.z * bary.y) +
                    bezierPoints[4] * (3 * bary.z * bary.z * bary.x) +
                    bezierPoints[5] * (3 * bary.x * bary.x * bary.z) +
                    bezierPoints[6] * (6 * bary.x * bary.y * bary.z
                );
                
                return lerp(flatPositionWS, smoothedPositionWS, smoothing);
            }

            float EdgeTesselationFactor(
                float scale, 
                float bias,
                float3 p0PositionWS,
                float3 p1PositionWS)
            {
                const float length = distance(p0PositionWS, p1PositionWS);

                const float distanceToCamera = distance(
                    GetCameraPositionWS(),
                    (p0PositionWS + p1PositionWS) * 0.5F
                );
                
                const float factor = length / (
                    scale *
                    distanceToCamera *
                    distanceToCamera
                );

                return max(1.0F, factor + bias);
            }

            // Runs once per triangle, or "patch"
            // Runs in parallel to the hull function
            TessellationFactors PatchConstantFunction(
                InputPatch<TessellationControlPoint, 3> patch)
            {
                UNITY_SETUP_INSTANCE_ID(patch[0]);

                TessellationFactors f;
                
                // Test if the patch is currently in the view
                if (ShouldClipPatch(
                    patch[0].positionCS,
                    patch[1].positionCS,
                    patch[2].positionCS))
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

                    f.inside = (
                        f.edge[0] +
                        f.edge[1] +
                        f.edge[2]
                    ) / 3.0F;

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
            // DOMAIN STAGE
            ////////////////

            // What to do with our vertices post-tessellation
            // Everything "in the domain" of the mesh surface

            // Fragment shader input data struct
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

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            #define BARYCENTRIC_INTERPOLATE(fieldName) \
                patch[0].fieldName * barycentricCoordinates.x + \
                patch[1].fieldName * barycentricCoordinates.y + \
                patch[2].fieldName * barycentricCoordinates.z

            // The vertex shader over all tessellated
            // points.
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

                float2 uv = BarycentricInterpolate2D(
                    barycentricCoordinates,
                    patch[0].uv,
                    patch[1].uv,
                    patch[2].uv
                ) + (_Time * _ScrollSpeed);

                positionWS.y += tex2Dlod(_NoiseTexture, float4(uv.xy, 0.0, 0.0)).r * _HeightIntensity;
                
                float3 normalWS = BARYCENTRIC_INTERPOLATE(normalWS);

                output.positionCS = TransformWorldToHClip(positionWS);
                output.positionWS = positionWS;
                output.positionVS = TransformWorldToView(
                    positionWS
                );
                output.screenPos = ComputeScreenPos(
                    output.positionCS
                );
                output.color = 1.0F;

                output.uv = uv;

                return output;   
            }

            ////////////////
            // FRAGMENT STAGE
            ////////////////
            
            float4 Fragment(Interpolators input) : SV_Target
            {
                float4 col = tex2D(_NoiseTexture, input.uv);
                col.a = col.r * 4.0 * _FogColor.a;
                return col * _FogColor;
            }
            
            ENDHLSL
        }
    }
}