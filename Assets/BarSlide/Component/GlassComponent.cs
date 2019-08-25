using Unity.Entities;

public struct GlassComponent : IComponentData
{
    public bool Active;
    public bool EndTag;
    public float AddSpeed;
    public float MaxValue;
    public float NowValue;
    public bool charging;
}
