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
        void Update();
        void Draw();
        void OnGUI();
    }

    public static class EngineCore {
        static bool running = false;
        public static List<IEngineSystem> components = new();
        static public int programCounter = 0;
        static int lastProgramCounter = 0;
        static float timer = 0f;
        public static int fps = 0;
        public static EngineProperties ctx;

        public const double Step = 1.0 / 60;
        public const int MaxStepPerFrame = 5;
        static double accumulator = 0;
        static double lastTime;

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
            if (Raylib.IsKeyPressed(KeyboardKey.F11))
            {
                Raylib.ToggleBorderlessWindowed();
            }
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Update();
            }

            timer += Raylib.GetFrameTime();
            if (timer >= 1f) {

                fps = programCounter - lastProgramCounter;
                lastProgramCounter = programCounter;
                timer = 0f;
            }

            Raylib.SetWindowTitle($"{ctx.window.title} - Healthy {fps}FPS");

            programCounter++;
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

        public static void Setup(EngineProperties ctx) {

            accumulator = 0;
            lastTime = Raylib.GetTime();

            Logger.Initialize();
            Logger.Log($"-----------------------------------------------------");
            Logger.Log($"EngineCore - Loading Core");

            EngineCore.ctx = ctx;

            Raylib.InitWindow(
             ctx.window.width,
             ctx.window.height,
             ctx.window.title
            );

            Raylib.SetTargetFPS(ctx.targetFPS);
            Logger.Log($"EngineCore - Loading Asset Manager");

            AssetManager.Initialize(ctx);


            // Adding Required Components
            Logger.Log($"EngineCore - Loading Entity Manager");
            AddComponent(new EntityManager());
            Logger.Log($"EngineCore - Loading Rendering Manager");
            AddComponent(new RenderingManager());
            Logger.Log($"EngineCore - Loading ImGUI Manager");
            AddComponent(new ImGUIManager(ctx.devMode));
            Logger.Log($"EngineCore - Loading Scene Manager");
            AddComponent(new SceneManager());
            Logger.Log($"EngineCore - Loading Input Manager");

            AddComponent(new InputManager());
            Logger.Log($"-----------------------------------------------------");

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
                double now = Raylib.GetTime();
                accumulator += now - lastTime;
                lastTime = now;


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
            }

            Logger.Log($"EngineCore - Unloading Asset Manager");
            AssetManager.Unload();
            Logger.Log($"EngineCore - Shutting down rlImGui");

            rlImGui.Shutdown();
            Logger.Log($"EngineCore - Closing Window");
            Logger.Log($"-----------------------------------------------------");


            Logger.WriteToFile();
            Raylib.CloseWindow();

        }

        public static void SetMaxFPS(int fps) {
            Raylib.SetTargetFPS(fps);
        }
    }
}
