using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;
using LubaEngine.Static;
namespace LubaEngine.Types
{
    [Serializable]
    public class FrameData
    {
        public FrameRect frame;
        public int duration;
        public string sfx;
    }
    [Serializable]
    public class FrameRect
    {
        public int x;
        public int y;
        public int w;
        public int h;
    }
    [Serializable]
    public class FrameSize
    {
        public int w;
        public int h;
    }
    [Serializable]
    public class SpriteSheetData
    {
        public List<FrameData> frames;
        public MetaData meta;
    }
    [Serializable]
    public class MetaData
    {
        public FrameSize size;
        public string scale;
        public List<FrameTag> frameTags;
        public List<LayerData> layers;
    }
    [Serializable]
    public class FrameTag
    {
        public string name;
        public int from;
        public int to;
        public string direction;
        public bool loop;

    }
    [Serializable]
    public class LayerData
    {
        public string name;
        public int opacity;
        public string blendMode;
    }
    public class SpriteRectData
    {
        public Rectangle rect;
        public SpriteRectData(Rectangle rect) { this.rect = rect; }
    }


    [Serializable]
    public class SpriteAnimationFrame
    {
        public int rectId;
        public int duration;
        public string sfx;
        public SpriteAnimationFrame(int rectId, int duration, string sfx)
        {
            this.rectId = rectId;
            this.duration = duration;
            this.sfx = sfx;
        }
    }

    [Serializable]
    public class SpriteAnimation
    {
        public List<SpriteAnimationFrame> frames;
        public bool loop;

        public SpriteAnimation()
        {
            frames = new List<SpriteAnimationFrame>();
            this.loop = false;
        }
    }

    [Serializable]
    public class SpriteResource
    {
        public string key;
        public List<Rectangle> rectData;
        public Dictionary<string, Texture2D> textureData;
        public Dictionary<string, SpriteAnimation> animations;

        public SpriteResource(string key, List<string> textureData, List<Rectangle> rectData)
        {
            this.key = key;
            this.animations = new Dictionary<string, SpriteAnimation>();
            this.rectData = rectData;

            this.textureData = new Dictionary<string, Texture2D>();
            for (int i = 0; i < textureData.Count; i++)
            {
                Texture2D tex = AssetManager.TextureManager.GetTexture(textureData[i]);
                this.textureData.Add(textureData[i], tex);
            }
        }

        public SpriteAnimation GetAnimation(string animation)
        {
            return this.animations[animation];
        }

    }
}
