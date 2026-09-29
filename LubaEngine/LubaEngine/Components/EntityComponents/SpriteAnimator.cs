using Raylib_cs;
using ImGuiNET;
using LubaEngine.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using Material = LubaEngine.Types.Material;
using LubaEngine.Types;

namespace LubaEngine.Components.EntityComponents
{
    public class SpriteAnimator : IEntityComponent
    {
        public SpriteResource data;
        public Rectangle rect;

        public SpriteAnimation currentAnimation;
        public List<SpriteRenderer> renderers;

        int timer = 0;
        int frame = 0;

        public SpriteAnimator(
            Entity parent,
            SpriteResource data,
            int layer,
            Material material)
        {
            renderers = new List<SpriteRenderer>();

            int layerOffset = 0;

            foreach (var texture in data.textureData)
            {
                SpriteRenderer renderer =
                    new SpriteRenderer(
                        parent,
                        texture.Value,
                        layer + layerOffset,
                        0,
                        material
                    );

                renderers.Add(renderer);
                parent.AddComponent(renderer);

                layerOffset++;
            }

            this.data = data;
        }

        public void SetVisible(int layer, bool visible) {
            this.renderers[layer].visible = visible;
        }
        private void ApplyFrame()
        {
            if (currentAnimation == null ||
                currentAnimation.frames.Count == 0)
                return;

            SpriteAnimationFrame animFrame =
                currentAnimation.frames[frame];

            rect =
                data.rectData[animFrame.rectId];

            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].rect = rect;
            }
        }

        public void Play(string name)
        {
            currentAnimation =
                data.GetAnimation(name.ToLower());

            frame = 0;
            timer = 0;

            ApplyFrame();
        }

        public void Awake()
        {
        }

        public void Start()
        {
            currentAnimation =
                data.GetAnimation("default");

            frame = 0;
            timer = 0;

            ApplyFrame();
        }
        public void OnDestroy()
        {

        }
        public void Update()
        {
            if (currentAnimation == null ||
                currentAnimation.frames.Count == 0)
                return;

            timer += 1;

            int duration =
                currentAnimation.frames[frame].duration;

            if (timer >= duration)
            {
                timer -= duration;

                frame++;

                if (frame >= currentAnimation.frames.Count)
                {
                    if (currentAnimation.loop)
                    {
                        frame = 0;
                    }
                    else
                    {
                        frame =
                            currentAnimation.frames.Count - 1;
                    }
                }

                ApplyFrame();
            }
        }

        public void Draw()
        {
        }

        public void OnGUI()
        {
            ImGui.Text($"Frame: {frame}");
            ImGui.Text($"Rect: {rect}");
            ImGui.Text($"Timer: {timer}");

            string currentName = currentAnimation != null
     ? data.animations.FirstOrDefault(x => x.Value == currentAnimation).Key
     : "";

            if (ImGui.BeginCombo("Animation", currentName))
            {
                foreach (var pair in data.animations)
                {
                    bool selected = pair.Value == currentAnimation;

                    if (ImGui.Selectable(pair.Key, selected))
                    {
                        Play(pair.Key);
                    }

                    if (selected)
                        ImGui.SetItemDefaultFocus();
                }

                ImGui.EndCombo();
            }
        }
    }


}
