// File: Program.cs
using System;
using SplashKitSDK;

namespace NeonDrift
{
    public static class Program
    {
        public static void Main()
        {
            const int Width = 960;
            const int Height = 540;

            // Load bundle if present (safe if missing)
            try { ResourceBundleLoader.Load("main", "main.txt"); } catch { }
            SoundBank.Init();

            var window = new Window("Neon Drift", Width, Height);

            // >>> Start in fullscreen
            window.ToggleFullscreen();   // SplashKit API to go fullscreen
            // <<<

            var game = new Game(window);

            // Clean, safe loop: stop as soon as OS or game asks to quit
            while (!window.CloseRequested && !game.QuitRequested)
            {
                SplashKit.ProcessEvents();

                // (Optional hotkey: F11 to toggle fullscreen at runtime)
                if (SplashKit.KeyTyped(KeyCode.F11Key))
                    window.ToggleFullscreen();

                if (window.CloseRequested || game.QuitRequested) break;

                game.HandleInput();
                if (window.CloseRequested || game.QuitRequested) break;

                game.Update();
                if (window.CloseRequested || game.QuitRequested) break;

                game.Draw();      // Game.cs clears the screen each frame
                if (window.CloseRequested || game.QuitRequested) break;

                window.Refresh(60);
            }

            // Shutdown
            try { SoundBank.StopMusic(); } catch { }
            try { game.Dispose(); } catch { }
            try { ResourceBundleLoader.Free("main"); } catch { }
        }
    }
}
