// File: Entities/Obstacle.cs
using SplashKitSDK;

namespace NeonDrift
{
    /// <summary>
    /// Falling obstacle drawn from a car bitmap.
    /// </summary>
    public sealed class Obstacle : IUpdatable, IRenderable, ICollidable
    {
        private double _x, _y;
        private readonly double _speed;
        private readonly double _worldH;
        private readonly Bitmap _bmp;

        public bool IsAlive { get; private set; } = true;

        public Obstacle(double x, double y, double speed, double worldHeight, Bitmap bmp)
        {
            _x = x; _y = y;
            _speed = speed;
            _worldH = worldHeight;
            _bmp = bmp;
        }

        public void Update()
        {
            _y += _speed;
            if (_y > _worldH) IsAlive = false;
        }

        public void Draw() => SplashKit.DrawBitmap(_bmp, _x, _y);  

        public Rectangle Hitbox => SplashKit.RectangleFrom(
            _x, _y,
            SplashKit.BitmapWidth(_bmp),
            SplashKit.BitmapHeight(_bmp)
        );
    }
}
