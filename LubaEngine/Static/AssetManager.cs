using Newtonsoft.Json;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using LubaEngine.Types;
using LubaEngine.Types.Core;

namespace LubaEngine.Static
{
    public static class AssetManager {

        public static void Initialize(EngineProperties properties) { 
            assetPath = 
                (properties.globalFolder) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), properties.name)
                : Path.Combine(AppContext.BaseDirectory, "assets");

			TextureManager.Initialize();
			SpriteManager.Initialize();
            ShaderManager.Initialize();
		}

        public static void Unload() {
            TextureManager.UnloadAll();
            ShaderManager.UnloadAll();
        }

		public static string assetPath = "";
        public static class SpriteManager {
            public static Dictionary<string, SpriteResource> spriteResources = new();

            public static void Initialize()
            {
                spriteResources.Clear();
                string path = Path.Combine(assetPath, "data", "spritepack");
                string[] files = Directory.GetFiles(path, "*.json");
                for(int i = 0; i < files.Length; i++)
                {
                    Load(Path.GetFileNameWithoutExtension(files[i]));
                }

                Console.WriteLine("SpritePack Manager initialized");
            }

            public static void Load(
                string fileName)
            {

                if (spriteResources.ContainsKey(fileName.ToLower()))
                {
                    Console.WriteLine(
                        $"SpritePack '{fileName.ToLower()}' is already loaded."
                    );

                    return;
                }

                string path = Path.Combine(assetPath, "data", "spritepack", fileName+ ".json");
                string file = File.ReadAllText(path);

                SpriteSheetData d = JsonConvert.DeserializeObject<SpriteSheetData>(file);
                InternalLoad(fileName.ToLower(), d);

                Console.WriteLine(
                    $"Loaded SpritePack: {fileName}"
                );
            }

            static void InternalLoad(string key, SpriteSheetData d) {
                List<Rectangle> rectData = new List<Rectangle>();
                for (int i = 0; i < d.frames.Count; i++)
                {
                    rectData.Add(new Rectangle(
                        d.frames[i].frame.x,
                        d.frames[i].frame.y,
                        d.frames[i].frame.w,
                        d.frames[i].frame.h));
                }

                List<string> layers = new List<string>();
                List<LayerData> lData = d.meta.layers;

                for (int i = 0; i < lData.Count; i++)
                {
                    layers.Add(lData[i].name.ToLower());
                }


                SpriteResource spriteResource = new SpriteResource(key, layers, rectData);

                // Load Animations

                for (int i = 0; i < d.meta.frameTags.Count; i++)
                {
                    FrameTag tag = d.meta.frameTags[i];

                    int sFrame = tag.from;
                    int eFrame = tag.to;
                    int frames = eFrame - sFrame + 1;


                    SpriteAnimation ani = new SpriteAnimation();
                    for (int f = 0; f < frames; f++)
                    {
                        string sfx = "";
                        sfx = d.frames[sFrame + f].sfx;

                        int duration = (int)MathF.Max(1, 
                              MathF.Round(d.frames[sFrame + f].duration * 60f / 1000f));
                        ani.frames.Add(new SpriteAnimationFrame(sFrame + f, duration, sfx));
                    }

                    ani.loop = tag.loop;
                    spriteResource.animations.Add(tag.name.ToLower(), ani);
                }
                spriteResources.Add(key, spriteResource);
            }


            public static SpriteResource GetSpriteResource(string key)
            {
                if (!spriteResources.TryGetValue(
                    key,
                    out SpriteResource resource))
                {
                    throw new KeyNotFoundException(
                        $"SpritePack '{key}' was not found."
                    );
                }

                return resource;
            }
        }
        public static class ShaderManager
        {
            private static readonly Dictionary<string, Shader> shaders = new();

            public static void Initialize()
            {
                shaders.Clear();
                Load("palette", "palette.vert", "palette.frag");
                Load("water", "palette.vert", "water.frag");

                Console.WriteLine("Shader Manager initialized");
            }

            public static void Load(
                string key,
                string vertexPath,
                string fragmentPath)
            {

                if (shaders.ContainsKey(key))
                {
                    Console.WriteLine(
                        $"Shader '{key}' is already loaded."
                    );

                    return;
                }

                Shader shader = Raylib.LoadShader(
                    Path.Combine(assetPath, "shader", vertexPath),
                    Path.Combine(assetPath, "shader", fragmentPath)
                );

                shaders.Add(key, shader);

                Console.WriteLine(
                    $"Loaded Shader: {key}"
                );
            }

            public static Shader GetShader(string key)
            {
                if (!shaders.TryGetValue(
                    key,
                    out Shader shader))
                {
                    throw new KeyNotFoundException(
                        $"Shader '{key}' was not found."
                    );
                }

                return shader;
            }

            public static bool HasShader(string key)
            {
                return shaders.ContainsKey(key);
            }

            public static Dictionary<string, Shader> GetShaders()
            {
                return shaders;
            }

            public static void Unload(string key)
            {
                if (!shaders.TryGetValue(
                    key,
                    out Shader shader))
                {
                    return;
                }

                Raylib.UnloadShader(shader);
                shaders.Remove(key);
            }

            public static void UnloadAll()
            {
                foreach (Shader shader in shaders.Values)
                {
                    Raylib.UnloadShader(shader);
                }

                shaders.Clear();
            }
        }

        public static class TextureManager { 
            static Dictionary<string, Texture2D> textures;

            static Dictionary<string, string> fullpaths;
            public static long textureSize;

            public static IReadOnlyDictionary<string, Texture2D> GetTextures() {
                return textures;
            }
            public static bool Contains(string key) {
                return textures.ContainsKey(key);
            }

            public static void Initialize() {
                fullpaths = new Dictionary<string, string>();
                textures = new Dictionary<string, Texture2D>();

                PreloadTextures("");
                PreloadTextures("palette");
                PreloadTextures("tileset");
            }


            public static void PreloadTextures(string subFolder)
            {
                string[] files = Directory.GetFiles(Path.Combine(assetPath, "textures", subFolder), "*.png");
                if (subFolder=="")
                    files = Directory.GetFiles(Path.Combine(assetPath, "textures"), "*.png");

                foreach (string file in files)
                {
                    string key = Path.GetFileNameWithoutExtension(file)
                        .ToLowerInvariant();

                    if (subFolder!="") key = subFolder + "/" + key;

                    if (!textures.TryAdd(key, Raylib.LoadTexture(file)))
                        continue;

                    fullpaths.Add(key, file);
                    Raylib.SetTextureFilter(textures[key], TextureFilter.Point);
                    textureSize += new FileInfo(file).Length;
                    Console.WriteLine(
                        $"LE | Core | Loading Textures {key}");
                }
            }
            public static Texture2D GetTexture(string key) { 
                return textures[key];
            }



            public static void UnloadAll() {
                foreach (KeyValuePair<string,Texture2D> tex in textures) { 
                    Raylib.UnloadTexture(tex.Value);
                }
            }
        }

}
}
