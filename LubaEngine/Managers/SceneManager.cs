using ImGuiNET;
using LubaEngine.Components.ImGUIComponents;
using LubaEngine.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace LubaEngine.Managers
{
    public class SceneManager : IEngineSystem {
        IScene? currentScene;
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
        public void Awake() {

            entityManager = EngineCore.GetComponent<EntityManager>();
            this.currentScene = null; // we make sure current scene is null

            EngineCore.GetComponent<ConsoleManager>().Register("scene", "scene command group", 0, args => SceneCommand(args));

        }
        public void FrameUpdate()
        {
        }
        public void Start() {
        }
        public void SceneCommand(string[] args) {
            ConsoleManager console = EngineCore.GetComponent<ConsoleManager>();
            if (args.Length == 0) {
                string cS = (currentScene == null) ? "Scene Not Loaded" : currentSceneName;

                console.Log($"current scene: {cS}");
                return;
            }
            switch (args[0]) {
                case "list":
                    foreach (string scene in scenes.Keys)
                    {
                        console.Log(scene);
                    }
                    break;
                case "load":
                    if (args.Length < 2) {
                        console.Log($"argument 1 (scene name) not specified");
                        return;
                    }
                    string sceneName = args[1];
                    if (scenes.ContainsKey(sceneName))
                    {
                        LoadScene(sceneName);
                        console.Log($"loading scene {sceneName}");
                    }
                    else {
                        console.Log($"scene {sceneName} not found");
                    }
                    break;
                case "reload":

                    if (currentScene != null) { 
                        LoadScene(currentSceneName);
                    }
                    console.Log(currentScene!=null? $"reloading scene {currentSceneName}" : "there's not a scene to reload (current scene is null)");

                    break;
                default:
                    console.PrintWarning("Usage: ");
                    console.PrintWarning("scene load 'sceneName'");
                    console.PrintWarning("scene list");
                    console.PrintWarning("scene reload");
                    break;
            }
      
        }
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
        public void Tick() {
            currentScene?.Update();
        }

        public void Unload() {
            if (this.currentScene == null) {
                Logger.Log($"-----------------------------------------------------");
                Logger.Log("Scene Manager - Tried to unload scene but this.currentScene was null");
                Logger.Log("Scene Manager - Salvating by skipping the unload process");
                Logger.Log("Scene Manager - if this is the first scene you load, this should be fine. check if otherwise");
                
                return;
            }

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

        }
    }
}
