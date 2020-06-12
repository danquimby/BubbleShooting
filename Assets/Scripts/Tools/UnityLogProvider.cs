using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public class Loggger
{
    private static ILogger _logger = Debug.unityLogger;
    private string _tag;

    public Loggger(string tag)
    {
        Assert.IsNotNull(tag, "tag can't be null or empty");
        _tag = tag;
    }

    public void i(string message)
    {
#if (UNITY_EDITOR)
        _logger.Log( $"<color=blue><b>{_tag}</b>: </color> {message}");
#endif
    }
    public void w(string message)
    {
#if (UNITY_EDITOR)
        _logger.LogWarning("", $"<color=yellow><b>{_tag}</b>: </color> {message}");
#endif
    }
    public void e(string message)
    {
#if (UNITY_EDITOR)
        _logger.LogError("", $"<color=red><b>{_tag}: {message}</b></color>");
#endif        
        
    }

    void prepare_format()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
        Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.Full);
        Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);
    }
}

public static class LoggerProvider
{
    public static Loggger get(MonoBehaviour mono)
    {
        return new Loggger(mono.GetType().Name);
    }
}
