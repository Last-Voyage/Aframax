/**************************************************************************
// File Name :          WaterShader.shader
// Author :             Miles Rogers
// Creation Date :      9/30/2024
//
// Brief Description :  The main water shader. Allows for
//                      refraction, light interaction, customizable
//                      wave spectra, object surrounding foam, and
//                      reactive ripples, if used with the RippleProjector
//                      prefab.
//                      
//                      Also dynamically tessellates based on camera
//                      distance, which is a majority of this file
//                      and the reason I didn't implement this in
//                      ShaderGraph. Feel free to skip past anything
//                      having to do with the Hull/Domain stages, if
//                      you just want to tweak the water appearance.
//                      These stages only concern how the mesh gets
//                      tessellated.
**************************************************************************/

Shader "Custom/WaterShader" 
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
        
        // A factor to multiply the evaluated wave height by
        _OverallHeightAdjust("Overall Height Adjust", Float) = 0.5
        
        // The three wave cascades (they are added together in the
        // domain stage)
        //
        // Expressed as a Vector4: (Direction, Steepness, 
        // Wavelength X, Wavelength Y)
        _WaveA ("Wave A (Dir, Steepness, Wavelength)", Vector) = (1, 0, 0.5, 10)
		_WaveB ("Wave B (Dir, Steepness, Wavelength)", Vector) = (0, 1, 0.25, 20)
		_WaveC ("Wave C (Dir, Steepness, Wavelength)", Vector) = (1, 1, 0.15, 10)
        
        // The base water color
        _WaterColor("Water Color", Color) = (0, 0, 1, 1)
        
        // The base water color
        _WaterTint("Water Tint", Color) = (1, 1, 1, 1)
        
        // The opacity of the overall specular contribution
        _SpecularIntensity("Specular Intensity", Float) = 1.0
        
        // The intensity of the evaluated wave normals
        _NormalIntensity("Normal Intensity", Float) = 1.0
        
        // The color the water depth fog is multiplied by
        _WaterFogColor ("Water Fog Color", Color) = (0, 0, 0, 0)
        
        // Contribution of the water depth fog
		_WaterFogDensity ("Water Fog Density", Range(0, 2)) = 0.1
        
        // The amount to UV warp the water reflections by 
        _RefractionIntensity("Refraction Intensity", Range(0, 1)) = 0.25
        
        // The width of the foam around the edges of objects
        // (calculated via depth-difference)
        _EdgeFoamAmount("Edge Foam Amount", Float) = 5.0
        
        // Top-bottom water gradient color
        _TopBottomGradientColor("Top -> Bottom Gradient Color", Color) = (1, 1, 1, 1)
        
        // Perlin noise texture
        _PerlinNoise("Perlin Noise", 2D) = "white" {}
        
        // The color of the edge foam is multiplied by
        _FoamColor("Foam Color", Color) = (1, 1, 1, 1)
        
        // The perlin noise normal map texture (used
        // for water ripple distortion)
        [Normal] _DetailNormal("Detail Normal", 2D) = "" {}
        
        // The intensity of the water ripple
        // distortion
        _DetailNormalIntensity("Detail Normal Intensity", Float) = 1
        
        ////////////////////////////
        // Ripple projection
        ////////////////////////////
        
        // (These are all controlled by a
        // script, so they are hidden in
        // the inspector)
        
        // The center of the projection
        [HideInInspector] _BoxCenter ("Box Center", Vector) = (0, 0, 0, 0)
        
        // The extents of the projection
        [HideInInspector] _BoxSize ("Box Size", Vector) = (10, 10, 10, 0)
        
        // Size of the render texture used for water ripples
        // (Needed to generate sobel normal map)
        [HideInInspector] _TextureSize("Texture Size", Int) = 128
        
        // The ripple render texture
        _RippleTex("Ripple Texture", 2D) = "black" {}
        
        // The intensity of the render texture
        // ripples on the water surface
        _RippleIntensity("Ripple Intensity", Float) = 2
        
        // Frequency of the ripples
        _RippleTiling("Ripple Tiling", Float) = 2
        
        // Speed of the ripples from center
        _RippleSpeed("Ripple Speed", Float) = 0.1
    }
    SubShader 
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Pass
        {
            HLSLPROGRAM

            #pragma target 5.0

            #pragma vertex Vertex
            #pragma hull Hull
            #pragma domain Domain
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            // Unity requires a re-declaration of all
            // ShaderLab Properties in the SubShader
            // pass.
            CBUFFER_START(UnityPerMaterial)
                float _TessellationFactor;
                float _TessellationBias;
                float _CullingTolerance;
                float _Smoothing;
                float _OverallHeightAdjust;
            
                float4 _WaveA;
                float4 _WaveB;
                float4 _WaveC;
            
                float4 _WaterColor;
                float4 _WaterTint;
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
                float4 _FoamColor;
                
                float3 _BoxCenter;
                float3 _BoxSize;
            
                sampler2D _RippleTex;
                float _RippleIntensity;
                float _RippleTiling;
                float _RippleSpeed;

                int _TextureSize;

                float4 _CameraDepthTexture_TexelSize;
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

            #define PI 3.141592654F

            // Calculate the position of the current vertex
            // coordinate based on the Gerstner Wave
            // function.
            float3 CalculateGerstnerWave(
                float4 wave,
                float3 pos,
                inout float3 tangent,
                inout float3 binormal)
            {
                const float steepness = wave.z;
                const float wavelength = wave.w;
                const float k = 2 * PI / wavelength;
                const float c = sqrt(9.8 / k);
			    float2 d = normalize(wave.xy);
                const float f = k * (
                    dot(d, pos.xz) - c * _Time.y
                );
                const float a = steepness / k;
                
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

                const float3 newNormal = normalize(
                    cross(binormal, tangent)
                );
                
                normalWS = lerp(
                    normalWS,
                    newNormal,
                    _OverallHeightAdjust + (_NormalIntensity - 1.0F)
                );

                output.positionCS = TransformWorldToHClip(positionWS);
                output.normalWS = normalWS;

                float3x3 tangentToWorld = float3x3(
                    tangent,
                    binormal,
                    normalWS
                );
                
                tangentToWorld = transpose(tangentToWorld);

                output.normalTS = TransformWorldToTangent(
                    normalWS,
                    tangentToWorld,
                    true
                );
                
                output.positionWS = positionWS;
                output.positionVS = TransformWorldToView(
                    positionWS
                );
                output.screenPos = ComputeScreenPos(
                    output.positionCS
                );
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
            // FRAGMENT STAGE
            ////////////////

            // Calculate foam that surrounds the object
            float ObjectSurroundingFoam(float4 screenPos, float2 objUV)
            {
                float2 uv = screenPos.xy / screenPos.w;
                
                const float2 rawDepth = SampleSceneDepth(uv);
                const float sceneEyeDepth = LinearEyeDepth(
                    rawDepth,
                    _ZBufferParams
                );
                const float foamLine = 1.0F -
                    saturate(
                        _EdgeFoamAmount *
                        (sceneEyeDepth - screenPos.w)
                );

                return smoothstep(
                    foamLine,
                    0.0F,
                    0.25F
                ) * _FoamColor.a;
            }

            // Water fog and reflection calculation
            float3 ColorBelowWater(float4 screenPos, float3 normal)
            {
                float2 uv = (
                    screenPos.xy +
                    (normal.xy *
                        _RefractionIntensity)) /
                            screenPos.w;

                const float fragmentEyeDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(
                    screenPos.z
                );
                
                float2 rawDepth = SampleSceneDepth(uv);
                float sceneEyeDepth = LinearEyeDepth(
                    rawDepth,
                    _ZBufferParams
                );
                
                float depthDiff = (
                    sceneEyeDepth -
                    fragmentEyeDepth
                ) / 20.0F;

                if (depthDiff < 0.0F)
                {
		            uv = screenPos.xy / screenPos.w;
		            #if UNITY_UV_STARTS_AT_TOP
			            if (_CameraDepthTexture_TexelSize.y < 0.0F)
			            {
				            uv.y = 1.0F - uv.y;
			            }
		            #endif
                    
                    rawDepth = SampleSceneDepth(uv);
                    sceneEyeDepth = LinearEyeDepth(
                        rawDepth,
                        _ZBufferParams
                    );
                    
		            depthDiff = sceneEyeDepth - fragmentEyeDepth;
	            }

                const float3 backgroundColor = tex2D(
                    _CameraOpaqueTexture,
                    uv
                ).rgb;
                
                const float fogFactor = exp2(
                    -_WaterFogDensity *
                    depthDiff
                );
                
                return lerp(
                    _WaterFogColor,
                    backgroundColor,
                    fogFactor
                );
            }

            // Use the ripple map gradient to sample
            // our perlin noise texture to simulate
            // ripple waves.
            float RippleSample(float u)
            {
                return tex2D(
                    _PerlinNoise,
                    float2(
                        (u * _RippleTiling) +
                        (_Time.x * _RippleSpeed),
                        0.0F
                )).r;
            }

            float3 SobelNormal(float2 uv, float3 normal, float3 tangent)
            {
                float texelStride = 1.0F / _TextureSize;
                
                float p = RippleSample(tex2D(_RippleTex, uv).r);
                float h1 = RippleSample(tex2D(
                    _RippleTex, uv + float2(texelStride, 0.0F))
                );
                float v1 = RippleSample(tex2D(
                    _RippleTex, uv + float2(0.0F, texelStride))
                );

                float2 t = p - float2(h1, v1);
                float3 n = float3(t.x, t.y, 1.0F);

                return n * _RippleIntensity;
            }

            float CalculateSpecular(float3 normal, float3 viewDir)
            {
                // Main light
                Light mainLight = GetMainLight();

                float diffuse = saturate(dot(normal, mainLight.direction));
                float specularDot = saturate(dot(
                    normal,
                    normalize(mainLight.direction + viewDir)
                ));

                // Output specular
                return pow(diffuse * specularDot, 16.0) * _SpecularIntensity;
            }
            
            float4 Fragment(Interpolators input) : SV_Target
            {
                // For any calculation that needs the light
                // direction vector
                Light mainLight = GetMainLight();

                // Sample detail normal map
                float3 normalData = ((tex2D(
                    _DetailNormal,
                    input.uv * 3.0F -
                    (_Time.xy * 0.05F))
                    * 2.0F) - 2.0F)
                        * _DetailNormalIntensity;

                float3 wsNormal = input.normalWS;

                // Calculate the world coordinates of our box
                const float boxMinX = _BoxCenter.x - _BoxSize.x;
                const float boxMaxX = _BoxCenter.x + _BoxSize.x;
                const float boxMinZ = _BoxCenter.z - _BoxSize.z;
                const float boxMaxZ = _BoxCenter.z + _BoxSize.z;

                // If the WS fragment position is in
                // the box
                if (input.positionWS.x > boxMinX &&
                    input.positionWS.x < boxMaxX)
                {
                    if (input.positionWS.z > boxMinZ &&
                        input.positionWS.z < boxMaxZ)
                    {
                        float uCoord = RangeRemap(
                            boxMinX,
                            boxMaxX,
                            input.positionWS.x
                        );
                        float vCoord = RangeRemap(
                            boxMinZ,
                            boxMaxZ,
                            input.positionWS.z
                        );
                        
                        float rippleVal = tex2D(
                            _RippleTex, float2(
                                uCoord,
                                vCoord
                        )).r;

                        const float3 calcNormal = SobelNormal(
                            float2(uCoord, vCoord),
                            input.normalWS,
                            input.normalTS
                        );

                        wsNormal += float3(calcNormal.xy, 0.0F);
                        normalData += calcNormal;
                    }
                }

                // Main light vector
                const float3 L = mainLight.direction;

                // World space normal vector
                const float3 N = normalize(wsNormal);

                // Diffuse lighting term
                const float3 NdotL = dot(N, L);

                // Specular Lighting
                const float3 viewDir = normalize(
                    _WorldSpaceCameraPos -
                    input.positionWS
                );

                // Reflection angle
                const float3 reflectDir = reflect(-L, -N);

                // Specular contribution
                const float specular = CalculateSpecular(
                    input.normalWS,
                    GetWorldSpaceNormalizeViewDir(input.positionWS)
                );

                // Depth Fog
                const float edgeFoam = ObjectSurroundingFoam(
                    input.screenPos,
                    input.uv
                );

                // Calculate color under the water w/ fog
                const float3 underwaterColor = ColorBelowWater(
                    input.screenPos,
                    (input.normalTS + normalData)
                );

                // Top-bottom gradient based on vertex height
                const float3 gradient = lerp(
                    _TopBottomGradientColor,
                    1.0F,
                    saturate(input.color.g)
                );

                // Adding all the colors together
                float3 finalColor = (
                    _WaterColor *
                    _WaterTint *
                    NdotL *
                    gradient) +
                        specular +
                        underwaterColor +
                        edgeFoam;

                // Pixel color output
                return float4(
                    finalColor,
                    1.0F
                );
            }
            
            ENDHLSL
        }
    }
}