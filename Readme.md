# Neon Drift — An OOP + SplashKit tutorial in C\#

> A publish-ready, step-by-step walkthrough showing how to design and build a small arcade game (“Neon Drift”) in C# with SplashKit, while explicitly demonstrating OOP design, state management, resource bundles, input, collisions, drawing, audio, and packaging. This tutorial is written in Markdown and maps the work to the unit’s learning outcomes for SIT771 Task 7.4H (High Distinction).&#x20;

---

## What you’ll build

Neon Drift is a minimalist lane-dodger:

* Player: a motorcycle you steer with the arrow keys
* Hazards: cars spawn and scroll down
* Pickups: shard collectibles add to score
* States: **Menu → Play → Game Over**
* HUD: score + best score
* Audio: background loop + pickup SFX
* Fullscreen toggle (F11)

You’ll see how to structure the code into **entities**, **systems**, and a **state machine**, and how to load assets with **SplashKit resource bundles**, process input each frame, draw, detect collisions, and organize a clean game loop. ([SplashKit][1])

---

## Learning outcomes (how this tutorial demonstrates them)

* **OOP design:** interfaces (`IGameState`, `IUpdatable`, `IRenderable`, `ICollidable`), composition (Player uses an `IInputSource`), SRP per class (Spawner, CollisionSystem, HUD).
* **Abstraction & modularity:** state machine separates flow from rendering/logic; systems separate “what it is” from “what it does”.
* **API use:** correct SplashKit usage for **event processing**, **graphics**, **geometry**, **resources**, and **audio**. ([SplashKit][2])
* **Process & testing:** incremental slices (boot → input → draw → collisions → audio), debug prints, and a final packaging checklist.
* **Professional practice:** resource bundles, attribution, and (optional) deployment notes.

---

## Prereqs & project structure

```
NeonDrift/
  Audio/            SoundBank.cs
  Core/             Game.cs, StateMachine.cs, MenuState.cs, PlayState.cs, GameOverState.cs
  Entities/         Player.cs, Obstacle.cs, Shard.cs
  Input/            IInputSource.cs, KeyboardInput.cs
  Systems/          Spawner.cs, CollisionSystem.cs, Hud.cs
  Resources/
    bundles/        main.txt
    images/         motorcycle_green.png, car_*_small_4.png
    sounds/         crime_never_sleeps.wav, pickup.wav
  Program.cs
  NeonDrift.csproj
```

**Why a `Resources/…` tree?** SplashKit resolves assets from a project `Resources` folder with subfolders (`bundles`, `images`, `sounds`, etc.). ([SplashKit][3])

---

## Step 1 — Create a window and the main loop

SplashKit GUIs must call **`ProcessEvents()` once every frame** to update key/mouse state; then you handle input, update, draw, and refresh. ([SplashKit][2])

```csharp
// Program.cs
using SplashKitSDK;

public static class Program
{
    public static void Main()
    {
        var window = new Window("Neon Drift", 960, 540);
        window.ToggleFullscreen(); // optional: start fullscreen

        var game = new NeonDrift.Game(window);

        while (!window.CloseRequested && !game.QuitRequested)
        {
            SplashKit.ProcessEvents(); // ← updates input state each frame
            game.HandleInput();
            game.Update();

            game.Draw();               // your Draw clears the backbuffer first
            window.Refresh(60);        // ~60 FPS
        }
    }
}
```

---

## Step 2 — Load assets with **resource bundles**

Create `Resources/bundles/main.txt` and list each asset as a CSV line. Supported kinds include **`BITMAP`**, **`SOUND`**, **`MUSIC`**, **`FONT`**, etc. Load the bundle at startup and you can retrieve assets by **name**. ([SplashKit][4])

```
# Resources/bundles/main.txt
BITMAP,player_bike,motorcycle_green.png
BITMAP,car_red,car_red_small_4.png
BITMAP,car_yellow,car_yellow_small_4.png
BITMAP,car_black,car_black_small_4.png
BITMAP,car_blue,car_blue_small_4.png

SOUND,bgm,crime_never_sleeps.wav
```

Why use bundles? They centralize asset names → filenames and let you load a whole set at once. ([SplashKit][4])

---

## Step 3 — A tiny **state machine**

```csharp
public interface IGameState : IDisposable
{
    void Enter();
    void HandleInput();
    void Update();
    void Draw();
}

public sealed class StateMachine : IDisposable
{
    private IGameState? _current;
    public void ChangeState(IGameState next)
    {
        _current?.Dispose();
        _current = next; _current.Enter();
    }
    public void HandleInput() => _current?.HandleInput();
    public void Update()      => _current?.Update();
    public void Draw()        => _current?.Draw();
    public void Dispose()     => _current?.Dispose();
}
```

