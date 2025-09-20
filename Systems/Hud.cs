// File: Systems/Hud.cs
using SplashKitSDK;

namespace NeonDrift
{
    /// <summary>
    /// Heads-up display for score/time (asset-free).
    /// - Increments score over time (frame-based) and supports manual AddScore.
    /// - Renders a translucent backdrop + text using SplashKit.DrawText.
    /// </summary>
    public sealed class Hud : IUpdatable, IRenderable
    {
        private readonly Game _game;

        private int _frames;          // used for simple time/score tick
        private int _score;           // public getter via property below

        // Tuning: how often to auto-add score (frames per point at ~60 fps)
        private readonly int _framesPerPoint;

        public Hud(Game game, int framesPerPoint = 10)
        {
            _game = game;
            _framesPerPoint = framesPerPoint < 1 ? 1 : framesPerPoint;
            _frames = 0;
            _score  = 0;
        }

        public int Score => _score;

        /// <summary>Add points (e.g., from pickups or near-misses).</summary>
        public void AddScore(int points) => _score = points < 0 ? _score : _score + points;

        public void Update()
        {
            _frames++;
            if (_frames % _framesPerPoint == 0) _score++; // +~6/sec if framesPerPoint=10
        }

        public void Draw()
        {
            var win = _game.Window;

            // Translucent backing panel to improve legibility
            SplashKit.FillRectangle(Color.RGBAColor(0, 0, 0, 140), 8, 8, 180, 54);

            // Labels (roughly aligned; default bitmap font ~8px/char)
            string s1 = $"SCORE: {_score}";
            string s2 = "ESC: Quit";

            SplashKit.DrawText(s1, Color.White, 16, 20);
            SplashKit.DrawText(s2, Color.Gray,  16, 44);
        }
    }
}
