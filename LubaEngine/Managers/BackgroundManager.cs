using ImGuiNET;
using LubaEngine.Components.EntityComponents;
using LubaEngine.Static;
using LubaEngine.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace LubaEngine.Managers
{
    public class BackgroundManager : IEngineSystem
    {
        public BackgroundData currentBackground;

        public void Awake()
        {
        }

        public void Draw()
        {

        }

        string snpName = "";
        public void OnGUI()
        {
            if (currentBackground != null) {
                ImGui.Text("Current Backgrund");
                for (int l = 0; l < currentBackground.layers.Count; l++) { 
                    BackgroundLayer layer = currentBackground.layers[l];
                    ImGui.Text($"{layer}");
                }
            }

            ImGui.Separator();

            foreach (KeyValuePair<string, BackgroundData> data in AssetManager.BackgroundManager.backgrounds) { 
                ImGui.Text($"{data.Key}");
            }

            ImGui.Text("Snapshot Exporter:");
            ImGui.Separator();

            ImGui.InputText("Snapshot File Name: ", ref snpName, 64);
            if (ImGui.Button("Snapshot")) {
                SnapshotAndSave(snpName);
            }
        }

        public void Start()
        {
            EngineCore.GetComponent<ConsoleManager>().Register("backgroundsnapshot", "background", 1, args => SnapshotAndSave(args[0]));
        }


        public void SnapshotAndSave(string name) {
            List<Entity> entities = new List<Entity>(EngineCore.GetComponent<EntityManager>().entities);
            List<Entity> fentities = new List<Entity>();

            for (int i = 0; i < entities.Count; i++) { 
                Entity ent = entities[i];
                if (!ent.HasComponent<BackgroundRenderer>()) continue;

                fentities.Add(ent);
            }

            TestSave(name,fentities);
        }
        public void TestSave(string name, List<Entity> entities) {
            string path = Path.Combine(LubaEngine.Static.AssetManager.assetPath, "data", "background", name + ".json");
            List<BackgroundLayer>layers = new List<BackgroundLayer>();
            for (int i = 0; i < entities.Count; i++) {
                BackgroundRenderer rend = entities[i].GetComponent<BackgroundRenderer>(0);
                BackgroundLayer layer = new BackgroundLayer(rend.texKey, rend.renderType== BackgroundRenderType.Scanline, rend.initialParallaxMultiplier, rend.parallaxEffect, rend.parallaxMultiplier);

                layer.layer = rend.layer;
                layer.offset = entities[i].position.ToPixelVector();
                layers.Add(layer);
            }
            BackgroundData data = new BackgroundData(layers);
            currentBackground = data;
            string d = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, d);
        }
        public void FrameUpdate()
        {
        }

        public void Tick()
        {
        }
    }
}
