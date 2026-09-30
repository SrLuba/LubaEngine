using ImGuiNET;
using LubaEngine.Managers;
using LubaEngine.Types;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LubaEngine.Components.ImGUIComponents
{
    public class ViewportToolbar : ImGUIUserInterface
    {
        public void Start() { }
        public void Update() { }

        public void Draw()
        {
            Camera dbg = Camera.debugCamera;
            Camera main = Camera.main;
            if (dbg == null || !dbg.enabled) return;   // ← sin la inversión

            Vector2 boxWorld = new Vector2(
                main.position.PixelX - main.renderTexture.Texture.Width / 2,
                main.position.PixelY - main.renderTexture.Texture.Height / 2);

            Vector2 inTexture = Raylib.GetWorldToScreen2D(boxWorld, dbg.cam);

            Rectangle vp = dbg.viewport;
            Vector2 onScreen = new Vector2(
                vp.X + inTexture.X * (vp.Width / dbg.resolution.X),
                vp.Y + inTexture.Y * (vp.Height / dbg.resolution.Y));

            // posición e inmediatamente después, SU ventana
            ImGui.SetNextWindowPos(onScreen, ImGuiCond.Always, new Vector2(0f, 1f));
            ImGui.SetNextWindowBgAlpha(0.35f);

            if (ImGui.Begin("##viewportToolbar",
                ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize |
                ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoFocusOnAppearing |
                ImGuiWindowFlags.NoNav))
            {
                ImGui.Text("Main Viewport");
                ImGui.Text($"sx: {Camera.main.position.x} sy: {Camera.main.position.y}");
                ImGui.Text($"x: {Camera.main.position.PixelX} y: {Camera.main.position.PixelY}");
                if (ImGui.Button("Grid")) { /* toggle */ }
                ImGui.SameLine();
                if (ImGui.Button("Reset")) { /* reset zoom / posición */ }
            }
            ImGui.End();   // End siempre, aunque Begin devuelva false
        }
    }
}
