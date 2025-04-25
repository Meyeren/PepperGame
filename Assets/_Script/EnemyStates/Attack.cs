using UnityEngine;

public class Attack : EnemyStates
{
    private float attackCooldown = 1f;
    private float lastAttackTime;

    Animator animator;

    public Attack(FlockingTest enemy) : base(enemy) { }

    public override void Enter()
    {
        
        base.Enter();
        enemy.StopMoving();
    }
    public override void Update()
    {
        base.Update();

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            enemy.DoAttack();
            lastAttackTime = Time.time;
        }

        if (!enemy.IsInAttackRange())
        {
            enemy.StateMachine.ChangeState(new Chase(enemy));
            enemy.EndAttackAnimation();
        }
    }
}

