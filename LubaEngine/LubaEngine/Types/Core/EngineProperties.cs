using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Text;

namespace LubaEngine.Types.Core
{
    public class EngineProperties
    {
        public string name;
        public bool devMode = false;
        public int targetFPS = 60;
        public WindowProperties window;
        public bool globalFolder = false;
        public List<KeyboardKey> bindings = new List<KeyboardKey>(); 
        public EngineProperties(string name, bool devMode, bool globalFolder, WindowProperties window, List<KeyboardKey> bindings) {
            this.name = name;
            this.targetFPS = 60;
            this.window = window;
            this.globalFolder = globalFolder;
            this.devMode = devMode;
            this.bindings = bindings;
        }
    }
}
