#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Builtin/BuiltinData.hlsl"

#define TILE_SIZE                   32u
#define WAVE_SIZE					64u

#ifdef VELOCITY_PREPPING
RWTexture2D<float3> _VelocityAndDepth;
#else
Texture2D<float3> _VelocityAndDepth;
#endif

#ifdef GEN_PASS
RWTexture2D<float3> _TileMinMaxVel;
#else
Texture2D<float3> _TileMinMaxVel;
#endif

#if NEIGHBOURHOOD_PASS
RWTexture2D<float4> _TileMaxNeighbourhood;
#else
Texture2D<float4> _TileMaxNeighbourhood;
#endif


CBUFFER_START(MotionBlurUniformBuffer)
float4x4 _PrevVPMatrixNoTranslation;
float4 _TileTargetSize;     // .xy size, .zw 1/size
float4 _MotionBlurParams0;  // Unpacked below.
float _MotionBlurIntensity;
int    _SampleCount;
CBUFFER_END

#define _ScreenMagnitude _MotionBlurParams0.x
#define _MotionBlurMaxVelocity _MotionBlurParams0.y
#define _MinVelThreshold  _MotionBlurParams0.z
#define _MinMaxVelRatioForSlowPath _MotionBlurParams0.w


// --------------------------------------
// Encoding/Decoding
// --------------------------------------
#define PACKING 0

// We use polar coordinates. This has the advantage of storing the length separately and we'll need the length several times.
// This returns a couple { Length, Angle }
// TODO_FCC: Profile! We should be fine since this is going to be in a bw bound pass, but worth checking as atan2 costs a lot. 
float2 EncodeVelocity(float2 velocity)
{

#if PACKING
    float velLength = length(velocity);
    if (velLength < 0.0001f)
    {
        return 0.0f;
    }
    else
    {
        /// TODO_FCC: This is to be removed
        velLength = min(velLength, _ScreenMagnitude * 64);
        /////

        float theta = atan2(velocity.y, velocity.x) + PI;       // TODO_FCC: Verify if it's beneficial to move the +PI as -PI during decoding.
        return float2(velLength, theta);
    }
#else

    float len = length(velocity);

    // TODO_FCC: PASS THIS 64 AS PARAM
    if(len > 0)
    {
        return min(len, _MotionBlurMaxVelocity / _ScreenMagnitude) * normalize(velocity);
    }
    else return 0;
#endif
}

float VelocityLengthFromEncoded(float2 velocity)
{
#if PACKING
    return  velocity.x;
#else
    return length(velocity);
#endif
}

float VelocityLengthInPixelsFromEncoded(float2 velocity)
{
#if PACKING
    return  velocity.x * _ScreenMagnitude;
#else
    return length(velocity * _ScreenSize.xy);
#endif
}

float2 DecodeVelocityFromPacked(float2 velocity)
{
#if PACKING
    float theta = velocity.y * (TWO_PI);
    return  float2(sin(theta), cos(theta)) * velocity.x;
#else
    return velocity;
#endif
}


// --------------------------------------
// Misc functions that work on encoded representation
// --------------------------------------

float2 MinVel(float2 v, float2 w)
{
    return VelocityLengthFromEncoded(v) < VelocityLengthFromEncoded(w) ? v : w;
}

float2 MaxVel(float2 v, float2 w)
{
    return VelocityLengthFromEncoded(v) < VelocityLengthFromEncoded(w) ? w : v;
}



