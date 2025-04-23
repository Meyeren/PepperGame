using Unity.VisualScripting;

public class Attack : EnemyStates
{
    public Attack(FlockingTest enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.StopMoving();
    }

    public override void Exit() { }

    public override void Update()
    {
        enemy.DoAttack();

        if (!enemy.IsInAttackRange())
        {
            enemy.StateMachine.ChangeState(new Flock(enemy));
        }
    }
}

