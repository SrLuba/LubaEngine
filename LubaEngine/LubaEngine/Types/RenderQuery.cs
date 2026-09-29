using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Raylib_cs;
using LubaEngine.Static;
namespace LubaEngine.Types
{

    public abstract class RenderQueryBase {
        public int layer = 0;
        public virtual void Render() { }
    }

    public class TextRenderQuery : RenderQueryBase
    {
        public Vector2 pos;
        public string text;
        public Color color;
        public int size;

        public override void Render()
        {
            Raylib.DrawText(text, (int)pos.X, (int)pos.Y, size, color);
        }
    }
    public class CircleRenderQuery : RenderQueryBase
    {
        public Vector2 pos;
        public float radius;
        public Color color;

        public override void Render()
        {
            Raylib.DrawCircle((int)pos.X, (int)pos.Y, radius, color);
        }
    }
    public class LineRenderQuery : RenderQueryBase {
        public Vector2 a, b;
        public Color color;
        public float size;

        public override void Render() {
            Raylib.DrawLineEx(a,b,size, color);
        }
    }
    public class RenderQuery : RenderQueryBase
    {
        public Texture2D tex;
        public Rectangle rect;
        public Vector2 position;
        public Vector2 scale;
        public float rotation;
        public Color tint;

        public string textureKey;
        public Material? material;

        public TextureWrap wrapMode;

        public RenderQuery(
            string key,
            Rectangle rect,
            Vector2 position,
            int layer)
        {
            if (key != "")
                this.tex =
                    AssetManager.TextureManager.GetTexture(key);

            this.rect = rect;
            this.position = position;
            this.scale = Vector2.One;
            this.rotation = 0f;
            this.tint = Color.White;
            this.layer = layer;
            this.textureKey = key;
            this.wrapMode = TextureWrap.Clamp;
        }

        public RenderQuery(
            string key,
            Vector2 position,
            int layer)
        {
            if (key != "")
                this.tex =
                    AssetManager.TextureManager.GetTexture(key);

            this.rect = key != ""
                ? new Rectangle(
                    0,
                    0,
                    tex.Width,
                    tex.Height)
                : new Rectangle();

            this.position = position;
            this.scale = Vector2.One;
            this.rotation = 0f;
            this.tint = Color.White;
            this.layer = layer;
            this.textureKey = key;
            this.wrapMode = TextureWrap.Clamp;
        }

        public void UpdateTexture(
            Texture2D tex,
            bool changeRect)
        {
            this.tex = tex;

            if (changeRect)
            {
                this.rect = new Rectangle(
                    0,
                    0,
                    tex.Width,
                    tex.Height
                );
            }
        }


        public void RenderRaw()
        {
            Raylib.SetTextureWrap(
                this.tex,
                wrapMode
            );

            position.X = MathF.Floor(position.X);
            position.Y = MathF.Floor(position.Y);
            Raylib.DrawTexturePro(
                tex,
                rect,
                new Rectangle(
                    position.X,
                    position.Y,
                    rect.Width * scale.X,
                    rect.Height * scale.Y
                ),
                Vector2.Zero,
                rotation,
                tint
            );

            ProfilerData.renderqueries++;
        }
        public override void Render()
        {
            if (material != null)
            {
                Raylib.BeginShaderMode(
                material.shader);

                material.Apply();
            }

            RenderRaw();

            if (material != null)
            {
                Raylib.EndShaderMode();
            }
        }

    }
}
