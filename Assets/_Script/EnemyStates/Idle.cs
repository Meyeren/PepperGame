public class Idle : EnemyStates
{
    public Idle(FlockingTest enemy) : base(enemy) { }

    public override void Enter() { }

    public override void Exit() { }

    public override void Update()
    {
        if (enemy.IsPlayerNearby())
        {
            enemy.StateMachine.ChangeState(new Flock(enemy));
        }
    }
}
