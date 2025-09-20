// File: Core/MenuState.cs
using System;
using SplashKitSDK;

namespace NeonDrift
{
    internal sealed class MenuState : IGameState
    {
        private readonly Game _game;
        private double _pulse;

        public MenuState(Game game) => _game = game;

        public void Enter() => _pulse = 0;

        public void HandleInput()
        {
            if (SplashKit.KeyTyped(KeyCode.ReturnKey))
                _game.StartGame();

            if (SplashKit.KeyTyped(KeyCode.EscapeKey))
                _game.RequestQuit();  // <-- do NOT close window here
        }

        public void Update()
        {
            _pulse += 0.06;
            if (_pulse > Math.PI * 2) _pulse = 0;
        }

        public void Draw()
        {
            var win = _game.Window;

            // simple ring + title
            float cx = (float)win.Width / 2f;
            float cy = (float)win.Height / 2f;
            float r  = 110f + 18f * (float)Math.Sin(_pulse);

            for (int i = 0; i < 5; i++) SplashKit.DrawCircle(Color.Cyan, cx, cy, r + i * 4f);

            string title = "NEON DRIFT";
            string play  = "Press Enter to Play";
            string quit  = "Press Esc to Quit";

            double cxd = win.Width * 0.5, cyd = win.Height * 0.5;
            double Left(string s) => cxd - (s.Length * 8) / 2.0;

            SplashKit.DrawText(title, Color.White, Left(title), cyd - 10);
            SplashKit.DrawText(play,  Color.White, Left(play),  cyd + 60);
            SplashKit.DrawText(quit,  Color.White, Left(quit),  cyd + 80);
        }

        public void Exit()    { }
        public void Dispose() { }
    }
}
