using System.Collections;
using UnityEngine;

public class Patrol : EnemyStates
{
    private Vector3 patrolPoint;
    private float repathTimer;

    public Patrol(FlockingTest enemy) : base(enemy)
    {
        GetNewPatrolPoint();
    }

    private void GetNewPatrolPoint()
    {
        patrolPoint = enemy.transform.position +
                     new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        repathTimer = Random.Range(3f, 8f);
    }

    public override void Update()
    {
        base.Update();

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0)
        {
            GetNewPatrolPoint();
        }

        if (enemy.IsPlayerNearby())
        {
            enemy.StateMachine.ChangeState(new Flock(enemy));
            return;
        }

        Vector3 direction = (patrolPoint - enemy.transform.position).normalized;
        enemy.transform.position += direction * enemy.enemySpeed * 0.3f * Time.deltaTime;
        enemy.transform.forward = Vector3.Lerp(
            enemy.transform.forward,
            direction,
            Time.deltaTime * 3f
        );
    }
}
