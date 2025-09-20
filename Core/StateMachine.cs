namespace NeonDrift;


public interface IGameState : IDisposable
{
    void Enter();
    void HandleInput();
    void Update();
    void Draw();
    void Exit();
}


public sealed class StateMachine : IDisposable
{
    private IGameState? _current;


    public IGameState? Current => _current;


    public void ChangeState(IGameState next)
    {
        _current?.Exit();
        _current?.Dispose();
        _current = next;
        _current.Enter();
    }


    public void HandleInput() => _current?.HandleInput();
    public void Update() => _current?.Update();
    public void Draw() => _current?.Draw();


    public void Dispose()
    {
        _current?.Exit();
        _current?.Dispose();
    }
}