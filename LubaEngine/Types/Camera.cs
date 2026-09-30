using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Raylib_cs;
using LubaEngine.Static;

namespace LubaEngine.Types
{
    public enum CameraScalingMode
    {
        Normal,
        Stretch
    }
    public class Camera
    {
        public bool enabled = true;
        public int id;
        public static Camera main;
        public static Camera debugCamera;
        public Camera2D cam;
        public RenderTexture2D renderTexture;
        public SBVector2 position;
        public Vector2 resolution;
        public Color backgroundColor;
        public int scale = 1;

        public List<RenderQueryBase> queries = new();
        public CameraScalingMode scalingMode;
        public Rectangle viewport;

        public float zoom;
        int screenWidth;
        public bool isCentered;
     
        public Camera(bool isMain, bool isDebug, Vector2 resolution, bool isCentered)
        {
            if (isMain) main = this;
            if (isDebug) debugCamera = this;

            this.resolution = resolution;

            this.renderTexture = Raylib.LoadRenderTexture(
                (int)resolution.X, (int)resolution.Y);

            this.backgroundColor = Color.Black;

            this.zoom = 1f;
            this.cam = new Camera2D
            {
                Target = position.ToPixelVector(),
                Offset = resolution / 2f,
                Rotation = 0f,
                Zoom = 1f
            };

            this.isCentered = isCentered;
            this.queries.Clear();

            this.screenWidth = Raylib.GetRenderWidth();

            int width = (int)resolution.X * scale;
            int height = (int)resolution.Y * scale;
            int x = (int)((screenWidth - width)/2);
            int y = (int)((Raylib.GetRenderHeight() - height) / 2f);

            this.viewport = new Rectangle(x, y, width, height);
            this.enabled = true;
        }

        public void SetOutputPosition(Vector2 position)
        {
            this.viewport.X = position.X;
            this.viewport.Y = position.Y;
        }
        public void SetOutputSize(Vector2 size)
        {
            this.viewport.Width = size.X;
            this.viewport.Height = size.Y;
        }

        public void SetViewport(Rectangle viewport)
        {
            this.viewport = viewport;
        }
        public void SetRenderInternalResolution(Vector2 resolution)
        {
            this.resolution = resolution;
            Raylib.UnloadRenderTexture(this.renderTexture);
            this.renderTexture = Raylib.LoadRenderTexture(
                (int)resolution.X, (int)resolution.Y);

            this.cam.Offset = resolution / 2f;
        }

        public void InternalRender()
        {
            if (!this.enabled) {
                queries.Clear();
                return;
            }

            
            queries.Sort((a, b) => a.layer.CompareTo(b.layer));

            this.cam.Target = position.ToPixelVector();
            this.cam.Zoom = this.zoom;
            this.screenWidth = StaticCore.twoMonitor ? Raylib.GetRenderWidth() / 2 : Raylib.GetRenderWidth();

            Raylib.BeginTextureMode(this.renderTexture);
            Raylib.ClearBackground(this.backgroundColor);

            Raylib.BeginMode2D(cam);
            foreach (RenderQueryBase query in queries)
            {
                query.Render();
            }
            Raylib.EndMode2D();
            Raylib.EndTextureMode();

            queries.Clear();
        }

        public void DrawToScreen()
        {
            if (!this.enabled) return;
            int width = (int)resolution.X * scale;
            int height = (int)resolution.Y * scale;


            if (isCentered)
            {
                this.viewport.X = (screenWidth - width) / 2f;
                this.viewport.Y = (Raylib.GetRenderHeight() - height) / 2f;
                this.viewport.Width = width;
                this.viewport.Height = height;
            }

            if (scalingMode == CameraScalingMode.Stretch)
            {
                this.viewport.X = 0;
                this.viewport.Y = 0;
                this.viewport.Width = Raylib.GetScreenWidth();
                this.viewport.Height = Raylib.GetScreenHeight();
            }


            Raylib.DrawTexturePro(
                renderTexture.Texture,
                new Rectangle(
                        0,
                        0,
                        resolution.X,
                        -resolution.Y
                    ),
                viewport,
                Vector2.Zero,
                0f,
                Color.White);
        }


        public void AddQuery(RenderQueryBase renderQuery)
        {
            this.queries.Add(renderQuery);
        }
    }
}
