using Unity.Mathematics;
using UnityEngine;

public struct Slime
{
    public int2 Position;
    public float Angle;
    public float Speed;
    public float TimeElapsed;
    public Color Color;
    public uint MultipliedSpeed;
    public uint Valid;

    public Slime(int2 position, float angleInDegrees, Color color, float speed = 10)
    {
        Position = position;
        Angle = angleInDegrees * Mathf.Deg2Rad;
        Speed = speed;
        TimeElapsed = 0;
        Color = color;
        MultipliedSpeed = 0;
        Valid = 1;
    }
}