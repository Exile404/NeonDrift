// File: Core/Contracts.cs
using SplashKitSDK;

namespace NeonDrift
{
    // Update/draw hooks so systems don't depend on concrete types
    public interface IUpdatable { void Update(); }
    public interface IRenderable { void Draw(); }

    // Expose a hitbox so we can use SplashKit.RectanglesIntersect(...)
    public interface ICollidable
    {
        Rectangle Hitbox { get; }
    }

    // Input abstraction so gameplay doesn't query SplashKit directly
    public interface IInputSource
    {
        bool Left { get; }
        bool Right { get; }
        bool Up { get; }
        bool Down { get; }
        bool Action { get; } // e.g., dash/boost
    }
}