`Game.cs` owns `StateMachine` and exposes **`StartGame()`**, **`ShowMenu()`**, **`GameOver(score)`**, and a `QuitRequested` flag (used by the loop). This isolates flow from features—clean OOP.

---

## Step 4 — Input abstraction

SplashKit exposes **KeyDown / KeyTyped** after `ProcessEvents()`. Wrap it in an interface so the Player doesn’t depend on the engine: ([SplashKit][2])

```csharp
public interface IInputSource { bool Left {get;} bool Right {get;} bool Up {get;} bool Down {get;} }
public sealed class KeyboardInput : IInputSource
{
    public bool Left  => SplashKit.KeyDown(KeyCode.LeftKey);
    public bool Right => SplashKit.KeyDown(KeyCode.RightKey);
    public bool Up    => SplashKit.KeyDown(KeyCode.UpKey);
    public bool Down  => SplashKit.KeyDown(KeyCode.DownKey);
}
```

---

## Step 5 — Player & rendering with bitmaps

Use **`BitmapNamed`** to retrieve a bitmap by name and **`DrawBitmap`** each frame. Use **`BitmapWidth/Height`** for hitboxes and clamping. ([SplashKit][5])

```csharp
// Entities/Player.cs
public sealed class Player : IUpdatable, IRenderable, ICollidable
{
    private readonly Bitmap _bmp = SplashKit.BitmapNamed("player_bike");
    private double _x, _y; private readonly IInputSource _in; private readonly double _wW, _wH;
    private const double Speed = 6;

    public Player(double x, double y, IInputSource input, double worldW, double worldH)
    { _x=x; _y=y; _in=input; _wW=worldW; _wH=worldH; }

    public void Update()
    {
        if (_in.Left)  _x -= Speed; if (_in.Right) _x += Speed;
        if (_in.Up)    _y -= Speed; if (_in.Down)  _y += Speed;
        _x = Math.Clamp(_x, 0, _wW - SplashKit.BitmapWidth(_bmp));
        _y = Math.Clamp(_y, 0, _wH - SplashKit.BitmapHeight(_bmp));
    }

    public void Draw() => SplashKit.DrawBitmap(_bmp, _x, _y);
    public Rectangle Hitbox => SplashKit.RectangleFrom(_x, _y, SplashKit.BitmapWidth(_bmp), SplashKit.BitmapHeight(_bmp));
}
```

---

## Step 6 — Obstacles, shards, and a spawner

The spawner randomly creates **car** obstacles (choosing one of four bitmaps) and slower **shards** (score pickups). Each object implements `IUpdatable`, `IRenderable`, `ICollidable`.

```csharp
// Systems/Spawner.cs (snippet)
private static readonly string[] CarNames = { "car_red", "car_yellow", "car_black", "car_blue" };
private void SpawnCar()
{
    string name = CarNames[_rng.Next(CarNames.Length)];
    var bmp = SplashKit.BitmapNamed(name);
    double w = SplashKit.BitmapWidth(bmp), h = SplashKit.BitmapHeight(bmp);
    double x = _rng.NextDouble() * Math.Max(1.0, _worldW - w);
    _obstacles.Add(new Obstacle(x, -h, _baseSpeed, _worldH, bmp));
}
```

---

## Step 7 — Collisions

Build a `CollisionSystem` to check **AABB** overlaps each frame. SplashKit provides **`RectangleFrom`** and **`RectanglesIntersect`** helpers so you don’t write your own geometry. ([SplashKit][6])

```csharp
// Systems/CollisionSystem.cs (snippet)
var p = _player.Hitbox;
foreach (var o in _spawner.Obstacles)
    if (SplashKit.RectanglesIntersect(p, o.Hitbox)) { PlayerCollided = true; return; }

for (int i = _spawner.Shards.Count - 1; i >= 0; i--)
{
    var s = _spawner.Shards[i];
    if (SplashKit.RectanglesIntersect(p, s.Hitbox))
    {
        _hud.AddScore(s.ScoreValue);
        if (SplashKit.HasSoundEffect("pickup")) SoundBank.PlaySfx("pickup");
        s.Kill();
    }
}
```

---

## Step 8 — HUD & drawing order

In your `Game.Draw()`:

1. **Clear** the back buffer
2. Draw background (neon lanes, grid)
3. Draw world (spawner, player)
4. Draw HUD (score, hints)

