using System;
using System.Collections.Generic;
using System.Text;

namespace LubaEngine.Types.Core
{
    public class WindowProperties
    {
        public int width;
        public int height;
        public string title;
        public WindowProperties(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.title = "Luba Engine";
        }

        public WindowProperties()
        {
            this.width = 1360;
            this.height = 768;
            this.title = "Luba Engine";
        }
    }
}
