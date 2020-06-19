using UnityEngine;
using UnityEngine.Assertions;

public static class DragObjectExtension
{
    public static DragObject toDragObject(this GameObject obj)
    {
        Assert.IsNotNull(obj);
        BaseBehavior _base = obj.GetComponent<DragObject>();
        return _base as DragObject;
    }
        
}