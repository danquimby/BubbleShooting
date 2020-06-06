using System;
using UnityEngine;

[Serializable]
public class Position
{
    [SerializeField] private int column;
    [SerializeField] private int row;

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

    public Position(int row = 0, int column = 0)
    {
        this.row = row;
        this.column = column;
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
        if (obj != null && obj is Position)
            return (obj as Position).column == this.column && (obj as Position).row == this.row;
        return false;
    }

    public static Position zero()
    {
        return new Position();
    }
}
