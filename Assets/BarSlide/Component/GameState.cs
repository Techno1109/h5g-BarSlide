using Unity.Entities;

public struct GameState : IComponentData
{
    public bool End;
    public int Score;
    public float Lv;
}