SplashKit exposes graphics primitives, text, and images; call **`ClearScreen`**, then draw lines/bitmaps/text, then **`Refresh`**. ([SplashKit][5])

---

## Step 9 — Audio (BGM & SFX)

Initialize audio once; then either:

* Play **SOUND** (WAV) in a loop for BGM
* Or use **MUSIC** (OGG) if you prefer streaming

Bundle lines and calls:

```txt
# main.txt
SOUND,bgm,crime_never_sleeps.wav
SOUND,pickup,pickup.wav
```

```csharp
// PlayState.Enter()
if (SplashKit.HasSoundEffect("bgm"))
    SoundBank.PlaySfx("bgm", volume: 0.8, times: 1000);
```

SplashKit’s **resource bundle** and **resources API** document how these names are resolved from `Resources/sounds`. ([SplashKit][7])

---

## Step 10 — Polishing touches

* **Fullscreen toggle**: `if (SplashKit.KeyTyped(KeyCode.F11Key)) window.ToggleFullscreen();`
* **Menu & GameOver text**: draw with `DrawText` after you’ve cleared the frame.
* **Best score**: persist a small JSON file and show “BEST: …”.

---

## Testing checklist

* Call **`ProcessEvents()` once per frame** before reading KeyDown/KeyTyped. ([SplashKit][2])
* Clear the back buffer each frame to avoid “smear” artifacts.
* Ensure **Resources/** sits next to your build output (bundles, images, sounds). ([SplashKit][3])
* Collisions verified via rectangles (bike vs car → GameOver; bike vs shard → +score). ([SplashKit][6])

---

## Packaging (optional, if you want to share builds)

You can produce a **single-file, self-contained** Windows build:

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishSingleFile=true \
  -p:SelfContained=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -o publish/win-x64
```

Single-file/self-contained is a supported .NET deployment model; the `IncludeNativeLibrariesForSelfExtract` property helps bundle native DLLs (SDL/SplashKit) into the single file for extraction at runtime. ([Microsoft Learn][8])

> Tip: when you share to classmates, **zip the whole publish folder** (EXE + Resources) to avoid missing assets.

---

## Attribution template

Create `ATTRIBUTIONS.txt`:

```
Music: "Crime Never Sleeps" — Darren Curtis (credit per author’s page)
Car/motorcycle images: your source and license here
Built with SplashKit (https://splashkit.io/)
```

---

## What to submit (for Task 7.4H)

* **This Markdown tutorial** (as a PDF or MD file)
* **Source code zip** (no `bin/`/`obj/`; include `Resources/`)
* **Short gameplay video** (60–90s) and 2–4 screenshots
* **Attributions.txt**

These items directly satisfy the brief’s request to “Provide a tutorial on the use of SplashKit to accomplish a task” and demonstrate excellent achievement of the learning outcomes through syntax quality, concept depth, and development process.&#x20;

---

## Appendix — minimal code links (by topic)

* **Bundles & resources**: how `main.txt` is structured and loaded. ([SplashKit][4])
* **Input**: `ProcessEvents`, `KeyDown`, `KeyTyped`. ([SplashKit][2])
* **Graphics**: drawing text, lines, bitmaps; window basics. ([SplashKit][5])
* **Geometry**: rectangles and intersection. ([SplashKit][6])
* **Getting started & guides**: SplashKit tutorials index. ([SplashKit][1])
* **.NET single-file publish** (optional deploy). ([Microsoft Learn][8])

---

### Where to go next

* Animate sprites (use `ANIM` bundles & sprite sheets). ([SplashKit][4])
* Add pause state, difficulty curves, and score multipliers.
* Explore networking or a tiny web API with SplashKit’s server utilities. ([SplashKit][1])

Happy drifting!

[1]: https://splashkit.io/guides/?utm_source=chatgpt.com "Tutorials and Guides - SplashKit"
[2]: https://splashkit.io/api/input/?utm_source=chatgpt.com "Input - SplashKit"
[3]: https://splashkit.io/api/resources/?utm_source=chatgpt.com "Resources - SplashKit"
[4]: https://splashkit.io/guides/resources/loading-resources-with-bundles/?utm_source=chatgpt.com "Loading Resources with Bundles | SplashKit"
[5]: https://splashkit.io/api/graphics/?utm_source=chatgpt.com "Graphics - SplashKit"
[6]: https://splashkit.io/api/geometry/?utm_source=chatgpt.com "Geometry - SplashKit"
[7]: https://splashkit.io/api/resource-bundles/?utm_source=chatgpt.com "Resource Bundles - SplashKit"
[8]: https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview?utm_source=chatgpt.com "Create a single file for application deployment - .NET"
