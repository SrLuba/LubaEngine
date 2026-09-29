using ImGuiNET;
using LubaEngine.Types;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Text;

namespace LubaEngine.Managers
{
    public class InputManager : IEngineSystem {

        public uint input;
        public uint lastInput;
        public List<KeyboardKey> keys;

        uint pressed;
        uint released;
        uint lastpressed;
        uint lastreleased;
        public InputManager() {
            this.keys = EngineCore.ctx.bindings;
        }
        public void Awake() { 
            
        }
        public void Start() { }
        public void Draw() { }

        public void Poll() {
           
        }


        public void UnlinkedUpdate()
        {

        }
        public bool GetDown(int id) => (input & (1u << id)) != 0;
        public bool GetPressed(int id) => (input & ~lastInput & (1u << id)) != 0;
        public bool GetReleased(int id) => (~input & lastInput & (1u << id)) != 0;

        string ToHexBytes(uint v) =>
    $"{(v >> 24) & 0xFF:X2} {(v >> 16) & 0xFF:X2} {(v >> 8) & 0xFF:X2} {v & 0xFF:X2}";
        public void OnGUI()
        {
            ImGui.Text("Raw Data: ");
            ImGui.Text(ToHexBytes(input));
            ImGui.Text(ToHexBytes(lastInput));
            ImGui.Text(ToHexBytes(lastpressed));
            ImGui.Text(ToHexBytes(lastreleased));
            ImGui.Separator();

            for (int i = 0; i < keys.Count; i++)
                ImGui.Text($"{keys[i]}: {((input & (1u << i)) != 0 ? "X" : "-")}");
        }
        public void Update()
        {
            lastInput = input;
            input = 0;
            for (int i = 0; i < keys.Count; i++)
            {
                if (Raylib.IsKeyDown(keys[i]))
                {
                    input |= 1u << i;
                }
            }

            pressed = input & ~lastInput;
            released = ~input & lastInput;

            if (lastpressed != pressed && pressed!=0)
                lastpressed = pressed;

            if (lastreleased != released && released != 0)
                lastreleased = released;
        }

        public void Unload()
        {
       
        }
    }
}
