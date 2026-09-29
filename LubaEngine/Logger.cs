using LubaEngine.Components.ImGUIComponents;
using System;
using System.Collections.Generic;
using System.Text;
using LubaEngine.Types;
namespace LubaEngine
{
    public class LoggerWriter : TextWriter
    {
        readonly TextWriter original;
        readonly StringBuilder line = new();

        public LoggerWriter(TextWriter original) { this.original = original; }

        public override Encoding Encoding => original.Encoding;

        public override void Write(char c)
        {
            if (c == '\n')
            {
                Logger.LogFromConsole(line.ToString().TrimEnd('\r'));
                line.Clear();
            }
            else
            {
                line.Append(c);
            }
        }
    }
    public static class Logger {
        public static List<Log> logs = new();
        public static TextWriter stdout;   // la consola real

        public static void Initialize() {
            File.WriteAllText(
                 Path.Combine(AppContext.BaseDirectory, "log.txt")
                 , "");

            stdout = Console.Out;                 
            Console.SetOut(new LoggerWriter(stdout));
        }
        public static void Log(Log log)
        {
            string fMessage = "PC: " + EngineCore.programCounter.ToString() + " | " + log.data;
            logs.Add(new Log(fMessage));

            File.AppendAllText(
            Path.Combine(AppContext.BaseDirectory, "log.txt")
            , "\n"+fMessage);
        }
        public static void LogFromConsole(string message)
        {
            string fMessage = "PC: " + EngineCore.programCounter + " | " + message;

            stdout.WriteLine(fMessage);     // consola real, NO Console.WriteLine
            logs.Add(new Log(fMessage));

            File.AppendAllText(
                Path.Combine(AppContext.BaseDirectory, "log.txt"),
                "\n" + fMessage);
        }
        public static void Log(string message)
        {
            string fMessage = "PC: " + EngineCore.programCounter.ToString() + " | " + message;

            Log log = new Log(fMessage);
            logs.Add(log);
            
            File.AppendAllText(
            Path.Combine(AppContext.BaseDirectory, "log.txt")
            , "\n" + log.data);
        }

        public static void WriteToFile()
        {
            List<string> log = new List<string>();
            for (int i = 0; i < logs.Count; i++)
            {
                log.Add(logs[i].data);
            }

            string[] logData = log.ToArray();
            File.WriteAllLines(
                Path.Combine(AppContext.BaseDirectory, "log.txt")
                , logData);

        }
    }
}
