using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LubaEngine.Types
{
    public interface IEntityComponent
    {
        void Awake();
        void Start();
        void Update();
        void Draw();

        void OnGUI();

        void OnDestroy();
    }

    public class Entity
    {
        public string name;
        public Vector2 position;
        public float rotation;
        public List<IEntityComponent> components;
        public bool destroyed = false;

        public Entity(string name, Vector2 position)
        {
            this.name = name;
            this.position = position;
            this.rotation = 0f;
            this.components = new List<IEntityComponent>();
        }

        public IEntityComponent AddComponent(IEntityComponent IEntityComponent)
        {

            this.components.Add(IEntityComponent);

            IEntityComponent.Awake();
            IEntityComponent.Start();

            return IEntityComponent;
        }

        public T GetComponent<T>(int index)
     where T : IEntityComponent
        {
            int found = 0;

            foreach (IEntityComponent IEntityComponent in components)
            {
                if (IEntityComponent is T typedComponent)
                {
                    if (found == index)
                        return typedComponent;

                    found++;
                }
            }

            throw new IndexOutOfRangeException(
                $"IEntityComponent {typeof(T).Name} with index {index} was not found."
            );
        }


        public void Update()
        {
            for (int i = 0; i < this.components.Count; i++)
            {
                this.components[i].Update();
            }
        }
        public void Draw()
        {
            for (int i = 0; i < this.components.Count; i++)
            {
                this.components[i].Draw();
            }
        }

        public void Destroy() {
            this.OnDestroy();
            for (int i = 0; i < this.components.Count; i++)
            {
                this.components[i].OnDestroy();
            }
            this.components.Clear();
            this.destroyed = true;
            Logger.Log($"Entity {this.GetType().FullName} - Destroy");
        }

        public void OnDestroy() { }
    }
}
