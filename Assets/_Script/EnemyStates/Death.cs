using System.Collections;
using UnityEngine;

public class Death : EnemyStates
{
    public Death(FlockingTest enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        enemy.StopMoving();
        enemy.GetComponent<Collider>().enabled = false;
        enemy.StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        enemy.StartDeathAnimation();
        yield return new WaitForSeconds(1f); 
        enemy.EnemyDeath();
    }
}
