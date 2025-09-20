// File: Audio/SoundBank.cs
using SplashKitSDK;

namespace NeonDrift
{
    /// <summary>
    /// Simple audio helper for SFX + music.
    /// - Init() makes sure audio is open.
    /// - PlaySfx(...) supports optional volume and repetitions.
    /// - PlayMusic(name, times) maps to SplashKit's int 'times' overload.
    /// - PlayMusicLoop(name) repeats many times (SplashKit C# has no bool loop).
    /// </summary>
    public static class SoundBank
    {
        private static bool _ready;

        public static void Init()
        {
            if (_ready) return;
            if (!SplashKit.AudioReady()) SplashKit.OpenAudio();
            _ready = SplashKit.AudioReady(); // true when audio is ready. :contentReference[oaicite:1]{index=1}
        }

        public static void PlaySfx(string name, double? volume = null, int times = 1)
        {
            if (!_ready) Init();
            if (string.IsNullOrWhiteSpace(name)) return;

            if (volume.HasValue)
            {
                if (times <= 1) SplashKit.PlaySoundEffect(name, volume.Value);
                else            SplashKit.PlaySoundEffect(name, times, volume.Value);
            }
            else
            {
                if (times <= 1) SplashKit.PlaySoundEffect(name);
                else            SplashKit.PlaySoundEffect(name, times);
            }
            // Overloads are documented on SplashKit's Audio page. :contentReference[oaicite:2]{index=2}
        }

        /// <summary>
        /// Play music by name. 'times' = number of times to play.
        /// </summary>
        public static void PlayMusic(string name, int times = 1)
        {
            if (!_ready) Init();
            if (string.IsNullOrWhiteSpace(name)) return;

            SplashKit.StopMusic(); // switch track cleanly. :contentReference[oaicite:3]{index=3}
            if (times <= 1) SplashKit.PlayMusic(name);      // plays once
            else            SplashKit.PlayMusic(name, times); // plays 'times' times
            // Signatures: PlayMusic(name) and PlayMusic(name, int times). :contentReference[oaicite:4]{index=4}
        }

        /// <summary>
        /// Convenience "loop". SplashKit C# doesn't take a bool; docs don’t specify an infinite sentinel.
        /// Using a large repeat count achieves near-indefinite play.
        /// </summary>
        public static void PlayMusicLoop(string name) => PlayMusic(name, 1000);

        public static void FadeOutAllSfx(int ms = 300)
        {
            if (!_ready) return;
            SplashKit.FadeAllSoundEffectsOut(ms); // documented API. :contentReference[oaicite:5]{index=5}
        }

        public static void StopMusic()
        {
            if (!_ready) return;
            SplashKit.StopMusic(); // documented API. :contentReference[oaicite:6]{index=6}
        }
    }
}
