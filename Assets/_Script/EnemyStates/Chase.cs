using System.Collections;
using UnityEngine;

public class Chase : EnemyStates
{
    public Chase(FlockingTest enemy) : base(enemy) { }

    public override void Update()
    {
        base.Update();

        if (enemy.IsInAttackRange())
        {
            enemy.StateMachine.ChangeState(new Attack(enemy));
            return;
        }

        enemy.ApplyFlocking(0.7f);

        if (enemy.target != null)
        {
            Vector3 directChaseDir = (enemy.target.position - enemy.transform.position).normalized;
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                Quaternion.LookRotation(directChaseDir),
                Time.deltaTime * 3f
            );
        }
    }
}
