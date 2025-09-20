// File: Core/Game.cs
using System;
using SplashKitSDK;

namespace NeonDrift
{
    public sealed class Game : IDisposable
    {
        private readonly Window _window;
        private readonly StateMachine _states;

        public Game(Window window)
        {
            _window = window;
            _states = new StateMachine();
            _states.ChangeState(new MenuState(this));   // start in Menu
        }

        public Window Window => _window;

        // Read by Program.cs to exit cleanly without double-closing the window
        public bool QuitRequested { get; private set; }
        public void RequestQuit() => QuitRequested = true;

        public void HandleInput() => _states.HandleInput();
        public void Update()      => _states.Update();

        public void Draw()
        {
            if (_window.CloseRequested) return; // safety

            // >>> this was missing — without it, frames accumulate
            SplashKit.ClearScreen(Color.Black);  // clear the back buffer each frame
            // <<<

            _states.Draw();
        }

        // State transitions
        public void StartGame()              => _states.ChangeState(new PlayState(this));
        public void ShowMenu()               => _states.ChangeState(new MenuState(this));
        public void GameOver()               => _states.ChangeState(new GameOverState(this, finalScore: 0));
        public void GameOver(int finalScore) => _states.ChangeState(new GameOverState(this, finalScore));

        public void Dispose() => _states.Dispose();
    }
}
