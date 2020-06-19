
using System;
using UnityEngine;

public static class Converter
{
    // offset for created balls
    // maybe change to unity game object
    public static readonly Vector3 Offset = new Vector3(-1.5f,3.55f, -0.15f);
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