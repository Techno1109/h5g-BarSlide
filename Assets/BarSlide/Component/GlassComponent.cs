using Unity.Entities;

public struct GlassComponent : IComponentData
{
    public float AddSpeed;
    public float MaxValue;
    public float NowValue;
    public bool charging;
}
