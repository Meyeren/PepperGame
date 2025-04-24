using System.Collections;
using UnityEngine;

public class Flock : EnemyStates
{
    public Flock(FlockingTest enemy) : base(enemy) { }

    public override void Update()
    {
        base.Update();

        enemy.ApplyFlocking(1f); 

        if (enemy.IsPlayerNearby() && stateTime > 1f) 
        {
            enemy.StateMachine.ChangeState(new Chase(enemy));
        }
    }
}

