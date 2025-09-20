// File: Core/PlayState.cs
using System;
using SplashKitSDK;

namespace NeonDrift
{
    internal sealed class PlayState : IGameState
    {
        private readonly Game _game;

        private IInputSource? _input;
        private Player? _player;
        private Spawner? _spawner;
        private CollisionSystem? _collisions;
        private Hud? _hud;

        private double _laneOffset;
        private const int LaneWidth = 96;
        private const int GridStep  = 32;
        private const double ScrollSpeed = 2.0;

        public PlayState(Game game) => _game = game;

        public void Enter()
        {
            Console.WriteLine("Play: Arrow keys move. Esc to quit.");

            _input = new KeyboardInput();

            double w = _game.Window.Width, h = _game.Window.Height;
            double startX = (w - 48.0) / 2.0, startY = h - 100.0;

            _player  = new Player(startX, startY, _input, w, h);
            _spawner = new Spawner(w, h, initialInterval: 50, baseSpeed: 3.5, shardChance: 0.25);
            _hud     = new Hud(_game, framesPerPoint: 10);
            _collisions = new CollisionSystem(_player, _spawner, _hud);

            _laneOffset = 0.0;

            // BGM as SOUND (WAV) — looping
            if (SplashKit.HasSoundEffect("bgm"))
                SoundBank.PlaySfx("bgm", volume: 0.8, times: 1000);
        }

        public void HandleInput()
        {
            if (SplashKit.KeyTyped(KeyCode.EscapeKey))
                _game.RequestQuit();   // <-- just set the flag
        }

        public void Update()
        {
            _player?.Update();
            _spawner?.Update();
            _hud?.Update();

            _collisions?.Update();
            if (_collisions is not null && _collisions.PlayerCollided)
            {
                int finalScore = _hud?.Score ?? 0;
                _game.GameOver(finalScore);
                return;
            }

            _laneOffset += ScrollSpeed;
            if (_laneOffset >= GridStep) _laneOffset -= GridStep;
        }

        public void Draw()
        {
            var win = _game.Window;

            for (int x = 0; x < win.Width; x += LaneWidth)
            {
                SplashKit.DrawLine(Color.Cyan,  x - 2, 0, x - 2, win.Height);
                SplashKit.DrawLine(Color.Cyan,  x + 2, 0, x + 2, win.Height);
                SplashKit.DrawLine(Color.White, x,     0, x,     win.Height);
            }

            for (int y = -GridStep; y < win.Height + GridStep; y += GridStep)
            {
                int yy = (int)(y + _laneOffset);
                SplashKit.DrawLine(Color.Gray, 0, yy, win.Width, yy);
            }

            _spawner?.Draw();
            _player?.Draw();
            _hud?.Draw();
        }

        public void Exit()    { }
        public void Dispose() { }
    }
}
