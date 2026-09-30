using LubaEngine;
using Raylib_cs;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static unsafe class RaylibLogBridge
{
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void OnRaylibLog(int logLevel, sbyte* text, sbyte* args)
    {
        string message = Logging.GetLogMessage(new IntPtr(text), new IntPtr(args));

        string level = (TraceLogLevel)logLevel switch
        {
            TraceLogLevel.Warning => "WARN",
            TraceLogLevel.Error => "ERROR",
            TraceLogLevel.Fatal => "FATAL",
            _ => "INFO"
        };

        try
        {
            Logger.Log($"raylib {level} | {message}");
        }
        catch (Exception e)
        {
            Logger.stdout?.WriteLine($"raylib log bridge failed: {e.Message}");  
        }
    }
}