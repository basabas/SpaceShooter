using System;
using UnityEngine;

namespace Bas.Debug
{
    public static class Logging
    {
        public static void Log(object message) => UnityEngine.Debug.Log(message);
        public static void Log(object message, GameObject o) => UnityEngine.Debug.Log(message, o);
        public static void LogError(object message) => UnityEngine.Debug.LogError(message);
        public static void LogWarning(object message) => UnityEngine.Debug.LogWarning(message);
        public static void LogException(Exception exception) => UnityEngine.Debug.LogException(exception);
    }
}
