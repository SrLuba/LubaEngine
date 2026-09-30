using ImGuiNET;
using LubaEngine.Components;
using LubaEngine.Components.ImGUIComponents;
using LubaEngine.Types;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;


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

        public void FrameUpdate()
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
        public void Tick()
        {
          
          
        }
        public void Draw()
        {
            if (Camera.debugCamera != null && Camera.debugCamera.enabled)
            {

                float wheel = Raylib.GetMouseWheelMove();
                if (wheel > 0)
                {
                    Camera.debugCamera.zoom += .1f;

                }
                else if (wheel < 0)
                {
                    Camera.debugCamera.zoom -= .1f;
                    if (Camera.debugCamera.zoom < .9f) Camera.debugCamera.zoom = .9f;
                }
                ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NoMouseCursorChange;
                if (Raylib.IsMouseButtonDown(MouseButton.Right))
                {
                    Vector2 delta = Raylib.GetMouseDelta();
                    delta /= Camera.debugCamera.scale * Camera.debugCamera.zoom;
                    Camera.debugCamera.position += new SBVector2((int)(-delta.X * 256), (int)(-delta.Y * 256));
                    Raylib.SetMouseCursor(MouseCursor.ResizeAll);
                }
                else { Raylib.SetMouseCursor(MouseCursor.Default); }
           


                float x = Camera.main.position.PixelX - (Camera.main.renderTexture.Texture.Width / 2);
                float y = Camera.main.position.PixelY - (Camera.main.renderTexture.Texture.Height / 2);
                float x2 = Camera.main.renderTexture.Texture.Width;
                float y2 = Camera.main.renderTexture.Texture.Height;

                QuickDrawBox(new Vector2(x, y), new Vector2(x2, y2), Color.Gold, 2, Camera.debugCamera.id);
                Camera cam = Camera.debugCamera;

                int left = (int)(cam.position.PixelX - cam.resolution.X / 2);
                int right = (int)(cam.position.PixelX + cam.resolution.X / 2);
                int top = (int)(cam.position.PixelY - cam.resolution.Y / 2);
                int bottom = (int)(cam.position.PixelY + cam.resolution.Y / 2);

                int cell = 16;
                int startX = (int)MathF.Floor(left / 16f) * 16; 
                int startY = (int)MathF.Floor(top / 16f) * 16;

                for (int xX = startX; xX <= right; xX += cell)
                    QuickDrawLine(new Vector2(xX, top), new Vector2(xX, bottom), new Color(1f,.5f,.5f,.04f * Camera.debugCamera.zoom), 1, Camera.debugCamera.id);

                for (int yY = startY; yY <= bottom; yY += cell)
                    QuickDrawLine(new Vector2(left, yY), new Vector2(right, yY), new Color(1f, .5f, .5f, .04f * Camera.debugCamera.zoom), 1, Camera.debugCamera.id);

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
