#ifndef GET_RAY_DIR_INCLUDED
#define GET_RAY_DIR_INCLUDED

void GetRayDirection_float(
    float2 ScreenUv,
    out float3 RayDirection)
{
    float3 right = UNITY_MATRIX_V._m00_m01_m02;
    float3 up = UNITY_MATRIX_V._m10_m11_m12;
    float3 forward = -UNITY_MATRIX_V._m20_m21_m22;

    float2 uv = (ScreenUv * 2.0f) - 1.0f;
    uv.x *= (_ScreenParams.x / _ScreenParams.y);

    RayDirection = normalize(
        (uv.x * right) + 
        (uv.y * up) + 
        (forward * 2.0F)
    );
}

#endif