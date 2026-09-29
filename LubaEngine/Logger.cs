using LubaEngine.Components.ImGUIComponents;
using System;
using System.Collections.Generic;
using System.Text;
using LubaEngine.Types;
namespace LubaEngine
{
    public static class Logger {
        public static List<Log> logs = new();

        public static void Initialize() {
            File.WriteAllText(
                 Path.Combine(AppContext.BaseDirectory, "log.txt")
                 , "");
        }
        public static void Log(Log log)
        {
            Console.WriteLine(log.data);
            string fMessage = "PC: " + EngineCore.programCounter.ToString() + " | " + log.data;
            logs.Add(new Log(fMessage));

            File.AppendAllText(
            Path.Combine(AppContext.BaseDirectory, "log.txt")
            , "\n"+fMessage);
        }

        public static void Log(string message)
        {
            string fMessage = "PC: " + EngineCore.programCounter.ToString() + " | " + message;

            Log log = new Log(fMessage);
            Console.WriteLine(log.data);
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
