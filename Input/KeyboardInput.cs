using SplashKitSDK;

namespace NeonDrift
{
    /// <summary>
    /// Keyboard-backed input source for gameplay. Uses held arrows for movement
    /// and a single-press Space as an action (e.g., dash/boost).
    /// </summary>
    public sealed class KeyboardInput : IInputSource
    {
        public bool Left  => SplashKit.KeyDown(KeyCode.LeftKey);
        public bool Right => SplashKit.KeyDown(KeyCode.RightKey);
        public bool Up    => SplashKit.KeyDown(KeyCode.UpKey);
        public bool Down  => SplashKit.KeyDown(KeyCode.DownKey);

        // Action is a "typed" (edge-trigger) input so it fires once per press.
        public bool Action => SplashKit.KeyTyped(KeyCode.SpaceKey);
    }
}
