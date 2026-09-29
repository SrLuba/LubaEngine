using Raylib_cs;
using LubaEngine.Components;
using LubaEngine.Components.ImGUIComponents;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using LubaEngine.Types;


namespace LubaEngine.Rendering
{

  
    

    public class RenderingManager : IEngineSystem
    {
        public List<Camera> cameras;

        public void Awake()
        {
            this.cameras = new List<Camera>();

            Camera baseCamera = new Camera(true, false, new Vector2(384, 216), true);
            AddCamera(baseCamera); // base camera
            
            // debug camera is set on the first screen by default, but should be changed whenever dualmode is active
            Camera debugCamera = new Camera(false, true, new Vector2(1920, 1080), true);
            debugCamera.enabled = false;
            debugCamera.isCentered = false;
            debugCamera.SetViewport(new Rectangle(0, 0, 1920, 1080));
            AddCamera(debugCamera);
        }

        public void UnlinkedUpdate()
        {

        }
        public void Start()
        {

        }
        public void QuickText(Vector2 pos, string text, int size, Color color, int cameraId)
        {
            TextRenderQuery rq = new TextRenderQuery();

            rq.text = text;
            rq.pos = pos;
            rq.color = color;
            rq.size = size;

            rq.layer = 100;

            cameras[cameraId]
             .AddQuery(rq);
        }
        public void QuickDrawLine(Vector2 a, Vector2 b, Color color, int thickness, int cameraId)
        {
            LineRenderQuery rq = new LineRenderQuery();

            rq.a = a;
            rq.b = b;
            rq.layer = 99;
            rq.size = thickness;
            rq.color = color;
            cameras[cameraId]
             .AddQuery(rq);
        }
        public void QuickDrawBox(Vector2 position, Vector2 size, Color color, int thickness, int cameraId)
        {
            LineRenderQuery tl = new LineRenderQuery();
            LineRenderQuery tr = new LineRenderQuery();
            LineRenderQuery bl = new LineRenderQuery();
            LineRenderQuery br = new LineRenderQuery();

            Vector2 topLeft = new Vector2(position.X, position.Y);
            Vector2 topRight = new Vector2(position.X + size.X, position.Y);
            Vector2 bottomLeft = new Vector2(position.X, position.Y + size.Y);
            Vector2 bottomRight = new Vector2(position.X + size.X, position.Y + size.Y);

            // top left
            tl.a = topLeft;
            tl.b = topRight;

            // topRight
            tr.a = topRight;
            tr.b = bottomRight;

            // bottomRight
            br.a = bottomRight;
            br.b = bottomLeft;

            // bottomLeft
            bl.a = bottomLeft;
            bl.b = topLeft;


            tl.size = thickness;
            br.size = thickness;
            tr.size = thickness;
            bl.size = thickness;

            tl.color = color;
            tr.color = color;
            br.color = color;
            bl.color = color;


            tl.layer = 99;
            tr.layer = 99;
            bl.layer = 99;
            br.layer = 99;

            cameras[cameraId]
             .AddQuery(tl);

            cameras[cameraId]
             .AddQuery(tr);

            cameras[cameraId]
             .AddQuery(bl);

            cameras[cameraId]
             .AddQuery(br);
        }

        public void QuickDrawCircle(Vector2 position, float radious, Color color, int cameraId) {
            CircleRenderQuery circleRenderQuery = new CircleRenderQuery();
            circleRenderQuery.pos = position;
            circleRenderQuery.radius = radious;
            circleRenderQuery.color = color;

            circleRenderQuery.layer = 99;

            cameras[cameraId]
                    .AddQuery(circleRenderQuery);

        }

        public void AddCamera(Camera camera) { 
            this.cameras.Add(camera);
            camera.id = cameras.Count - 1;
            Logger.Log($"Rendering Manager {this.GetType().FullName} - Added Camera {this.cameras.Count-1}");

        }
        public void Update()
        {
          
          
        }
        public void Draw()
        {
            if (Camera.debugCamera != null && Camera.debugCamera.enabled)
            {
                float x = Camera.main.position.X - (Camera.main.renderTexture.Texture.Width / 2);
                float y = Camera.main.position.Y - (Camera.main.renderTexture.Texture.Height / 2);
                float x2 = Camera.main.renderTexture.Texture.Width;
                float y2 = Camera.main.renderTexture.Texture.Height;

                QuickDrawBox(new Vector2(x, y), new Vector2(x2, y2), Color.Gold, 2, Camera.debugCamera.id);
                Camera.debugCamera.position = Camera.main.position;
            }
            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].InternalRender();
            }
            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].DrawToScreen();
            }
        }

        public void OnGUI()
        {

        }
    }
}
