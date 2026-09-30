using ImGuiNET;
using LubaEngine.Components.ImGUIComponents;
using LubaEngine.Rendering;
using LubaEngine.Static;
using LubaEngine.Types;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Dictionary<KeyboardKey, string> binds = new();
        public readonly List<Log> logs = new();

        public bool showIntlog = true;

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
        public void FrameUpdate()
        {
            if (ImGui.GetIO().WantCaptureKeyboard) return;

            Dictionary<KeyboardKey, string> copy = new Dictionary<KeyboardKey, string>(binds);
            foreach(KeyValuePair<KeyboardKey, string> keyValuePair in copy)
            {
                if (Raylib.IsKeyPressed(keyValuePair.Key))
                    LoadCfg(keyValuePair.Value);
            }
        }
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
            Print("> " + line);
            if (!cmd.Execute(args, out string error))
                PrintError(error);

        }
        public void Awake()
        {
            this.showIntlog = false;
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

                        case "renderscaling":
                            int indexx = int.Parse(splits[2]);
                            string mode = splits[3].ToLower();
                            Camera cas = rManager.cameras[indexx];
                            cas.scalingMode = (mode == "stretch") ? CameraScalingMode.Stretch : CameraScalingMode.Normal;
                            break;

                        case "viewport":
                            if (splits.Length == 2) {
                                PrintWarning("usage: ");
                                PrintWarning("set camera viewport position 'id' 'x' 'y'");
                                PrintWarning("set camera viewport size 'id' 'w' 'h'");
                                PrintWarning("set camera viewport scale 'id' 'scale'");
                                PrintWarning("set camera viewport center 'id' 'scale'");
                                return;
                            }
                            string vp = splits[2];

                            switch (vp) {
                                case "position":
                                    if (splits.Length != 6)
                                    {
                                        PrintError("usage: ");
                                        PrintWarning("set camera viewport position 'id' 'x' 'y'");
                                        return;
                                    }
                                    int indx = int.Parse(splits[3]);
                                    int vx1 = int.Parse(splits[4]);
                                    int vy1 = int.Parse(splits[5]);

                                    Camera c = rManager.cameras[indx];
                                    c.isCentered = false;
                                    c.scalingMode = CameraScalingMode.Normal;
                                    c.SetViewport(new Rectangle(vx1, vy1, c.renderTexture.Texture.Width * c.scale, c.renderTexture.Texture.Height*c.scale));
                                    break;
                                case "size":
                                    if (splits.Length != 6)
                                    {
                                        PrintError("usage: ");
                                        PrintWarning("set camera viewport size 'id' 'w' 'h'");
                                        return;
                                    }
                                    int indx2 = int.Parse(splits[3]);
                                    int vx2 = int.Parse(splits[4]);
                                    int vy2 = int.Parse(splits[5]);

                                    Camera c1 = rManager.cameras[indx2];
                                    c1.isCentered = false;
                                    c1.scalingMode = CameraScalingMode.Normal;

                                    c1.SetViewport(new Rectangle(c1.viewport.X, c1.viewport.Y, vx2 * c1.scale, vy2 * c1.scale));

                                    break;
                                case "scale":
                                    if (splits.Length != 5) { 
                                        PrintError("usage: ");
                                        PrintWarning("set camera viewport scale 'id' 'scale'");
                                        return;
                                    }
                                    int indx3 = int.Parse(splits[3]);
                                    int sc = int.Parse(splits[4]);

                                    Camera c2 = rManager.cameras[indx3];
                                    c2.scale = sc;
                                    break;
                                case "center":
                                    if (splits.Length != 4)
                                    {
                                        PrintError("usage: ");
                                        PrintWarning("set camera viewport center 'id'");
                                        return;
                                    }

                                    rManager.cameras[int.Parse(splits[3])].isCentered = 
                                        !rManager.cameras[int.Parse(splits[3])].isCentered;
                                    break;
                                default:
                                    PrintWarning("usage: ");
                                    PrintWarning("set camera viewport position 'id' 'x' 'y'");
                                    PrintWarning("set camera viewport size 'id' 'w' 'h'");
                                    PrintWarning("set camera viewport scale 'id' 'scale'");
                                    PrintWarning("set camera viewport center 'id' 'scale'");
                                    break;
                            }
                            break;

                        default:
                            PrintWarning("usage: ");
                            PrintWarning("set camera renderscale 'id' 'scale'");
                            PrintWarning("set camera renderscaling 'id' 'stretch/none'");
                            PrintWarning("set camera viewport");
                            break;
                    }
                    break;
                default:
                    PrintWarning("usage: ");
                    PrintWarning("set camera");
                    break;
            }
        }
        bool didAutoexec = false;
        public void Start() 
        {
            /**/
            Register("set", "set", 2, args => ExecuteSet(args));
                 
            Register("dev", "devmode set", 1, args => { StaticCore.devVisible = (args[0].ToLowerInvariant()=="1" || args[0].ToLowerInvariant()=="true"); });
            Register("dualmode", "dualmode set", 1, args => {
                  DualMode((args[0].ToLowerInvariant() == "1" || args[0].ToLowerInvariant() == "true"));
            });
            Register("exec", "Executes a .cfg file", 1, args => LoadCfg(args[0]));
            Register("bind", "binds a key to a cfg file", 2, args => Bind(args[0], args[1]));
            Register("cls", "clears the console", 0, args => {
                this.logs.Clear();
            });
            Register("clear", "clears the console", 0, args => {
                this.logs.Clear();
            });
            Register("bindlist", "shows bind list", 0, args => {
                Bindlist();
            });
            Register("unbind", "unbinds key from cfg", 1, args => {
                Unbind(args[0]);
            });

            Register("imguidemo", "toggles imgui demo", 0, args => {
                bool imguidemo = EngineCore.GetComponent<ImGUIManager>().showDemo;
                EngineCore.GetComponent<ImGUIManager>().showDemo = !imguidemo;
            });
            Register("intlog", "enabled or disables internal logs", 1, args => {
                string val = args[0].ToLowerInvariant();
                if (val == "1" || val == "true")
                {
                    if (!showIntlog)
                    {
                        PrintWarning("Internal Logs were already disabled");
                        return;
                    }

                    showIntlog = false;
                    Print("Internal Logs Disabled");
                }
                else {
                    if (showIntlog)
                    {
                        PrintWarning("Internal Logs were already enabled");
                        return;
                    }

                    showIntlog = true;
                    Print("Internal Logs Enabled");
                }
            });


            didAutoexec = false;
        }

        public void Unbind(string key) {
            if (Enum.TryParse<KeyboardKey>(key, true, out var rKey))
            {
                if (binds.ContainsKey(rKey))
                {
                    Print($"Key '{key}' was unbinded");
                    binds.Remove(rKey);
                }
            }
            else
            {
                PrintError($"Exec Unbind Failed, bind '{key}' does not exist or cannot be interpreted");
            }
        }

        public void Bindlist() {
            Dictionary<KeyboardKey, string> copy = new Dictionary<KeyboardKey, string>(binds);
            Print($"<----------------------Bind List----------------------->");

            foreach (KeyValuePair<KeyboardKey, string> keyValuePair in copy)
            {
                Print($"Key '{keyValuePair.Key}' is binded to {keyValuePair.Value}.cfg");
            }
            Print($"<------------------------------------------------------>");

        }
        public void Bind(string key, string fileName) {
            if (Enum.TryParse<KeyboardKey>(key, true, out var rKey))
            {
                string path = Path.Combine(AssetManager.assetPath, "cfg", fileName);

                if (!fileName.EndsWith(".cfg"))
                {
                    path += ".cfg";
                }

                if (!File.Exists(path))
                {
                    PrintError($"Exec Bind Failed, file {path} does not exist");
                    return;
                }

                binds[rKey] = fileName;
                Print($"{key} binded to {fileName} succesfully");
            }
            else {
                PrintError($"Exec Bind Failed, bind '{key}' does not exist or cannot be interpreted");
            }
        }
        public void LoadCfg(string fileName) {
            Print($"Trying to Load a cfg {fileName}");

            string path = Path.Combine(AssetManager.assetPath, "cfg", fileName);

            if (!fileName.EndsWith(".cfg")) {
                path += ".cfg";
            }

            if (!File.Exists(path)) {
                PrintWarning($"Exec Failed, File {path} does not exist");
                return;
            }


            PrintWarning($"Executing {fileName}");
            ExecuteCfg(path);   
        }
        int execDepth = 0;
        public void ExecuteCfg(string iPath)
        {
            if (execDepth >= 8) { PrintError("exec: max depth reached"); return; }
            execDepth++;
            string path = (!iPath.EndsWith(".cfg")) ? iPath + ".cfg" : iPath;

            try
            {
                if (!File.Exists(path))
                {
                    PrintWarning($"Exec Failed, File {path} does not exist");
                    return;
                }

                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {

                    string line = lines[i];
                    if (line == "" || line == "\n" || line.Length == 0) continue; // we check for empty line

                    // We check the starting 2 characters for comment.

                    if (line.Length > 2)
                    {
                        if (line[..2] == "-#") { continue; }
                    }

                    // We check further in case a comment is on the same line as the command
                    int loc = line.IndexOf("-#");

                    if (loc >= 0) 
                        line = line.Substring(0, loc);


                    Execute(line);
                }
            }
            finally {
                execDepth--;
                Print($"'{iPath}' executed succesfully");

            }

        }

        public void DualMode(bool dual) {
            if (!dual) {
                StaticCore.twoMonitor = false;

                Logger.Log($"DUALMODE - Disabling Debug Camera");
                Camera.debugCamera.enabled = false;

                Logger.Log($"DUALMODE - Setting Debug Camera Viewport (0, 0, 1920, 1080)");
                Camera.debugCamera.SetViewport(new Rectangle(0, 0, 1920, 1080));

                Logger.Log($"DUALMODE - Setting Window Resolution ({1360}, {768})");
                Raylib.SetWindowSize(1360, 768);

                Logger.Log($"DUALMODE - Setting Window Position ({0}, {0})");
                Raylib.SetWindowPosition(0, 0);
                return;
            }
            RenderingManager rManager = EngineCore.GetComponent<RenderingManager>();

            StaticCore.twoMonitor = true;
            Camera.main.isCentered = true;


            if (Raylib.IsWindowState(ConfigFlags.BorderlessWindowMode))
            {
                Raylib.ToggleBorderlessWindowed();
            }
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

        public void Draw() 
        { 

        }

        public void OnGUI()
        {
          
        }
        public void Tick()
        {
            if (!didAutoexec && EngineCore.isReady) { 
                ExecuteCfg(Path.Combine(AssetManager.assetPath, "cfg", "autoexec.cfg"));
                didAutoexec = true;
            }
        }

        public void Unload()
        {

        }
    }
}
