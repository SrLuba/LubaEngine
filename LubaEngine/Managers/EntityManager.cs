using ImGuiNET;
using LubaEngine.Components.ImGUIComponents;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using LubaEngine.Types;
using LubaEngine.Components.EntityComponents;
using LubaEngine.Static;

namespace LubaEngine.Managers
{
	
    public class EntityManager : IEngineSystem
	{
        public List<Entity> entities;
        public void FrameUpdate()
        { }
        public void AddEntity(Entity entity) {
            this.entities.Add(entity);
        }

        public void Awake() {
            entities = new List<Entity>();
            Logger.Log($"Entity Manager {this.GetType().FullName} - Loaded");
        }
        public void Start() {
        }

        public void EntityCommand(string[] args) {
            switch (args[0].ToLowerInvariant()) {
                case "spawn":
                    // entity spawn 'name' 'x' 'y'
                    EntitySpawn(args[1],  SBVector2.FromPixels(int.Parse(args[2]), int.Parse(args[3])));
                    EngineCore.GetComponent<ConsoleManager>().Print($"Added Entity {args[1]} x: {int.Parse(args[2])} y: {int.Parse(args[3])}");

                    break;
                case "add":
                    // entity 'name' add

                    switch (args[2].ToLowerInvariant()) {

                        case "spriterenderer":
                            // entity 'name' add spriterenderer 'texId'
                            Entity ent = GetEntityByName(args[1].ToLower());
                            if (ent == null) return;
                            SpriteRenderer spriteRenderer =  new SpriteRenderer(ent, AssetManager.TextureManager.GetTexture(args[3].ToLower()), int.Parse(args[4]), 0, null);
                            ent.AddComponent(spriteRenderer);

                            break;
                    }
                    break;
                case "edit":
                    switch (args[2].ToLowerInvariant())
                    {
                        case "spriterenderer":
                            // entity 'name' edit spriterenderer 
                            Entity ent = GetEntityByName(args[1].ToLower());
                            if (ent == null) return;

                            switch (args[3]) {
                                case "textureid":
                                    string identifier = args[4];
                                    ent.GetComponent<SpriteRenderer>(0).cTex = AssetManager.TextureManager.GetTexture(identifier.ToLower());
                                    break;
                            }

                            break;
                    }
                    break;
            }
        }

        public void EntitySpawn(string name, SBVector2 position) { 
            Entity ent = new Entity(name, position);
            this.AddEntity(ent);
        }


        public Entity GetEntityByName(string name) {
            List<Entity> cEntities = new List<Entity>(this.entities);

            for (int i = 0; i < cEntities.Count; i++) {
                if (cEntities[i].name.ToLowerInvariant() == name.ToLowerInvariant()) return cEntities[i];
            }

            return null;
        }
        bool registered = false;
        public void Tick() {
            if (EngineCore.isReady && !registered)
            {
                EngineCore.GetComponent<ConsoleManager>().Register("entity", "entity command group", 1, args => EntityCommand(args));
                registered = true;
            }
            for (int i = 0; i < this.entities.Count; i++) {
                this.entities[i].Update();
            }
        }

        public void Draw() {
            for (int i = 0; i < this.entities.Count; i++)
            {
                this.entities[i].Draw();
            }
        }

        public void OnGUI()
        {
            if (ImGui.TreeNode($"Entity Hierarchy"))
            {
                EntityManager eManager = this;
                for (int i = 0; i < eManager.entities.Count; i++)
                {

                    if (ImGui.Selectable(eManager.entities[i].name)) { 
                        Inspector.instance.selectedEntity = eManager.entities[i];
                        Inspector.instance.show = true;
                    }

                }
                ImGui.TreePop();
            }
        }

        public void Clear() {
            for (int i = 0; i < entities.Count; i++) {
                entities[i].Destroy();
            }

            entities.Clear();
            Logger.Log($"Entity Manager {this.GetType().FullName} - Clear");

        }
    }


}
