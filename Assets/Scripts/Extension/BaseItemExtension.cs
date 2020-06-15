using UnityEngine;
using UnityEngine.Assertions;

public static class BaseItemExtension 
{
    /// <summary>
    /// Convert string to enum of game tags
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static BaseItem toBaseItem(this GameObject obj)
    {
        Assert.IsNotNull(obj);
        BaseBehavior _base = obj.GetComponent<BaseBehavior>();
        Assert.IsNull(_base, "Game object not contains BaseBehavior");
        return _base as BaseItem;
    }
}