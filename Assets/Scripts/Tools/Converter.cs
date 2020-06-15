
using System;
using UnityEngine;

public static class Converter
{
    public static readonly Vector3 Offset = new Vector3(-2,4, 0);
    public static readonly float Scale = 0.5f;

    public static Vector3 ToViewPosition(Position position, int z=0)
    {
        return Offset + new Vector3(position.Row * Scale, -(position.Column * Scale), z);
    }
    public static Position ToGridPosition(Vector3 position)
    {
        Vector3 _position = Offset - position;
        return new Position(
            (int) Math.Round(Math.Abs(_position.x / Scale)), 
            (int) Math.Round(Math.Abs(_position.y / Scale))
            );
    }
}