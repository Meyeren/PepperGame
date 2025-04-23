public class Death : EnemyStates
{
    public Death(FlockingTest enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.EnemyDeath();
    }

    public override void Exit() { }

    public override void Update() { }
}
