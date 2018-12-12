#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Builtin/BuiltinData.hlsl"

#define TILE_SIZE                   16u
#define WAVE_SIZE					64u

#ifdef VELOCITY_PREPPING
RWTexture2D<float3> _VelocityAndDepth;
#else
Texture2D<float3> _VelocityAndDepth;
#endif

float2 VelocityToNormalized(float2 velocity)
{
    return velocity * 0.5f + 0.5f;
}

float2 NormalizedVelToClampedNDCVel(float2 velocity)
{
    // Back to -1, 1
    return velocity * 2.0f - 1.0f;
}
