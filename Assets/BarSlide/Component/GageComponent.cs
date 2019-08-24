using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.UIControls;

public struct GageComponent : IComponentData
{
    public float MaxValue;
    public float FirstLength;
    public float FistPos;
    public float NowValue;
}
