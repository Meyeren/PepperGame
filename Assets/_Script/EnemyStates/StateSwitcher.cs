public class StateSwitcher 
{
    public EnemyStates CurrentState { get; private set; }
    public EnemyStates PreviousState { get; private set; }

    public void ChangeState(EnemyStates newState)
    {
        PreviousState = CurrentState;
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }

    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }

    public void RevertToPreviousState()
    {
        if (PreviousState != null)
        {
            ChangeState(PreviousState);
        }
    }
}

