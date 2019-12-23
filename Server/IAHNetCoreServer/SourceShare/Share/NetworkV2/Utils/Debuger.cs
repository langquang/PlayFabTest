using System;

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
        private static           ILogger Logger       = null;
        private static readonly object  DebugLogLock = new object();

        private static void WriteLogic(DebuggerLevel logLevel, string str, params object[] args)
        {
            lock (DebugLogLock)
            {
                if (Logger == null)
                {
#if UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
                    UnityEngine.Debug.Log(string.Format(str, args));
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
            WriteLogic(DebuggerLevel.Error, str, args);
        }
    }
}