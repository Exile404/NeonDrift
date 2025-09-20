// File: Entities/Shard.cs
using System;
using SplashKitSDK;

namespace NeonDrift
{
    /// <summary>
    /// Score collectible that drifts downward and can be picked up by the player.
    /// - Simple rectangular hitbox (AABB) for collisions
    /// - Gentle shimmer line for a bit of visual feedback without assets
    /// - Marked dead (IsAlive=false) when collected or off-screen
    /// </summary>
    public sealed class Shard : IUpdatable, IRenderable, ICollidable
    {
        private double _x, _y;
        private readonly double _w, _h;
        private readonly double _worldH;
        private double _vy;
        private double _t;

        /// <summary>True while active; set to false to remove from the game.</summary>
        public bool IsAlive { get; private set; } = true;

        /// <summary>How many points this shard awards on pickup.</summary>
        public int ScoreValue { get; }

        public Shard(double x, double y, double width, double height, double speedY, int scoreValue, double worldHeight)
        {
            _x = x;
            _y = y;
            _w = width;
            _h = height;
            _vy = speedY;
            _worldH = worldHeight;
            ScoreValue = scoreValue < 0 ? 0 : scoreValue;
        }

        public void Update()
        {
            _t += 0.08;     // time base for shimmer animation
            _y += _vy;      // downward drift

            // Off-screen cleanup
            if (_y > _worldH) IsAlive = false;
        }

        public void Draw()
        {
            // Neon-ish pill
            SplashKit.FillRectangle(Color.Yellow, _x, _y, _w, _h);
            SplashKit.DrawRectangle(Color.Black, _x + 1, _y + 1, _w - 2, _h - 2);

            // Faint moving highlight
            double hx = _x + (_w * (0.2 + 0.1 * Math.Sin(_t)));
            SplashKit.DrawLine(Color.White, hx, _y + 2, hx, _y + _h - 2);
        }

        /// <summary>Axis-aligned bounding box used for collision checks.</summary>
        public Rectangle Hitbox => SplashKit.RectangleFrom(_x, _y, _w, _h);

        /// <summary>Mark this shard for removal (called on pickup).</summary>
        public void Kill() => IsAlive = false;
    }
}
