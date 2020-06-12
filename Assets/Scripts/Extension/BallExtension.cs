using System;
using UnityEngine;
using UnityEngine.Assertions;

public static class BallExtension 
{
    /// <summary>
    /// Convert string to enum of game tags
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Ball toBall(this GameObject obj)
    {
        Assert.IsNotNull(obj);
        BaseBehavior _base = obj.GetComponent<BaseBehavior>();
        return _base as Ball;
    }
}