public class Flock : EnemyStates
{
    public Flock(FlockingTest enemy) : base(enemy) { }

    public override void Enter() { }

    public override void Exit() { }

    public override void Update()
    {
        enemy.ApplyFlocking();

        if (enemy.IsInAttackRange())
        {
            enemy.StateMachine.ChangeState(new Attack(enemy));
        }
    }
}

