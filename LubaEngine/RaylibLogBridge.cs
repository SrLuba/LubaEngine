using LubaEngine;
using Raylib_cs;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static unsafe class RaylibLogBridge
{
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void OnRaylibLog(int logLevel, sbyte* text, sbyte* args)
    {
        // raylib manda el formato printf + los argumentos; Raylib-cs trae el helper que los combina
        string message = Logging.GetLogMessage(new IntPtr(text), new IntPtr(args));

        string level = (TraceLogLevel)logLevel switch
        {
            TraceLogLevel.Warning => "WARN",
            TraceLogLevel.Error => "ERROR",
            TraceLogLevel.Fatal => "FATAL",
            _ => "INFO"
        };

        Logger.Log($"raylib {level} | {message}");
    }
}