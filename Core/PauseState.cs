// File: Core/PauseState.cs
using System;
using SplashKitSDK;

namespace NeonDrift
{
    /// <summary>
    /// Pause screen overlay.
    /// NOTE: This version restarts a fresh run on resume (Enter) to avoid
    /// changing the StateMachine disposal logic. If you want true resume,
    /// we can add a push/pop stack in StateMachine next.
    ///
    /// Controls:
    ///   Enter = resume (starts a new PlayState)
    ///   Esc   = menu
    /// </summary>
    internal sealed class PauseState : IGameState
    {
        private readonly Game _game;
        private double _pulse;

        public PauseState(Game game) => _game = game;

        public void Enter() => _pulse = 0;

        public void HandleInput()
        {
            // Edge-triggered key presses per SplashKit input docs
            if (SplashKit.KeyTyped(KeyCode.ReturnKey))
                _game.StartGame();   // fresh run

            if (SplashKit.KeyTyped(KeyCode.EscapeKey))
                _game.ShowMenu();
        }

        public void Update()
        {
            _pulse += 0.05;
            if (_pulse > Math.PI * 2) _pulse = 0;
        }

        public void Draw()
        {
            var win = _game.Window;

            // Dim the screen with a translucent overlay
            SplashKit.FillRectangle(Color.RGBAColor(0, 0, 0, 140), 0, 0, win.Width, win.Height);

            // Pulsing outline rectangle for a little feedback
            int w = 360, h = 120;
            int x = (win.Width  - w) / 2;
            int y = (win.Height - h) / 2;
            SplashKit.DrawRectangle(Color.White, x, y, w, h);

            string title = "PAUSED";
            string hint1 = "Enter: Resume";
            string hint2 = "Esc: Menu";

            // Rough centering using ~8 px/char (bitmap font via DrawText)
            double cx = win.Width  * 0.5;
            double cy = win.Height * 0.5;
            double Left(string s) => cx - (s.Length * 8) / 2.0;

            // Slight vertical bob
            double bob = Math.Sin(_pulse) * 4.0;

            SplashKit.DrawText(title, Color.White, Left(title), cy - 20 + bob);
            SplashKit.DrawText(hint1, Color.White, Left(hint1), cy + 14);
            SplashKit.DrawText(hint2, Color.White, Left(hint2), cy + 34);
        }

        public void Exit()    { }
        public void Dispose() { }
    }
}
