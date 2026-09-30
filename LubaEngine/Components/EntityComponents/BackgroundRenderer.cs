using ImGuiNET;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using LubaEngine.Types;
using LubaEngine.Static;
using LubaEngine.Rendering;

namespace LubaEngine.Components.EntityComponents
{
    public enum BackgroundRenderType { 
        Image,
        Scanline
    }
    public class BackgroundRenderer : IEntityComponent
    {

        public BackgroundRenderType renderType;
        public Vector2 parallaxEffect;
        Texture2D tex;
        Entity parent;
        public int layer;
        public Vector2 parallaxMultiplier;
        public Vector2 initialParallaxMultiplier;
        public string texKey;
        public BackgroundRenderer(Entity parent, string textureKey, BackgroundRenderType renderType, int layer) {
            tex = AssetManager.TextureManager.GetTexture(textureKey);
            this.renderType = renderType;
            this.parent = parent;
            this.layer = layer;
            this.parallaxEffect = new Vector2(1f, 1f);
            this.parallaxMultiplier = new Vector2(1f, 1f);
            this.initialParallaxMultiplier = Vector2.Zero;
            this.texKey = textureKey;
        }

        public void SetParallaxEffect(Vector2 effect) { 
            this.parallaxEffect = effect;
        }

        public void SetParallaxEffectScanline(Vector2 effect, Vector2 parallaxMultiplier, Vector2 initialParallaxMultiplier)
        {
            this.parallaxEffect = effect;
            this.parallaxMultiplier = parallaxMultiplier;
            this.initialParallaxMultiplier = initialParallaxMultiplier;
        }

        public void Awake() { 
        
        }
        public void Start() { 
        
        }
        public void Update() { 
        
        }
        public void OnDestroy() { 
        
        }
        public void Draw() {
            Camera cam = Camera.main;
            if (this.renderType == BackgroundRenderType.Image)
            {
                float offset =
                 -(cam.position.PixelX * parallaxEffect.X)
                 % tex.Width;

                if (offset > 0)
                    offset -= tex.Width;

                for (
                    float x = offset - tex.Width;
                    x < cam.resolution.X + tex.Width;
                    x += tex.Width
                )
                {
                    float worldX =
                        cam.position.PixelX + x;

                    RenderQuery query = new RenderQuery(
                        "",
                        new Vector2(
                            worldX,
                            parent.position.PixelY
                        ),
                        layer
                    );

                    query.UpdateTexture(tex, true);

                    cam.AddQuery(query);
                    if (StaticCore.twoMonitor && EngineCore.ctx.devMode) { 
                        Camera.debugCamera?.AddQuery(query);
                    }

                }


                }
            else if (this.renderType == BackgroundRenderType.Scanline)
            {
                for (int y = 0; y < tex.Height; y++)
                {
                    float parallax =
                        (float)y /
                        (tex.Height - 1) *
                        parallaxEffect.X + initialParallaxMultiplier.X;

                    float offset =
                        -(cam.position.PixelX * parallax)
                        % tex.Width;

                    if (offset > 0)
                        offset -= tex.Width;

                    float localY =
                        initialParallaxMultiplier.Y +
                        (parallaxEffect.Y * cam.position.PixelY) *
                        (y * parallaxMultiplier.Y) +
                        y;

                    for (
                        float x = offset - tex.Width;
                        x < cam.resolution.X + tex.Width;
                        x += tex.Width
                    )
                    {
                        float worldX =
                            cam.position.PixelX + x;

                        Vector2 position = new Vector2(
                            worldX,
                            parent.position.PixelY + localY
                        );

                        RenderQuery query = new RenderQuery(
                            "",
                            position,
                            layer
                        );

                        query.UpdateTexture(tex, true);

                        query.rect = new Rectangle(
                            0,
                            y,
                            tex.Width,
                            1
                        );

                        cam.AddQuery(query);
                        if (StaticCore.twoMonitor && EngineCore.ctx.devMode)
                        {
                            Camera.debugCamera?.AddQuery(query);
                        }
                    }
                }
            }
        }
        public void OnGUI() {
            Camera cam = Camera.main;
            Vector2 pos = parent.position.ToPixelVector() + new Vector2(parallaxEffect.X * cam.position.PixelX, parallaxEffect.Y * cam.position.PixelY);

            ImGui.Text($"Render Type: {this.renderType.ToString()}");
            ImGui.Text($"Position x: {pos.X} y: {pos.Y}");

            float px = parallaxEffect.X;
            float py = parallaxEffect.Y;

            ImGui.SetNextItemWidth(100);
            if (ImGui.InputFloat("Parallax Fx X", ref px))
            {
                parallaxEffect.X = px;
            }

            ImGui.SameLine();

            ImGui.SetNextItemWidth(100);
            if (ImGui.InputFloat("Parallax Fx Y", ref py))
            {
                parallaxEffect.Y = py;
            }
            ImGui.Separator();

            float pxm = parallaxMultiplier.X;
            float pym = parallaxMultiplier.Y;

            ImGui.SetNextItemWidth(100);
            if (ImGui.InputFloat("Parallax Fx MX", ref pxm))
            {
                parallaxMultiplier.X = pxm;
            }

            ImGui.SameLine();

            ImGui.SetNextItemWidth(100);
            if (ImGui.InputFloat("Parallax Fx MY", ref pym))
            {
                parallaxMultiplier.Y = pym;
            }
            ImGui.Separator();

            float pxms = initialParallaxMultiplier.X;
            float pyms = initialParallaxMultiplier.Y;

            ImGui.SetNextItemWidth(100);
            if (ImGui.InputFloat("Parallax Fx MXS", ref pxms))
            {
                initialParallaxMultiplier.X = pxms;
            }

            ImGui.SameLine();

            ImGui.SetNextItemWidth(100);
            if (ImGui.InputFloat("Parallax Fx MYS", ref pyms))
            {
                initialParallaxMultiplier.Y = pyms;
            }
            ImGui.Separator();


            ImGui.Image((IntPtr)tex.Id, new Vector2(128, 64));
        }
    }
}
