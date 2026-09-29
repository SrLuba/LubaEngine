using ImGuiNET;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using LubaEngine.Static;
using LubaEngine.Managers;
using LubaEngine.Types;
using LubaEngine.Rendering;

namespace LubaEngine.Components.ImGUIComponents
{
    public class ConsoleUI : ImGUIUserInterface
    {
        private readonly List<Log> logs = new();

        private bool autoScroll = true;
        private string input = "";

        public bool internalLogEnable = false;
        public bool show = false;

        RenderingManager rManager;
        public void Start()
        {
        }
        bool focus = false;
        public void Update()
        {
            bool consoleKey =
                Raylib.IsKeyPressed(KeyboardKey.Backslash) ||
                Raylib.IsKeyPressed(KeyboardKey.Grave);
            if (consoleKey)
            {
                show = !show;
                Console.WriteLine("Toggled Console");
                if (show) focus = true;
            }

        }
        uint testMask;
 
        public void Draw()
        {
            if (!show) return;
            ImGui.Begin("LE - Console");

            ImGui.BeginChild(
                "ConsoleOutput",
                new Vector2(0, -35),
                ImGuiChildFlags.None,
                ImGuiWindowFlags.HorizontalScrollbar);


            foreach (Log log in logs)
            {
                ImGui.TextColored(log.color, log.data);
                ImGui.Separator();

            }

            if (internalLogEnable) {
                foreach (LubaEngine.Types.Log log in Logger.logs)
                {
                    ImGui.TextColored(log.color, log.data);
                    ImGui.Separator();
                }
            }
            if (autoScroll &&
                ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
            {
                ImGui.SetScrollHereY(1.0f);
            }

            ImGui.EndChild();

            // -----------------------------
            // Input
            // -----------------------------

            ImGui.Separator();
            if (focus)
            {
                ImGui.SetKeyboardFocusHere();
                focus = false;
            }
            ImGui.SetNextItemWidth(-1);
            if (ImGui.InputText(
                "##ConsoleInput",
                ref input,
                1024,
                ImGuiInputTextFlags.EnterReturnsTrue))
            {
                ExecuteCommand(input);
                input = "";
            }

            ImGui.End();
        }

  

        public void Log(Vector4 color, string message)
        {
            logs.Add(new Log(color, message));
        }

     

        public void Log(string message)
        {
            logs.Add(new Log(message));
        }

        private void ExecuteCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return;

            rManager = EngineCore.GetComponent<RenderingManager>();

            Log("> " + command);

            string[] splits = command.ToLower().Split(" ");
            switch (splits[0])
            {
                case "cls":
                    logs.Clear();
                    break;
                case "fscb":
                    Raylib.ToggleBorderlessWindowed();
                    Log(new Vector4(100,100,100,100),"Toggled FullSCreen Borderless");
                    break;
                case "fsc":
                    Raylib.ToggleFullscreen();
                    Log(new Vector4(100, 100, 100, 100), "Toggled FullSCreen Exclusive");

                    break;
                case "help":
                    Log("available commands:");
                    Log("help");
                    Log("clear");
                    Log("fscb");
                    Log("fsc");
                    Log("CFG: ");
                    Log("   cfg_intlog 'enable'");
                    break;

                case "cfg_intlog":
                    Log(new Vector4(100, 100, 100, 100), "Disabled Internal Logs");

                    bool t = splits[1] == "1";
                    this.internalLogEnable = t;

                    break;

                case "set":
                    string comm = splits[1];
                    switch (comm) {
                        case "camera":
                            string comm2 = splits[2];
                            switch (comm2) {
                                case "renderresolution":
                                    int index = int.Parse(splits[3]);
                                    int width = int.Parse(splits[4]);
                                    int height = int.Parse(splits[5]);
                                    Camera cam = rManager.cameras[index];
                                    cam.SetRenderInternalResolution(new Vector2(width, height));
                                    Log(new Vector4(1f, 1f, 0f, 1),$"Set resolution of camera {index} to ({width}x{height})");
                                    break;
                                case "renderposition":
                                    int ind = int.Parse(splits[3]);

                                    int x = int.Parse(splits[4]);
                                    int y = int.Parse(splits[5]);


                                    Camera camer = rManager.cameras[ind];
                                    camer.SetOutputPosition(new Vector2(x, y));
                                    break;
                                case "viewport":
                                        int indx = int.Parse(splits[3]);
                                        int vx = int.Parse(splits[4]);
                                        int vy = int.Parse(splits[5]);
                                        int vw = int.Parse(splits[6]);
                                        int vh = int.Parse(splits[7]);
                                        Camera c = rManager.cameras[indx];
                                        c.isCentered = false;
                                        c.scalingMode = CameraScalingMode.Normal;
                                    c.SetViewport(new Rectangle(vx, vy, vw, vh));
                                    break;
                                case "renderscale":
                                    int inde = int.Parse(splits[3]);
                                    int scale = int.Parse(splits[4]);

                                    Camera came = rManager.cameras[inde];
                                    came.scale = scale;


                                    break;
                                case "renderscaling":
                                    int indexx = int.Parse(splits[3]);
                                    string mode = splits[4].ToLower();
                                    Camera cas = rManager.cameras[indexx];
                                    if (mode == "stretch") {
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

                 
                    break;
                case "dev":
                    StaticCore.devVisible = !StaticCore.devVisible;
                    break;
                case "scene":
                    if (command.Length == 1) {
                        break;
                    }
                    SceneManager sceneManager = EngineCore.GetComponent<SceneManager>();

                    switch (splits[1]) {

                         case "list":
                            Log("avaiable scenes: ");
                            foreach (string key in sceneManager.scenes.Keys)
                            {
                                Log(key);
                            }
                            Log("---");

                            break;
                        case "load":
                            string scene = splits[2];
                            sceneManager.LoadScene(scene);
                            break;
                        case "reload":
                            sceneManager.LoadScene(sceneManager.GetCurrentScene());

                            break;
                    }
                    break;
                case "dualmode":
                    StaticCore.twoMonitor = !StaticCore.twoMonitor;
                    Camera ca = rManager.cameras[0];
                    ca.isCentered = true;


                    if (Raylib.IsWindowState(ConfigFlags.BorderlessWindowMode))
                    {
                        Raylib.ToggleBorderlessWindowed();
                    }
                    if (StaticCore.twoMonitor)
                    {
                        Raylib.SetWindowSize(3840, 1040);
                        Raylib.SetWindowPosition(0, 0);



                        Camera debugCamera = new Camera(false, true, new Vector2(1920, 1080), true);
                        debugCamera.id = rManager.cameras.Count - 1;
                        debugCamera.isCentered = false;
                        debugCamera.SetViewport(new Rectangle(1920, 0, 1920, 1080));
                        rManager.AddCamera(debugCamera);

                    }
                    else
                    {
                        Raylib.SetWindowSize(1280, 720);
                    }


                    break;
                default:
                    Log(new Vector4(1f, 0, 0, 1f), "Unknown command: " + command);
                    break;
            }
        }
    }
}