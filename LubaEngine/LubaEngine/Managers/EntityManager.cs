using ImGuiNET;
using LubaEngine.Components.ImGUIComponents;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using LubaEngine.Types;

namespace LubaEngine.Managers
{
	
    public class EntityManager : IEngineSystem
	{
        public List<Entity> entities;

        public void AddEntity(Entity entity) {
            this.entities.Add(entity);
        }

        public void Awake() {
            entities = new List<Entity>();
            Logger.Log($"Entity Manager {this.GetType().FullName} - Loaded");
        }
        public void Start() { 
            
        }
        
        public void Update() {
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
