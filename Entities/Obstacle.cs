// File: Entities/Obstacle.cs
using SplashKitSDK;

namespace NeonDrift
{
    /// <summary>
    /// Falling obstacle the player must dodge.
    /// - Frame-based vertical motion
    /// - Deactivates when it leaves the screen
    /// - Exposes a Rectangle hitbox for simple collision checks
    /// </summary>
    public sealed class Obstacle : IUpdatable, IRenderable, ICollidable
    {
        private double _x, _y;
        private readonly double _w, _h;
        private double _vy;
        private readonly double _worldH;

        public bool IsAlive { get; private set; } = true;

        public Obstacle(double x, double y, double width, double height, double speedY, double worldHeight)
        {
            _x = x;
            _y = y;
            _w = width;
            _h = height;
            _vy = speedY;
            _worldH = worldHeight;
        }

        public void Update()
        {
            _y += _vy;

            // Offscreen -> mark for removal by the spawner/system
            if (_y > _worldH) IsAlive = false;
        }

        public void Draw()
        {
            // Neon-ish placeholder; you can swap to a bitmap/sprite later
            SplashKit.FillRectangle(Color.Fuchsia, _x, _y, _w, _h);
            SplashKit.DrawRectangle(Color.Black, _x + 1, _y + 1, _w - 2, _h - 2);
        }

        // Use SplashKit geometry helpers for collisions
        public Rectangle Hitbox => SplashKit.RectangleFrom(_x, _y, _w, _h);
    }
}
