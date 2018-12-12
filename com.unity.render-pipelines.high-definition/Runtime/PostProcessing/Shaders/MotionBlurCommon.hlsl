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

#ifdef GEN_PASS
RWTexture2D<float3> _TileMinMaxVel;
#else
Texture2D<float3> _TileMinMaxVel;
#endif

#if NEIGHBOURHOOD_PASS
RWTexture2D<float2> _TileMaxNeighbourhood;
#else
Texture2D<float2> _TileMaxNeighbourhood;
#endif


CBUFFER_START(MotionBlurUniformBuffer)
float4 _TileTargetSize;     // .xy size, .zw 1/size
float  _MinSqVelThreshold;
float  _MinMaxSqVelRatioForSlowPath;
int    _SampleCount;
CBUFFER_END


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
        float pixelLength = length(_ScreenSize.zw);
        velLength = min(velLength, pixelLength * 64);
        /////

        float theta = atan2(velocity.y, velocity.x) + PI;       // TODO_FCC: Verify if it's beneficial to move the +PI as -PI during decoding.
        return float2(velLength, theta);
    }
#else
    return velocity;
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
    return v.x < w.x ? v : w;
}

float2 MaxVel(float2 v, float2 w)
{
    return v.x < w.x ? w : v;
}



