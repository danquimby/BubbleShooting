using System;
using UnityEngine;

[Serializable]
public class Position
{
    public static Vector3 Offset = new Vector3(-2,4, 0);
    public static float Scale = 0.5f;
    [SerializeField] private int column; // y
    [SerializeField] private int row; // x

    public int Column
    {
        get => column;
        set => column = value;
    }

    public int Row
    {
        get => row;
        set => row = value;
    }
    public Position(int row = Int32.MaxValue, int column = Int32.MaxValue)
    {
        this.row = row;
        this.column = column;
    }
    public bool isEmpty()
    {
        return row == Int32.MaxValue && column == Int32.MaxValue;
    }
    public override string ToString()
    {
        return $"Column={Column} Row={Row}";
    }
    public void Clear()
    {
        this.column = this.row = 0;
    }
    public override bool Equals(object obj)
    {
        if (!isEmpty() && obj != null && obj is Position)
            return (obj as Position).column == this.column && (obj as Position).row == this.row;
        return false;
    }

    public static Position zero()
    {
        return new Position();
    }
}
