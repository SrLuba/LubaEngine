using ImGuiNET;
using LubaEngine.Managers;
using LubaEngine.Rendering;
using LubaEngine.Static;
using LubaEngine.Types;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;

namespace LubaEngine.Components.ImGUIComponents
{
    public class ConsoleUI : ImGUIUserInterface
    {

        private bool autoScroll = true;
        private string input = "";

        public bool internalLogEnable = false;
        public bool show = false;

        ConsoleManager consoleManager;
        bool focus = false;
        string checksum;
        public void Start()
        {
            consoleManager = EngineCore.GetComponent<ConsoleManager>();
            string exePath = Environment.ProcessPath!;
            byte[] hash = SHA256.HashData(File.ReadAllBytes(exePath));
            checksum = Convert.ToHexString(hash);
        }

        public void Update() {
            bool consoleKey =
               Raylib.IsKeyPressed(KeyboardKey.Backslash) ||
               Raylib.IsKeyPressed(KeyboardKey.Grave);
            if (consoleKey) Toggle();
        }

        public void Toggle() {
            show = !show;
            Console.WriteLine("Toggled Console");
            if (show) focus = true;
        }
        uint testMask;
 
        public void Draw()
        {
            if (!show) return;
            ImGui.Begin("LE - Console");


            // Top Bar
            ImGui.TextColored(new Vector4(.9f, .9f, 0f, 0.9f), $"Luba Engine v0.0.0.1a - Loaded: {EngineCore.ctx.name}");

            ImGui.SameLine();
            string text = $"{checksum}"; // Checksum
            float width = ImGui.CalcTextSize(text).X;
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X - width);
            ImGui.TextColored(new Vector4(.9f, .9f, 0f, 0.9f), text);

            ImGui.TextColored(new Vector4(.9f, .9f, 0f, 0.9f), $"FPS: {EngineCore.fps}");
            ImGui.SameLine();

            ImGui.TextColored(new Vector4(.9f, .9f, 0f, 0.9f), $"| PC: {EngineCore.programCounter}");
            ImGui.Separator();

            // Console Outputs
            ImGui.BeginChild(
                "ConsoleOutput",
                new Vector2(0, -35),
                ImGuiChildFlags.None);

            // Log Display
            if (consoleManager.showIntlog) { 
                foreach (LubaEngine.Types.Log log in Logger.logs)
                {
                    ImGui.TextColored(new Vector4(.6f, .6f, .6f, 6f), log.data);
                }
            }
            foreach (Log log in consoleManager.logs)
            {
                ImGui.TextColored(log.color, log.data);
            }

            if (autoScroll &&
                ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
            {
                ImGui.SetScrollHereY(1.0f);
            }

            ImGui.EndChild();
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
                EngineCore.GetComponent<ConsoleManager>().Execute(input);
                input = "";
            }
          

            ImGui.End();
        }
    }
}