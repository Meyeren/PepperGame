public abstract class EnemyStates
{
    protected FlockingTest enemy;

    public EnemyStates(FlockingTest enemy)
    {
        this.enemy = enemy;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
}