using Unity.Entities;
using Unity.Tiny.Input;

public struct OKAreaTag : IComponentData
{
    public float MinPos;
    public float MaxPos;
    public float MinSize;
    public float MaxSize;
    public Pointer Point;
}
