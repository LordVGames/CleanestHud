using BepInEx.Logging;

namespace CleanestHud
{
    internal static class Log
    {
        private static ManualLogSource _logSource;

        internal static void Init(ManualLogSource logSource)
        {
            _logSource = logSource;
        }

        internal static void Debug(object data)
        {
            // always do debug logging when compiled for debug, only do it if config allows it when compiled for release
#if !DEBUG
            if (ConfigOptions.EnableDebugLogging.Value)
            {
#endif
                _logSource.LogDebug(data);
#if !DEBUG
            }
#endif
        }
        internal static void Error(object data) => _logSource.LogError(data);
        internal static void Fatal(object data) => _logSource.LogFatal(data);
        internal static void Info(object data) => _logSource.LogInfo(data);
        internal static void Message(object data) => _logSource.LogMessage(data);
        internal static void Warning(object data) => _logSource.LogWarning(data);
    }
}