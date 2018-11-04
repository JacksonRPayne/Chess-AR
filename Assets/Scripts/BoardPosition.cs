using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BoardPosition{

    /// <summary>
    /// X position on the board (0 is left on white side)
    /// </summary>
    public int x;

    /// <summary>
    /// Y position on the board (0 is bottom on white side)
    /// </summary>
    public int y;

    /// <summary>
    /// Compares the x and y values to determine if the BoardPositions are equal
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        //Casts the parameter as a BoardPosition
        BoardPosition bp = (BoardPosition)obj;
        //Returns whether the x and ys are the same
        return (x == bp.x && y == bp.y);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}
