using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;

namespace LubaEngine.Types
{
    public abstract class Material
    {
        public Shader shader;

        private readonly Dictionary<string, object> properties = new();

        protected Material(Shader shader)
        {
            this.shader = shader;
        }

        public void SetMaterialProperty(
            string name,
            object value)
        {
            properties[name] = value;
        }

        public bool TryGetProperty(
            string name,
            out object value)
        {
            return properties.TryGetValue(
                name,
                out value);
        }

        public abstract void Apply();
    }

}
