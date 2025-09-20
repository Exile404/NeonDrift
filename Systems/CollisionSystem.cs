// File: Systems/CollisionSystem.cs
using SplashKitSDK;

namespace NeonDrift
{
    /// <summary>
    /// Detects Player vs Obstacles (death) and Player vs Shards (pickup).
    /// On shard pickup: adds to HUD score, plays SFX (if available), and flags shard for removal.
    /// </summary>
    public sealed class CollisionSystem : IUpdatable
    {
        private readonly Player  _player;
        private readonly Spawner _spawner;
        private readonly Hud     _hud;

        public bool PlayerCollided { get; private set; }

        public CollisionSystem(Player player, Spawner spawner, Hud hud)
        {
            _player  = player;
            _spawner = spawner;
            _hud     = hud;
        }

        public void Update()
        {
            PlayerCollided = false;

            // Player rectangle
            var p = _player.Hitbox;

            // 1) Death check: player vs obstacles
            var obstacles = _spawner.Obstacles;
            for (int i = 0; i < obstacles.Count; i++)
            {
                if (SplashKit.RectanglesIntersect(p, obstacles[i].Hitbox))
                {
                    PlayerCollided = true;
                    return;
                }
            }

            // 2) Pickups: player vs shards (award score + remove)
            var shards = _spawner.Shards;
            for (int i = shards.Count - 1; i >= 0; i--)
            {
                var s = shards[i];
                if (SplashKit.RectanglesIntersect(p, s.Hitbox))
                {
                    _hud.AddScore(s.ScoreValue);
                    if (SplashKit.HasSoundEffect("pickup")) SoundBank.PlaySfx("pickup");
                    s.Kill(); // mark for pruning; Spawner removes it next Update()
                }
            }
        }
    }
}
