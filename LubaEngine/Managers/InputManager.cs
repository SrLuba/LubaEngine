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


        public void FrameUpdate()
        {

        }
        public bool GetDown(int id) => (input & (1u << id)) != 0;
        public bool GetPressed(int id) => (input & ~lastInput & (1u << id)) != 0;
        public bool GetReleased(int id) => (~input & lastInput & (1u << id)) != 0;

        
        public void OnGUI()
        {

            ImGui.Text("Raw Data: ");
            ImGui.Text("ci:"+ Utils.Text.ToHexBytesNoSpaces(input));
            ImGui.SameLine();

            ImGui.Text("li:"+ Utils.Text.ToHexBytesNoSpaces(lastInput));
            ImGui.SameLine();

            ImGui.Text("lp: "+Utils.Text.ToHexBytesNoSpaces(lastpressed));
            ImGui.SameLine();

            ImGui.Text("lr: "+Utils.Text.ToHexBytesNoSpaces(lastreleased));
            ImGui.Separator();

            for (int i = 0; i < keys.Count; i++)
                ImGui.Text($"{keys[i]}: {((input & (1u << i)) != 0 ? "X" : "-")}");
        }
        public void Tick()
        {
            if (ImGui.GetIO().WantCaptureKeyboard)
            {
                input = 0;
                lastInput = 0;
                pressed = 0;
                released = 0;
                lastpressed = 0;
                lastreleased = 0;

                return;
            }

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
