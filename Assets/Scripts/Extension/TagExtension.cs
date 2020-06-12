using System;
using UnityEngine.Assertions;

public static class TagExtension 
{
    /// <summary>
    /// Convert string to enum of game tags
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameTags ToTag(this string tag)
    {
        Assert.IsNotNull(tag);
        return (GameTags) Enum.Parse(typeof(GameTags), tag, true);
    }
}