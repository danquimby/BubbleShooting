
using System;
using UnityEngine;

static public class Converter
{
    public static Vector3 Offset = new Vector3(-2,4, 0);
    public static float Scale = 0.5f;

    public static Vector3 ToViewPosition(Position position, int z=0)
    {
        return Offset + new Vector3(position.Row * Scale, -(position.Column * Scale), z);
    }

    public static Position ToGridPosition(Vector3 position)
    {
        Vector3 _position = position - Offset;
        _position.x /= Scale;
        _position.y /= Scale;
        Position pos = new Position((int) Math.Round(Math.Abs(_position.x)), (int) Math.Round(Math.Abs(_position.y)));
        return pos;
    }
}