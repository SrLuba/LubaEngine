using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using rlImGui_cs;
using ImGuiNET;
using LubaEngine.Types;
using LubaEngine.Static;
using LubaEngine.Rendering;
namespace LubaEngine.Components.EntityComponents
{
    public class SpriteRenderer : IEntityComponent {
        public bool visible;
        public Entity parent;
        public int cameraId;
        public int layer;
        public Vector2 offset;
        public Rectangle rect;
        public string targetTexture;
        RenderQuery query;
        public Texture2D cTex;

        public string paletteTextureKey;
        public bool useShader;

        public int ogPalette = 0;
        public int tPalette = 0;
        public Types.Material material;

        public bool debug;
        public SpriteRenderer(Entity parent, string textureKey, int layer, int cameraId, Types.Material? material) {
            this.parent = parent;
            this.targetTexture = textureKey;
            this.cTex = AssetManager.TextureManager.GetTexture(textureKey);
            this.rect = new Rectangle(0, 0, cTex.Width, cTex.Height);
            this.cameraId = cameraId;
            this.offset = Vector2.Zero;
            this.layer = layer;
            this.useShader = false;
            this.material = material;
            this.visible = true;
        }

        public SpriteRenderer(Entity parent, Texture2D tex, int layer, int cameraId, Types.Material? material)
        {
            this.parent = parent;
            this.cTex = tex;
            this.rect = new Rectangle(0, 0, cTex.Width, cTex.Height);
            this.cameraId = cameraId;
            this.offset = Vector2.Zero;
            this.layer = layer;
            this.useShader = false;
            this.material = material;
            this.visible = true;
        }
        public void Awake() {
        }
        public void Start() { }
        public void Update() {

         
        }
        public void OnDestroy()
        {

        }
        public void Draw() {
            if (!visible) return;
            this.query = new RenderQuery("", this.rect, parent.position.ToPixelVector() + offset, this.layer);
            this.query.UpdateTexture(cTex, false);

         
            if (this.material != null) { 
                this.query.material = this.material;
                this.material.SetMaterialProperty("originalPalette", this.ogPalette);
                this.material.SetMaterialProperty("targetPalette", this.tPalette);
            }
            EngineCore.GetComponent<RenderingManager>()
                .cameras[cameraId]
                .AddQuery(this.query);

            float centerX = parent.position.PixelX + (rect.Width / 2);
            float centerY = parent.position.PixelY + (rect.Height / 2);

            float bX = parent.position.PixelX + (rect.Width / 2);
            float bY = parent.position.PixelY + (rect.Height);


            // Sprite Renderer Debug
                
            if (StaticCore.twoMonitor && EngineCore.ctx.devMode) {
                EngineCore.GetComponent<RenderingManager>().QuickDrawLine(new Vector2(centerX, centerY),
                    new Vector2(bX, bY), Color.Red,
                    2,
                    1);

                EngineCore.GetComponent<RenderingManager>().QuickDrawBox(
                            new Vector2(parent.position.PixelX, parent.position.PixelY),
                            new Vector2(rect.Width, rect.Height),
                            Color.Violet,
                            2,
                     1
                    );

                EngineCore.GetComponent<RenderingManager>().QuickDrawCircle(
                        new Vector2(centerX, centerY),
                        2f,
                        Color.Magenta,
                     1
                );

                EngineCore.GetComponent<RenderingManager>().QuickText(
                        new Vector2(parent.position.PixelX + rect.Width, parent.position.PixelY),
                        $"pos (x: {parent.position.PixelX:F2}, y: {parent.position.PixelY:F2})",
                        2,
                        Color.White,
                     1
                    );

                EngineCore.GetComponent<RenderingManager>().QuickText(
                        new Vector2(parent.position.PixelX + rect.Width, parent.position.PixelY + (12)),
                        $"subpixel (x: {parent.position.x:F2}, y: {parent.position.y:F2})",
                        2,
                        Color.White,
                     1
                    );
                EngineCore.GetComponent<RenderingManager>().QuickText(
                     new Vector2(parent.position.PixelX + rect.Width, parent.position.PixelY + (24)),
                     $"rect (rectW: {rect.Width}, rectH: {rect.Height})",
                     2,
                     Color.White,
                     1
                 );

                Camera.debugCamera?.AddQuery(this.query);
            }
        }

    
        public void OnGUI() {
            ImGui.Text($"Offset: {offset}");
            ImGui.Text($"Texture: {targetTexture}");


            ImGui.Text($"Rect: {rect.X} {rect.Y} {rect.Width} {rect.Height}");

            if (ImGui.Checkbox("visible", ref visible)) { }
            Vector2 uv0 = new Vector2(
                rect.X / cTex.Width,
                rect.Y / cTex.Height
            );

            Vector2 uv1 = new Vector2(
                (rect.X + rect.Width) / cTex.Width,
                (rect.Y + rect.Height) / cTex.Height
            ); 

            ImGui.PushID("PreviewCrop");
                ImGui.Image((IntPtr)cTex.Id,
                    new Vector2(rect.Width, rect.Height),
                    uv0, uv1);
            ImGui.PopID();


            int ogPaletteId = this.ogPalette;
            ImGui.SetNextItemWidth(100);
            if (ImGui.InputInt("ogPalette", ref ogPaletteId))
            {
                this.ogPalette = ogPaletteId;
            }

            ImGui.SameLine();

            int tPalette = this.tPalette;
            ImGui.SetNextItemWidth(100);
            if (ImGui.InputInt("tPalette", ref tPalette))
            {
                this.tPalette = tPalette;
            }


        }

    }
}
