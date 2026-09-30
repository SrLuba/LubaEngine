using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LubaEngine.Types
{
    [System.Serializable]
    public class BackgroundLayer
    {
        public string textureKey;
        public bool scanline;
        public Vector2 baseParallaxAdd;
        public Vector2 baseParallax;
        public Vector2 baseScanlineMultiplier;
        public Vector2 offset;
        public int layer = 0;

        public BackgroundLayer(string key, bool scanline, Vector2 baseParallaxAdd, Vector2 baseParallax, Vector2 baseScanlineMultiplier) {
            this.textureKey = key;
            this.baseParallaxAdd = baseParallaxAdd;
            this.baseParallax = baseParallax;
            this.baseScanlineMultiplier = baseScanlineMultiplier;
            this.scanline = scanline;
            this.offset = new Vector2(0f, 0f);
        }
    }
    [System.Serializable]
    public class BackgroundData {
        public List<BackgroundLayer> layers;
        public BackgroundData(List<BackgroundLayer> layers) { this.layers = layers; }
    }
}
