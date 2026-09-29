using System;
using System.Collections.Generic;
using System.Text;
using LubaEngine.Rendering;
using LubaEngine.Managers;
using LubaEngine.Types;
using Raylib_cs;

namespace LubaEngine.Types
{
    public class PaletteMaterial : LubaEngine.Types.Material
    {
        private readonly Dictionary<string, int> locations = new();

        public PaletteMaterial(Shader shader)
            : base(shader)
        {
            locations["originalPalette"] =
                Raylib.GetShaderLocation(
                    shader,
                    "originalPalette");

            locations["targetPalette"] =
                Raylib.GetShaderLocation(
                    shader,
                    "targetPalette");

            locations["paletteWidth"] =
                Raylib.GetShaderLocation(
                    shader,
                    "paletteWidth");

            locations["paletteHeight"] =
                Raylib.GetShaderLocation(
                    shader,
                    "paletteHeight");

            locations["texture1"] =
                Raylib.GetShaderLocation(
                    shader,
                    "texture1");
        }

        public override void Apply()
        {
            if (TryGetProperty(
                "originalPalette",
                out object original))
            {
                Raylib.SetShaderValue(
                    shader,
                    locations["originalPalette"],
                    Convert.ToSingle(original),
                    ShaderUniformDataType.Float);
            }

            if (TryGetProperty(
                "targetPalette",
                out object target))
            {
                Raylib.SetShaderValue(
                    shader,
                    locations["targetPalette"],
                    Convert.ToSingle(target),
                    ShaderUniformDataType.Float);
            }

            if (TryGetProperty(
                "paletteWidth",
                out object width))
            {
                Raylib.SetShaderValue(
                    shader,
                    locations["paletteWidth"],
                    Convert.ToSingle(width),
                    ShaderUniformDataType.Float);
            }

            if (TryGetProperty(
                "paletteHeight",
                out object height))
            {
                Raylib.SetShaderValue(
                    shader,
                    locations["paletteHeight"],
                    Convert.ToSingle(height),
                    ShaderUniformDataType.Float);
            }

            if (TryGetProperty(
                "paletteTexture",
                out object texture))
            {
                Raylib.SetShaderValueTexture(
                    shader,
                    locations["texture1"],
                    (Texture2D)texture);
            }
        }

    }




}
