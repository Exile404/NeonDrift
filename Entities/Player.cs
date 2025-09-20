// File: Entities/Player.cs
using SplashKitSDK;

namespace NeonDrift
{
    /// <summary>
    /// Minimal player entity:
    /// - Consumes IInputSource (no direct engine calls in game logic)
    /// - Implements IUpdatable, IRenderable, ICollidable
    /// - Clamps movement to the world bounds
    /// </summary>
    public sealed class Player : IUpdatable, IRenderable, ICollidable
    {
        private readonly IInputSource _input;

        // World (playfield) bounds — typically your Window.Width/Height.
        private readonly double _worldW;
        private readonly double _worldH;

        // Position and dimensions
        private double _x;
        private double _y;
        private const double ShipW = 48.0;
        private const double ShipH = 28.0;

        // Basic movement speed (pixels per frame at ~60fps)
        private const double Speed = 6.0;

        public Player(double startX, double startY, IInputSource input, double worldWidth, double worldHeight)
        {
            _x = startX;
            _y = startY;
            _input = input;
            _worldW = worldWidth;
            _worldH = worldHeight;
        }

        public void Update()
        {
            // Movement from input
            if (_input.Left)  _x -= Speed;
            if (_input.Right) _x += Speed;
            if (_input.Up)    _y -= Speed;
            if (_input.Down)  _y += Speed;

            // Clamp to world bounds so the player never leaves the screen
            _x = Clamp(_x, 0, _worldW - ShipW);
            _y = Clamp(_y, 0, _worldH - ShipH);

            // (Optional later) if (_input.Action) -> dash/boost, cooldown, etc.
        }

        public void Draw()
        {
            // Simple neon-y placeholder: cyan body with a thin inner line
            SplashKit.FillRectangle(Color.Cyan, _x, _y, ShipW, ShipH);
            SplashKit.DrawRectangle(Color.Black, _x + 2, _y + 2, ShipW - 4, ShipH - 4);
        }

        public Rectangle Hitbox => SplashKit.RectangleFrom(_x, _y, ShipW, ShipH);

        // Utility
        private static double Clamp(double v, double min, double max)
            => (v < min) ? min : (v > max ? max : v);
    }
}
