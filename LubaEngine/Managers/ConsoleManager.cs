using ImGuiNET;
using LubaEngine.Components.ImGUIComponents;
using LubaEngine.Rendering;
using LubaEngine.Static;
using LubaEngine.Types;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml;

namespace LubaEngine.Managers
{

    public class ConsoleCommand {
        public string name;
        public string description;
        public int minargs;
        public Action<string[]> exec;

        public bool Execute(string[] args, out string error) {
            error = null;

            if (args.Length < minargs) {
                error = $"Usage: {name} - {description}";
                return false;
            }

            try
            {
                exec(args);
                return true;
            }
            catch (Exception e) {
                error = $"{name}: {e.Message}";
                return false;
            }
        }
    }

    public class ConsoleManager : IEngineSystem
    {

        Dictionary<string, ConsoleCommand> commands = new();
        public readonly List<Log> logs = new();

        public void Register(string name, string description, int minArgs, Action<string[]> exec) {
            name = name.ToLowerInvariant();
            if (commands.ContainsKey(name)) {
                Logger.Log($"Console - Command '{name}' already registered");
                return;
            }

            commands[name] = new ConsoleCommand { 
                name = name, 
                description = description, 
                minargs = minArgs, 
                exec = exec 
            };
        }
        public void Log(Vector4 color, string message)
        {
            logs.Add(new Log(color, message));
        }

        public void Log(string message)
        {
            logs.Add(new Log(message));
        }
        public void UnlinkedUpdate()
        { }
        string[] Tokenize(string line) {
            List<string> tokens = new();
            StringBuilder current = new();
            bool inQuotes = false;

            foreach (char c in line) {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ' ' && !inQuotes)
                {
                    if (current.Length > 0)
                    {
                        tokens.Add(current.ToString());
                        current.Clear();
                    }
                }
                else {
                    current.Append(c);
                }
            }

            if (current.Length > 0) {
                tokens.Add(current.ToString());
            }

            return tokens.ToArray();
        }
        public void PrintWarning(string text)
        {
            Log(new System.Numerics.Vector4(1f, 1f, .1f, 1f), text);
        }
        public void Print(string text) {
            Log(new System.Numerics.Vector4(.9f, .9f, .9f, 1f), text);
        }
        public void PrintError(string text)
        {
           Log(new System.Numerics.Vector4(1f, 0f ,0f ,1f),text);
        }
        public void Execute(string line)
        {
            string[] tokens = Tokenize(line);
            if (tokens.Length == 0) return;

            if (!commands.TryGetValue(tokens[0].ToLowerInvariant(), out var cmd)) {
                PrintWarning($"Unknown Command: {tokens[0]}");
                return;
            }

            string[] args = tokens[1..];
            if (args.Length < cmd.minargs) {
                PrintWarning($"Usage: {cmd.name} - {cmd.description}");
                return;
            }

            if (!cmd.Execute(args, out string error))
                PrintError(error);
        }
        public void Awake()
        {

        }

        void ExecuteSet(string[] splits)
        {
            RenderingManager rManager = EngineCore.GetComponent<RenderingManager>();
            string comm = splits[0];
            switch (comm)
            {
                case "camera":
                    string comm2 = splits[1];
                    switch (comm2)
                    {
                        case "renderresolution":
                            int index = int.Parse(splits[2]);
                            int width = int.Parse(splits[3]);
                            int height = int.Parse(splits[4]);
                            Camera cam = rManager.cameras[index];
                            cam.SetRenderInternalResolution(new Vector2(width, height));
                            Log(new Vector4(1f, 1f, 0f, 1), $"Set resolution of camera {index} to ({width}x{height})");
                            break;
                        case "renderposition":
                            int ind = int.Parse(splits[2]);

                            int x = int.Parse(splits[3]);
                            int y = int.Parse(splits[4]);


                            Camera camer = rManager.cameras[ind];
                            camer.SetOutputPosition(new Vector2(x, y));
                            break;
                        case "viewport":
                            int indx = int.Parse(splits[2]);
                            int vx = int.Parse(splits[3]);
                            int vy = int.Parse(splits[4]);
                            int vw = int.Parse(splits[5]);
                            int vh = int.Parse(splits[6]);
                            Camera c = rManager.cameras[indx];
                            c.isCentered = false;
                            c.scalingMode = CameraScalingMode.Normal;
                            c.SetViewport(new Rectangle(vx, vy, vw, vh));
                            break;
                        case "renderscale":
                            int inde = int.Parse(splits[2]);
                            int scale = int.Parse(splits[3]);

                            Camera came = rManager.cameras[inde];
                            came.scale = scale;


                            break;
                        case "renderscaling":
                            int indexx = int.Parse(splits[2]);
                            string mode = splits[4].ToLower();
                            Camera cas = rManager.cameras[indexx];
                            if (mode == "stretch")
                            {
                                cas.scalingMode = CameraScalingMode.Stretch;
                            }
                            if (mode != "stretch")
                            {
                                cas.scalingMode = CameraScalingMode.Normal;
                            }


                            break;
                    }
                    break;
            }
        }
        public void Start() 
        {
            /**/
            Register("set", "set", 2, args => ExecuteSet(args));
                 
            Register("dev", "devmode enable", 0, args => { StaticCore.devVisible = !StaticCore.devVisible; });
            Register("dualmode", "dualmode enable", 0, args => DualMode());
        }
        public void DualMode() {
            RenderingManager rManager = EngineCore.GetComponent<RenderingManager>();

            StaticCore.twoMonitor = !StaticCore.twoMonitor;
            Camera ca = rManager.cameras[0];
            ca.isCentered = true;


            if (Raylib.IsWindowState(ConfigFlags.BorderlessWindowMode))
            {
                Raylib.ToggleBorderlessWindowed();
            }
            if (StaticCore.twoMonitor)
            {
                Logger.Log("DUALMODE - Activating DualMode");

                Logger.Log($"DUALMODE - Setting Window Resolution ({3840}, {1040})");
                Raylib.SetWindowSize(3840, 1040);
                Logger.Log($"DUALMODE - Setting Window Position ({0}, {0})");

                Raylib.SetWindowPosition(0, 0);


                Logger.Log($"DUALMODE - Enabling Debug Camera");

                // just enabling the camera, should be created on program initialization, even on release.
                Camera.debugCamera.enabled = true;


                Logger.Log($"DUALMODE - Setting Debug Camera Viewport (1920, 0, 1920, 1080)");
                // setting dual mode viewport
                Camera.debugCamera.SetViewport(new Rectangle(1920, 0, 1920, 1080));


            }
            else
            {
                Logger.Log($"DUALMODE - Disabling Debug Camera");

                Camera.debugCamera.enabled = false;
                Logger.Log($"DUALMODE - Setting Debug Camera Viewport (0, 0, 1920, 1080)");
                Camera.debugCamera.SetViewport(new Rectangle(0, 0, 1920, 1080));

                Logger.Log($"DUALMODE - Setting Window Resolution ({1360}, {768})");
                Raylib.SetWindowSize(1360, 768);

                Logger.Log($"DUALMODE - Setting Window Position ({0}, {0})");
                Raylib.SetWindowPosition(0, 0);

            }


        }

        public void Draw() 
        { 

        }

        public void OnGUI()
        {
          
        }
        public void Update()
        {
          
        }

        public void Unload()
        {

        }
    }
}
