using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LubaEngine.Types
{
    public class Log
    {
        public Vector4 color;
        public string data;

        public Log(Vector4 color, string data)
        {
            this.color = color;
            this.data = data;
        }
        public Log(string data)
        {
            this.color = new Vector4(1f, 1f, 1f, 1f);
            this.data = data;
        }
    }
}
