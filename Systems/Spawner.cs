// File: Systems/Spawner.cs
using System;
using System.Collections.Generic;

namespace NeonDrift
{
    /// <summary>
    /// Spawns and manages falling obstacles + collectible shards.
    /// - Interval-based spawning with a mild difficulty ramp
    /// - Randomly chooses obstacle vs shard (see _shardChance)
    /// - Prunes off-screen entities
    /// </summary>
    public sealed class Spawner : IUpdatable, IRenderable
    {
        private readonly double _worldW, _worldH;
        private readonly List<Obstacle> _obstacles = new();
        private readonly List<Shard> _shards = new();
        private readonly Random _rng = new();

        private int _cooldown;
        private int _baseInterval;      // frames between spawns
        private double _baseSpeed;      // base fall speed
        private int _frame;

        private readonly double _shardChance; // probability per spawn

        public Spawner(double worldWidth, double worldHeight, int initialInterval = 50, double baseSpeed = 3.5, double shardChance = 0.25)
        {
            _worldW = worldWidth;
            _worldH = worldHeight;
            _baseInterval = Math.Max(10, initialInterval);
            _baseSpeed = baseSpeed;
            _shardChance = Math.Clamp(shardChance, 0.0, 1.0);
            ResetCooldown();
        }

        public IReadOnlyList<Obstacle> Obstacles => _obstacles;
        public IReadOnlyList<Shard>    Shards    => _shards;

        public void Update()
        {
            _frame++;
            _cooldown--;

            if (_cooldown <= 0)
            {
                SpawnOne();
                ResetCooldown();
            }

            // Update/prune obstacles
            for (int i = _obstacles.Count - 1; i >= 0; i--)
            {
                var o = _obstacles[i];
                o.Update();
                if (!o.IsAlive) _obstacles.RemoveAt(i);
            }

            // Update/prune shards
            for (int i = _shards.Count - 1; i >= 0; i--)
            {
                var s = _shards[i];
                s.Update();
                if (!s.IsAlive) _shards.RemoveAt(i);
            }
        }

        public void Draw()
        {
            foreach (var o in _obstacles) o.Draw();
            foreach (var s in _shards)    s.Draw();
        }

        public void Clear()
        {
            _obstacles.Clear();
            _shards.Clear();
        }

        private void SpawnOne()
        {
            if (_rng.NextDouble() < _shardChance) SpawnShard();
            else                                   SpawnObstacle();
        }

        private void SpawnObstacle()
        {
            double w = Lerp(36, 72, _rng.NextDouble());
            double h = Lerp(20, 40, _rng.NextDouble());
            double x = _rng.NextDouble() * (_worldW - w);
            double y = -h;
            double speed = _baseSpeed + Lerp(0.0, 3.0, DifficultyFactor());

            _obstacles.Add(new Obstacle(x, y, w, h, speed, _worldH));
        }

        private void SpawnShard()
        {
            double w = Lerp(20, 28, _rng.NextDouble());
            double h = Lerp(12, 18, _rng.NextDouble());
            double x = _rng.NextDouble() * (_worldW - w);
            double y = -h;
            double speed = (_baseSpeed * 0.55) + Lerp(0.0, 1.2, DifficultyFactor());
            int score = 5 + (int)Math.Floor(Lerp(0, 10, DifficultyFactor())); // 5..15

            _shards.Add(new Shard(x, y, w, h, speed, score, _worldH));
        }

        private void ResetCooldown()
        {
            int elapsedSeconds = _frame / 60;
            int interval = Math.Max(16, _baseInterval - (elapsedSeconds / 10));
            int jitter = _rng.Next(-4, 5); // [-4,+4]
            _cooldown = Math.Max(8, interval + jitter);
        }

        private double DifficultyFactor() => Math.Min(1.0, _frame / 3600.0);
        private static double Lerp(double a, double b, double t) => a + (b - a) * t;
    }
}
