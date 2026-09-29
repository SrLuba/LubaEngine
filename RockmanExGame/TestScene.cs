using LubaEngine;
using LubaEngine.Components.EntityComponents;
using LubaEngine.Managers;
using LubaEngine.Rendering;
using LubaEngine.Static;
using LubaEngine.Types;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Raylib_cs;

namespace RockmanExGame
{
    public class TestScene : IScene
    {
        Entity background, background2, map, x;
        Texture2D backgroundTex;
        RenderingManager renderingManager;
        public Camera camera;
        public void OnLoad()
        {
            renderingManager = EngineCore.GetComponent<RenderingManager>(); // Get the render manager
            camera = Camera.main;
            camera.position = new Vector2(0, 0);

            // Palette Shader & Material
            Shader paletteShader = AssetManager.ShaderManager.GetShader("palette");

            Texture2D palette = AssetManager.TextureManager.GetTexture("palette/xplayer");
            PaletteMaterial pMaterial = new PaletteMaterial(paletteShader);

            pMaterial.SetMaterialProperty("paletteTexture", palette);
            pMaterial.SetMaterialProperty("originalPalette", 0);
            pMaterial.SetMaterialProperty("targetPalette", 0);
            pMaterial.SetMaterialProperty(
                "paletteWidth",
                (float)palette.Width
            );

            pMaterial.SetMaterialProperty(
                "paletteHeight",
                (float)palette.Height
            );

            // entity initialization
            background = new Entity("background", new Vector2(0f, 108f));
            background2 = new Entity("background2", new Vector2(0f, -20f));
            map = new Entity("map", new Vector2(0f, 0f));
            x = new Entity("x", new Vector2(0f, 0f));

            // components
            map.AddComponent(new SpriteRenderer(map, "map", 2, 0, null));
            SpriteAnimator xRend = new SpriteAnimator(x, AssetManager.SpriteManager.GetSpriteResource("xplayer"), 4, pMaterial);
            x.AddComponent(xRend);
            xRend.SetVisible(1, false);

            BackgroundRenderer rend = new BackgroundRenderer(background, "background", BackgroundRenderType.Scanline, 1);
            rend.SetParallaxEffectScanline(new Vector2(0.1f, 0f), new Vector2(0.1f, 0f), new Vector2(0.2f, 0f));
            background.AddComponent(rend);

            BackgroundRenderer REN2 = new BackgroundRenderer(background2, "background2", BackgroundRenderType.Image, 0);
            REN2.SetParallaxEffect(new Vector2(0.02f, 0f));
            background2.AddComponent(REN2);

            EngineCore.GetComponent<EntityManager>().AddEntity(background);
            EngineCore.GetComponent<EntityManager>().AddEntity(background2);
            EngineCore.GetComponent<EntityManager>().AddEntity(map);
            EngineCore.GetComponent<EntityManager>().AddEntity(x);
        }
        public void OnUnload() { }
        public void Start()
        {
           
        }
        public void Update()
        {

            InputManager iManager = EngineCore.GetComponent<InputManager>();

            if (iManager.GetDown(1))
                camera.position.X -= 3;

            if (iManager.GetDown(0))
                camera.position.X += 3;

            if (iManager.GetDown(2))
                camera.position.Y -= 3;

            if (iManager.GetDown(3))
                camera.position.Y += 3;

 
            camera.backgroundColor = new Color(
                    49,
                    130,
                    198,
                    255
                );

            map.position.X = 0;
            map.position.Y = 96;

          
        }

    }
}
