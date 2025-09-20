// File: Core/GameOverState.cs
using System;
using SplashKitSDK;

namespace NeonDrift
{
    /// <summary>
    /// Game over: shows SCORE and BEST SCORE, persists best via BestScoreStore.
    /// Enter = restart, Esc = menu.
    /// </summary>
    internal sealed class GameOverState : IGameState
    {
        private readonly Game _game;
        private readonly int  _finalScore;
        private int  _bestScore;
        private bool _newBest;
        private double _pulse;

        public GameOverState(Game game, int finalScore)
        {
            _game = game;
            _finalScore = finalScore;
        }

        public void Enter()
        {
            // stop/transition audio if you like
            if (SplashKit.HasMusic("bgm")) SoundBank.StopMusic();

            _bestScore = BestScoreStore.Load();
            if (_finalScore > _bestScore)
            {
                _bestScore = _finalScore;
                _newBest = true;
                BestScoreStore.Save(_bestScore);
                if (SplashKit.HasSoundEffect("newbest")) SoundBank.PlaySfx("newbest");
            }
            else
            {
                if (SplashKit.HasSoundEffect("gameover")) SoundBank.PlaySfx("gameover");
            }

            _pulse = 0;
        }

        public void HandleInput()
        {
            if (SplashKit.KeyTyped(KeyCode.ReturnKey)) _game.StartGame();
            if (SplashKit.KeyTyped(KeyCode.EscapeKey)) _game.ShowMenu();
        }

        public void Update()
        {
            _pulse += 0.06;
            if (_pulse > Math.PI * 2) _pulse = 0;
        }

        public void Draw()
        {
            var win = _game.Window;
            SplashKit.ClearScreen(Color.Black);

            string title = "GAME OVER";
            string score = $"SCORE: {_finalScore}";
            string best  = $"BEST:  {_bestScore}";
            string hint1 = "Enter: Restart";
            string hint2 = "Esc: Menu";
            string flag  = _newBest ? "NEW BEST!" : "";

            double cx = win.Width * 0.5, cy = win.Height * 0.4;
            double Left(string s) => cx - (s.Length * 8) / 2.0;

            SplashKit.DrawText(title, Color.White, Left(title), cy);
            SplashKit.DrawText(score, Color.White, Left(score), cy + 24);
            SplashKit.DrawText(best,  Color.White, Left(best),  cy + 44);

            if (_newBest)
                SplashKit.DrawText(flag, Color.Yellow, Left(flag), cy + 64);

            SplashKit.DrawText(hint1, Color.White, Left(hint1), cy + 86);
            SplashKit.DrawText(hint2, Color.White, Left(hint2), cy + 106);
        }

        public void Exit()    { }
        public void Dispose() { }
    }
}
