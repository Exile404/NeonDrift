// File: Entities/Player.cs
using SplashKitSDK;

namespace NeonDrift
{
    /// <summary>
    /// Player as a bitmap (motorcycle).
    /// - Uses IInputSource for movement
    /// - Hitbox matches the bitmap's pixel size
    /// </summary>
    public sealed class Player : IUpdatable, IRenderable, ICollidable
    {
        private readonly IInputSource _input;
        private readonly double _worldW, _worldH;

        private double _x, _y;
        private readonly Bitmap _bmp;

        // Movement speed (px/frame @ ~60fps)
        private const double Speed = 6.0;

        public Player(double startX, double startY, IInputSource input, double worldWidth, double worldHeight)
        {
            _x = startX; _y = startY;
            _input = input;
            _worldW = worldWidth; _worldH = worldHeight;

            // Loaded by ResourceBundleLoader from Resources/bundles/main.txt
            // e.g., BITMAP,player_bike,motorcycle_green.png
            _bmp = SplashKit.BitmapNamed("player_bike");   // :contentReference[oaicite:1]{index=1}
        }

        public void Update()
        {
            if (_input.Left)  _x -= Speed;
            if (_input.Right) _x += Speed;
            if (_input.Up)    _y -= Speed;
            if (_input.Down)  _y += Speed;

            double w = SplashKit.BitmapWidth(_bmp);   // :contentReference[oaicite:2]{index=2}
            double h = SplashKit.BitmapHeight(_bmp);  // :contentReference[oaicite:3]{index=3}

            _x = Clamp(_x, 0, _worldW - w);
            _y = Clamp(_y, 0, _worldH - h);
        }

        public void Draw()
        {
            // Draw from top-left (keep consistent with Hitbox)
            SplashKit.DrawBitmap(_bmp, _x, _y);       // :contentReference[oaicite:4]{index=4}
        }

        public Rectangle Hitbox
        {
            get
            {
                return SplashKit.RectangleFrom(
                    _x, _y,
                    SplashKit.BitmapWidth(_bmp),
                    SplashKit.BitmapHeight(_bmp)
                );
            }
        }

        private static double Clamp(double v, double min, double max)
            => (v < min) ? min : (v > max ? max : v);
    }
}
