public class Death : EnemyStates
{
    public Death(FlockingTest enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.StopMoving();
    }

    public override void Exit() { }

    public override void Update() { }
}
