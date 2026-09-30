using ImGuiNET;
using LubaEngine.Components.ImGUIComponents;
using LubaEngine.Managers;
using LubaEngine.Rendering;
using LubaEngine.Static;
using Newtonsoft.Json.Linq;
using Raylib_cs;
using rlImGui_cs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Text;
using LubaEngine.Types.Core;
namespace LubaEngine
{
    public static class ProfilerData {
        public static double drawMS;
        public static double updateMS;
        public static int drawCalls;
        public static int renderqueries;

    }

    public interface IEngineSystem {
        void Awake();
        void Start();
        void Tick();
        void FrameUpdate();
        void Draw();
        void OnGUI();
    }

    public static class EngineCore {
        static bool running = false;
        public static List<IEngineSystem> components = new();
        static public int programCounter = 0;
        static int lastProgramCounter = 0;
        static float timer = 0f;
        public static int fps = 60;
        public static int tps = 60;
        public static EngineProperties ctx;

        public static double Step = 0;
        public const int MaxStepPerFrame = 5;
        static double accumulator = 0;
        static double lastTime;
        public static bool isReady = false;
        public static void AddComponent(IEngineSystem IEntityComponent) {
            Console.WriteLine($"Adding IEntityComponent {IEntityComponent.GetType().FullName}");
            components.Add(IEntityComponent);
            IEntityComponent.Awake();
            IEntityComponent.Start();
        }

        public static T GetComponent<T>() where T : class {
            return
                components.OfType<T>().FirstOrDefault();
        }

        public static void Update() {
      
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Tick();
            }


            Raylib.SetWindowTitle($"{ctx.window.title} - Healthy {fps}FPS");

        }

        public static void Draw() {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(
              Color.Black
            );

            for (int i = 0; i < components.Count; i++)
            {
                components[i].Draw();
                ProfilerData.drawCalls++;
            }

            Raylib.EndDrawing();
        }

        public static ConsoleManager cManager;
        public static void Setup(EngineProperties ctx) {
            isReady = false;
            accumulator = 0;
            lastTime = Raylib.GetTime();
            Logger.Initialize();
            Logger.Log($"-----------------------------------------------------");
            Logger.Log($"EngineCore - Loading Core");

            EngineCore.ctx = ctx;
            unsafe { Raylib.SetTraceLogCallback(&RaylibLogBridge.OnRaylibLog); }

            Raylib.InitWindow(
             ctx.window.width,
             ctx.window.height,
             ctx.window.title
            );

            SetFPS(ctx.targetFPS);
            SetTPS(ctx.targetFPS);

            Logger.Log($"EngineCore - Loading Asset Manager");

            AssetManager.Initialize(ctx);
            

            // Adding Required Components
            Logger.Log($"EngineCore - Loading Entity Manager");
            AddComponent(new EntityManager());
            Logger.Log($"EngineCore - Loading Console Manager");
            cManager = new ConsoleManager();
            AddComponent(cManager);
            Logger.Log($"EngineCore - Loading Rendering Manager");
            AddComponent(new RenderingManager());
            Logger.Log($"EngineCore - Loading ImGUI Manager");
            AddComponent(new ImGUIManager(ctx.devMode));
            Logger.Log($"EngineCore - Loading Scene Manager");
            AddComponent(new SceneManager());
            Logger.Log($"EngineCore - Loading Input Manager");
            AddComponent(new InputManager());
            Logger.Log($"EngineCore - Loading Background Manager");
            AddComponent(new BackgroundManager());

            cManager.Register("setfps", "sets max fps", 1, args => { SetFPS(int.Parse(args[0])); });
            cManager.Register("settps", "sets max ticks per second", 1, args => { SetTPS(int.Parse(args[0])); });
        }
        public static void Run() {
            Logger.Log($"-----------------------------------------------------");
            Logger.Log($"EngineCore - Starting");

            running = true;
            ProfilerData.drawMS = 0;
            ProfilerData.updateMS = 0;
            ProfilerData.renderqueries = 0;
            ProfilerData.drawCalls = 0;

            Logger.Log($"EngineCore - Loading rlImGui");
            rlImGui.Setup(true);
            ImGuiIOPtr io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            Logger.Log($"EngineCore - Starting Engine Loop");


            while (running && !Raylib.WindowShouldClose())
            {
                isReady = true;
                Step = 1.0 / tps;

                if (Raylib.IsKeyPressed(KeyboardKey.F11))
                {
                    Raylib.ToggleBorderlessWindowed();
                }
                for (int i = 0; i < components.Count; i++)
                {
                    components[i].FrameUpdate();
                }

                if (timer >= 1f)
                {

                    fps = programCounter - lastProgramCounter;
                    lastProgramCounter = programCounter;
                    timer = 0f;
                }

                double now = Raylib.GetTime();
                accumulator += now - lastTime;
                lastTime = now;
                timer += Raylib.GetFrameTime(); 
           

                int steps = 0;

                while (accumulator >= Step && steps < MaxStepPerFrame) {

                    Stopwatch updateWatch = Stopwatch.StartNew();
                            Update();
                    updateWatch.Stop();
                    ProfilerData.updateMS = updateWatch.Elapsed.TotalMilliseconds;

                    accumulator -= Step;
                    steps++;
                }

                if (steps == MaxStepPerFrame)
                    accumulator = 0;

                Stopwatch drawWatch = Stopwatch.StartNew();

                Draw();
                drawWatch.Stop();

                ProfilerData.drawMS = drawWatch.Elapsed.TotalMilliseconds;
                ProfilerData.drawCalls = 0;
                ProfilerData.renderqueries = 0;
                programCounter++;

            }

            Logger.Log($"EngineCore - Unloading Asset Manager");
            AssetManager.Unload();
            Logger.Log($"EngineCore - Shutting down rlImGui");

            rlImGui.Shutdown();
            Logger.Log($"EngineCore - Closing Window");


            Logger.WriteToFile();
            Raylib.CloseWindow();

        }
        public static void SetTPS(int tTps)
        {
            if (tps <= 0) {
                cManager.PrintError("value should be greater than 0 (v>=0)");
                return;
            }
            tps = tTps;
        }
        public static void SetFPS(int fps) {
            if (fps <= 0)
            {
                cManager.PrintError("value should be greater than 0 (v>=0)");
                return;
            }
            Raylib.SetTargetFPS(fps);
        }
    }
}
