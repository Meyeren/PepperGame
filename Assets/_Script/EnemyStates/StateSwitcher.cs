public class StateSwitcher 
{
    public EnemyStates CurrentState { get; private set; }

    public void ChangeState(EnemyStates newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}

