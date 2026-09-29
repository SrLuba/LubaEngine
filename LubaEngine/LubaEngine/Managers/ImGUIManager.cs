using ImGuiNET;
using LubaEngine.Components.ImGUIComponents;
using LubaEngine.Managers;
using LubaEngine.Static;
using Raylib_cs;
using rlImGui_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LubaEngine.Managers
{
    public interface ImGUIUserInterface
    {
        public void Start();
        public void Update();
        public void Draw();
    }

    public class ImGUIManager : IEngineSystem
    {

        public List<ImGUIUserInterface> userInterfaces;


        public ImGUIManager(bool appendDebug) {
            this.userInterfaces = new List<ImGUIUserInterface>();
            if (appendDebug) AppendDebugUI();
        }
        public void Awake()
        {
        }

        public void AddUI(ImGUIUserInterface ui) {
            this.userInterfaces.Add(ui);
            ui.Start();

            Logger.Log($"ImGUIManager - Adding UI {ui.GetType().FullName}");
        }
        public void Start()
        {
         
        }

         void AppendDebugUI() {
            // Add Debug UI
            AddUI(new ConsoleUI());
            AddUI(new Inspector());
            AddUI(new UIHierarchy());
        }


        public void Update()
        {
            for (int i = 0; i < userInterfaces.Count; i++) {
                this.userInterfaces[i].Update();
            }
        }
        private void DrawDockSpace()
        {
            ImGuiWindowFlags windowFlags =
                ImGuiWindowFlags.NoDocking |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoNavFocus |
                ImGuiWindowFlags.NoBackground;



            float yOffset = StaticCore.devVisible ? 24: 0;
            Vector2 dockPosition = (StaticCore.twoMonitor) ? new Vector2(1920, yOffset) : new Vector2(0, yOffset);
            Vector2 dockSize = (StaticCore.twoMonitor) ? new Vector2(1920, Raylib.GetRenderHeight() - yOffset) : new Vector2(Raylib.GetRenderWidth(), Raylib.GetRenderHeight() - yOffset);

            ImGui.SetNextWindowPos(dockPosition);
            ImGui.SetNextWindowSize(dockSize);

            ImGui.Begin(
                "MainDockSpace",
                windowFlags
            );

            uint dockspaceId = ImGui.GetID("MainDockSpace");

            ImGui.DockSpace(
                dockspaceId,
                Vector2.Zero,
                ImGuiDockNodeFlags.PassthruCentralNode
            );

            ImGui.End();
        }

        public void Draw()
        {
            rlImGui.Begin();
            DrawDockSpace();
            for (int i = 0; i < userInterfaces.Count; i++)
            {
                this.userInterfaces[i].Draw();
            }

            rlImGui.End();
        }

        public void OnGUI() { }
    }
}
