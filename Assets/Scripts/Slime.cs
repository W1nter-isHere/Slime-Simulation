using Unity.Mathematics;

public struct Slime
{
    public int2 Position;
    public float Angle;
    public float Speed;
    public uint Valid;

    public Slime(int2 position, float angle, float speed = 10)
    {
        Position = position;
        Angle = angle;
        Speed = speed;
        Valid = 1;
    }
}