using LubaEngine;
using LubaEngine.Components.ImGUIComponents;
using LubaEngine.Managers;
using LubaEngine.Rendering;
using LubaEngine.Types.Core;
using Raylib_cs;
using RockmanExGame;

internal static class Program
{
    private static void Main(string[] args)
    {
        bool devMode = args.Contains("-dev");
        bool globalFolder = args.Contains("-globalfolder");

        #if DEBUG
            devMode = true;
            globalFolder = true;
        #endif


        WindowProperties window = new WindowProperties
        {
            width = 1360,
            height = 768,
            title = "Luba Retro Engine"
        };

        EngineProperties engineProperties = new EngineProperties("RockmanEx", devMode, globalFolder, window,
            new List<KeyboardKey> { KeyboardKey.Right, KeyboardKey.Left, KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Z, KeyboardKey.X, KeyboardKey.C});
        engineProperties.targetFPS = 120;

        // Setup engine
        EngineCore.Setup(engineProperties);

        // Setup Engine Systems
        EngineCore.GetComponent<SceneManager>().RegisterScene("test", () => new TestScene());
        EngineCore.GetComponent<SceneManager>().LoadScene("test");
        EngineCore.Run(); // Start Engine, this enters a loop.
    }
}