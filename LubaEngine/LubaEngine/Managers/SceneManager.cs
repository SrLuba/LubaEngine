using ImGuiNET;
using LubaEngine.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace LubaEngine.Managers
{
    public class SceneManager : IEngineSystem {
        IScene currentScene;
        EntityManager entityManager;
        string currentSceneName;
        public Dictionary<string, Func<IScene>> scenes = new();

        public void RegisterScene(string name, Func<IScene> factory)
        {
            scenes[name] = factory;
        }
        public void LoadScene(string name)
        {
            if (!scenes.TryGetValue(name, out Func<IScene> factory))
            {
                Logger.Log($"Scene Manager - Scene '{name}' not found");
                return;
            }
            currentSceneName = name;
            LoadScene(factory());   // factory() crea una instancia nueva
        }

        public string GetCurrentScene() {
            return currentSceneName;
        }
        public void Awake() { entityManager = EngineCore.GetComponent<EntityManager>(); }
        public void Start() { }
        public void Draw() { }
        public void OnGUI() {
            ImGui.Separator();
            ImGui.Text($"Current Scene");
            ImGui.SameLine();
            if (currentScene != null)
            {
                ImGui.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), currentScene.GetType().FullName);
            }
            else { 
                ImGui.TextColored(new System.Numerics.Vector4(1f, 0f, 0f, 1f), "None");
            }
            ImGui.Separator();
        }
        public void Update() {
            currentScene?.Update();
        }

        public void Unload() {
            this.currentScene.OnUnload();
            entityManager.Clear();
        }
        void LoadScene(IScene scene) {
            Logger.Log($"-----------------------------------------------------");
            Logger.Log($"Scene Manager - Start Load {scene.GetType().FullName}");
            if (currentScene != null) {
                Unload();
                Logger.Log($"Scene Manager - Unloaded Scene {currentScene.GetType().FullName}");
                this.currentScene = null;
            }

            this.currentScene = scene;
            this.currentScene.OnLoad();
            Logger.Log($"Scene Manager - Scene {currentScene.GetType().FullName} / OnLoad()");

            this.currentScene.Start();
            Logger.Log($"Scene Manager - Scene {currentScene.GetType().FullName} / Start()");
            Logger.Log($"-----------------------------------------------------");

        }
    }
}
