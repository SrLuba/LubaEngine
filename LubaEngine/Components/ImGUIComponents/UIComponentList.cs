using ImGuiNET;
using LubaEngine.Managers;
using LubaEngine.Rendering;
using LubaEngine.Static;
using LubaEngine.Types;
using Raylib_cs;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace LubaEngine.Components.ImGUIComponents
{
    public class UIHierarchy : ImGUIUserInterface
    {
        RenderQuery sQuery = null;
        Camera cam = null;
        public bool profiler = false;
        private List<float> frameTimes = new();

        public void Start() { this.frameTimes = new List<float>(); }
        public void Update() {
            
        }
       
        public void Draw()
        {
            if (!StaticCore.devVisible) return;


            if (ImGui.BeginMainMenuBar()) {

                if (ImGui.BeginMenu("Tools"))
                {
                    if (ImGui.MenuItem("Profiler"))
                    {
                        profiler = !profiler;
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Editor"))
                {
                    if (ImGui.MenuItem("Level Editor"))
                    {

                    }
                    ImGui.EndMenu();
                }
               
                ImGui.EndMainMenuBar();
            }
            ImGui.Begin("Luba Engine");

            if (ImGui.TreeNode("Components"))
            {
                ImGui.Separator();

                foreach (IEngineSystem IEntityComponent in EngineCore.components)
                {
                    DrawComponent(IEntityComponent);
                }

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Textures"))
            {
                ImGui.Separator();
                Double mb = AssetManager.TextureManager.textureSize / (1024.0f * 1024.0f);
                ImGui.Text($"Loaded Textures ({mb:F2}MB)");


                foreach (string key in AssetManager.TextureManager.GetTextures().Keys)
                {
                    if (ImGui.Selectable("- " + key + ".png"))
                    {
                    }
                }
                ImGui.TreePop();

            }

            if (ImGui.TreeNode("Sprite Manager"))
            {
                ImGui.Separator();

                Dictionary<string, SpriteResource> spriteResources = AssetManager.SpriteManager.spriteResources;
                foreach (var pair in spriteResources) {
                    if (ImGui.TreeNode(pair.Key+".spritePack"))
                    {

                        SpriteResource res = pair.Value;
                        foreach (var tex in res.textureData) {
                            ImGui.Text($"layer: {tex.Key}");
                        }
                        ImGui.Separator();

                            foreach (var pair2 in res.animations)
                        {
                            if (ImGui.TreeNode($"Animation [{pair2.Key}]")) {

                                ImGui.Text($"frames: {pair2.Value.frames.Count}");
                                ImGui.Text($"loop: {pair2.Value.loop}");

                                int duration = 0;
                                for (int i = 0; i < pair2.Value.frames.Count; i++)
                                {
                                    ImGui.Text($"rectId: {pair2.Value.frames[i].rectId.ToString()}");
                                    ImGui.Text($"duration: {pair2.Value.frames[i].duration}ms");
                                    ImGui.Separator();

                                    duration += pair2.Value.frames[i].duration;
                                }
                                ImGui.Text($"duration: {duration}");
                                ImGui.TreePop();
                            }

                        }

                        ImGui.TreePop();

                    }
                }


                ImGui.TreePop();

            }
            ImGui.End();

            ImGui.Begin("LE - Rendering Hierarchy");
            List<Camera> cameras = EngineCore.GetComponent<RenderingManager>().cameras;
            for (int i = 0; i < cameras.Count; i++) { 
                Camera cam = cameras[i];
                ImGui.Separator();
                Texture2D tex = cam.renderTexture.Texture;
                ImGui.Image(
                        (IntPtr)tex.Id,
                        new Vector2(tex.Width / 2, tex.Height / 2),
                        new Vector2(0, 1),   // uv0: arriba-izquierda toma el Y de abajo
                        new Vector2(1, 0)    // uv1: abajo-derecha toma el Y de arriba
                    );


                if (ImGui.TreeNode($"Camera ({i})[{cam.resolution.X}x{cam.resolution.Y}]"))
                {
                    ImGui.Separator();
                    this.cam = cam;
                    List<RenderQueryBase> queries = this.cam.queries;

                    ImGui.Separator();
                    ImGui.Text($"x: {this.cam.cam.Target.X}, y: {this.cam.cam.Target.Y}");
                    ImGui.Text($"viewport position ({this.cam.viewport.X}, {this.cam.viewport.Y})");
                    ImGui.Text($"viewport size ({this.cam.viewport.Width},{this.cam.viewport.Height})");
                    ImGui.Text($"viewport scale ({this.cam.scale})");
                    ImGui.Separator();
                    ImGui.Separator();



                    if (ImGui.TreeNode($"Render Queries ({queries.Count})")) { 
                        ImGui.Separator();
                        ImGui.TreePop();
                    }
                    ImGui.TreePop();
                }
                ImGui.Separator();
            }

      
                ImGui.End();

            if (profiler)
            {
                ImGui.Begin("LE - Profiler");


                ImGui.Text($"FPS {EngineCore.fps}");
      
                ImGui.Text($"Update {ProfilerData.updateMS:F2}ms");
                ImGui.Text($"Draw {ProfilerData.drawMS:F2}ms");
                ImGui.Text($"Draw Calls {ProfilerData.drawCalls}");
                ImGui.Text($"Render Queries {ProfilerData.renderqueries}");
                ImGui.Text($"GC Collection Gen0 Count {GC.CollectionCount(0)}");
                ImGui.Text($"GC Collection Gen1 Count {GC.CollectionCount(1)}");
                ImGui.Text($"GC Collection Gen2 Count {GC.CollectionCount(2)}");

                ImGui.End();
            }
        }

        private void DrawComponent(IEngineSystem IEntityComponent)
        {
            string name = IEntityComponent.GetType().Name;

            if (ImGui.TreeNode(name))
            {
                IEntityComponent.OnGUI();
                ImGui.TreePop();
            }
        }
    }
}
