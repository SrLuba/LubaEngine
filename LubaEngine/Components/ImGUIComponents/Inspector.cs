using ImGuiNET;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using LubaEngine.Managers;
using LubaEngine.Static;
using LubaEngine.Types;
namespace LubaEngine.Components.ImGUIComponents
{
    public class Inspector : ImGUIUserInterface
    {
        public static Inspector instance;
        public Entity selectedEntity;
        public bool show;
        public Inspector()
        {
            instance = this;
        }
        public void Start() {  }
        public void Update()
        {
            if (!StaticCore.devVisible) show = false;
        }

        public void Draw()
        {
            if (!show) return;
            if (selectedEntity == null) return;
            if (selectedEntity.destroyed) return;

            ImGui.Begin($"LE - Inspector");
            ImGui.Text($"GameObject: {selectedEntity.name}");

            ImGui.Text("Position");
            ImGui.SameLine();

            float x = selectedEntity.position.PixelX;
            float y = selectedEntity.position.PixelY;
            float sx = selectedEntity.position.x;
            float sy = selectedEntity.position.y;

            ImGui.Text($"x: {x} y: {y}");
            ImGui.Text($"sx: {sx} sy: {sy}");

            ImGui.Separator();
            ImGui.Text("Rotation");

            float rot = selectedEntity.rotation;

            ImGui.SameLine();
            ImGui.SetNextItemWidth(100);
            if (ImGui.InputFloat("Z", ref rot))
            {
                selectedEntity.rotation = rot;
            }
            ImGui.Separator();
            if (ImGui.TreeNode("Components"))
            {
                for (int i = 0; i < selectedEntity.components.Count; i++)
                {
                    if (ImGui.TreeNode($"{selectedEntity.components[i].GetType().Name} ({i})"))
                    {
                        selectedEntity.components[i].OnGUI();
                        ImGui.TreePop();
                    }
                }
                ImGui.TreePop();
            }

            ImGui.End();
        }
    }
}
