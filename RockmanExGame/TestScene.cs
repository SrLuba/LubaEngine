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
using BackgroundRenderer = LubaEngine.Components.EntityComponents.BackgroundRenderer;

namespace RockmanExGame
{
    public class TestScene : IScene
    {
        Entity background, background2, background3, map, x;
        Texture2D backgroundTex;
        RenderingManager renderingManager;
        public Camera camera;
        public void OnLoad()
        {
            renderingManager = EngineCore.GetComponent<RenderingManager>(); // Get the render manager
            camera = Camera.main;
            camera.position = new SBVector2(0, 0);

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
            background = new Entity("background", SBVector2.FromPixels(0, 108));
            background2 = new Entity("background2", SBVector2.FromPixels(0, -20));
            background3 = new Entity("background3", SBVector2.FromPixels(0, -20));
            map = new Entity("map", SBVector2.FromPixels(0, 0));
            x = new Entity("x", SBVector2.FromPixels(0, 0));

            // components
            map.AddComponent(new SpriteRenderer(map, "map", 3, 0, null));
            SpriteAnimator xRend = new SpriteAnimator(x, AssetManager.SpriteManager.GetSpriteResource("xplayer"), 4, pMaterial);
            x.AddComponent(xRend);
            xRend.SetVisible(1, false);

            BackgroundRenderer rend = new BackgroundRenderer(background, "background", BackgroundRenderType.Scanline, 1);
            rend.SetParallaxEffectScanline(new Vector2(0.1f, 0f), new Vector2(0.1f, 0f), new Vector2(0.2f, 0f));
            background.AddComponent(rend);

            BackgroundRenderer rend3 = new BackgroundRenderer(background3, "background3", BackgroundRenderType.Image, 2);
            rend3.SetParallaxEffect(new Vector2(0.02f, 0f));
            background3.AddComponent(rend3);

            BackgroundRenderer REN2 = new BackgroundRenderer(background2, "background2", BackgroundRenderType.Image, 0);
            REN2.SetParallaxEffect(new Vector2(0.02f, 0f));
            background2.AddComponent(REN2);

            EngineCore.GetComponent<EntityManager>().AddEntity(background);
            EngineCore.GetComponent<EntityManager>().AddEntity(background2);
            EngineCore.GetComponent<EntityManager>().AddEntity(background3);

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


            float k = 60f / EngineCore.tps;
            int speed = (int)MathF.Round(256f*k);
            if (iManager.GetDown(1))
                camera.position -= new SBVector2(speed, 0);

            if (iManager.GetDown(0))
                camera.position += new SBVector2(speed, 0);

            if (iManager.GetDown(2))
                camera.position -= new SBVector2(0, speed);


            if (iManager.GetDown(3))
                camera.position += new SBVector2(0, speed);



            camera.backgroundColor = new Color(
                    49,
                    130,
                    198,
                    255
                );

          
        }

    }
}
