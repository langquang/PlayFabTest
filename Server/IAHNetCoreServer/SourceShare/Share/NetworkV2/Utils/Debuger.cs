using System;
using System.Collections.Generic;

namespace SourceShare.Share.NetworkV2.Utils
{
    public enum DebuggerLevel
    {
        Warning,
        Error,
        Trace,
        Info
    }

    /// <summary>
    /// Interface to implement for your own logger
    /// </summary>
    public interface ILogger
    {
        void WriteNet(DebuggerLevel level, string str, params object[] args);
    }

    /// <summary>
    /// Static class for defining your own LiteNetLib logger instead of Console.WriteLine
    /// or Debug.Log if compiled with UNITY flag
    /// </summary>
    public static class Debugger
    {
        private static          ILogger Logger       = null;
        private static readonly object  DebugLogLock = new object();

        private static void WriteLogic(DebuggerLevel logLevel, string str, params object[] args)
        {
            lock (DebugLogLock)
            {
                if (Logger == null)
                {
#if UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
                    if (logLevel == DebuggerLevel.Error)
                    {
                        if (args == null || args.Length == 0)
                            UnityEngine.Debug.LogError(str);
                        else
                            UnityEngine.Debug.LogError(string.Format(str, args));
                    }
                    else if (logLevel == DebuggerLevel.Warning)
                    {
                        if (args == null || args.Length == 0)
                            UnityEngine.Debug.LogWarning(str);
                        else
                            UnityEngine.Debug.LogWarning(string.Format(str, args));
                    }
                    else
                    {
                        if (args == null || args.Length == 0)
                            UnityEngine.Debug.Log(str);
                        else
                            UnityEngine.Debug.Log(string.Format(str, args));
                    }
#else
                    Console.WriteLine(str, args);
#endif
                }
                else
                {
                    Logger.WriteNet(logLevel, str, args);
                }
            }
        }

        public static void Write(string str, params object[] args)
        {
            WriteLogic(DebuggerLevel.Trace, str, args);
        }

        public static void Write(DebuggerLevel level, string str, params object[] args)
        {
            WriteLogic(level, str, args);
        }

        public static void WriteForce(string str, params object[] args)
        {
            WriteLogic(DebuggerLevel.Trace, str, args);
        }

        public static void WriteForce(DebuggerLevel level, string str, params object[] args)
        {
            WriteLogic(level, str, args);
        }

        public static void WriteError(string str, params object[] args)
        {
            WriteLogic(DebuggerLevel.Error, $"[Error] {str}", args);
        }

        public static void WriteWarning(string str, params object[] args)
        {
            WriteLogic(DebuggerLevel.Warning, $"[Warning] {str}", args);
        }

        public static string FindConstName<T>(int code)
        {
            foreach (var field in typeof(T).GetFields())
            {
                if ((int) field.GetValue(null) == code)
                    return field.Name;
            }

            return $"Unknown prop: {code}";
        }

        public static void Write<K, V>(Dictionary<K, V> dictionary, string dictionaryName = null, Func<K, string> keyMapper = null)
        {
            if (dictionary == null)
                return;

            var realName = dictionaryName ?? string.Empty;
            WriteLogic(DebuggerLevel.Trace, $"Dictionary {realName}:");
            foreach (var pair in dictionary)
            {
                var keyName = pair.Key.ToString();
                if (keyMapper != null)
                    keyName = keyMapper.Invoke(pair.Key);
                WriteLogic(DebuggerLevel.Trace, $"  ** {keyName} => {pair.Value.ToString()}");
            }
        }

        public static void Write<T>(IEnumerable<T> collection, string collectionName = null, Func<T, string> keyMapper = null)
        {
            if (collection == null)
                return;

            var realName = collectionName ?? string.Empty;
            WriteLogic(DebuggerLevel.Trace, $"Collection {realName}:");
            foreach (var item in collection)
            {
                var keyName = string.Empty;
                if (keyMapper != null)
                    keyName = keyMapper.Invoke(item);
                WriteLogic(DebuggerLevel.Trace, $"  ** {keyName} => {item.ToString()}");
            }
        }
    }
}