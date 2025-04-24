using UnityEngine;

public abstract class EnemyStates
{
    protected FlockingTest enemy;
    protected float stateTime;

    public EnemyStates(FlockingTest enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter()
    {
        stateTime = 0f;
    }

    public virtual void Exit() { }

    public virtual void Update()
    {
        stateTime += Time.deltaTime;
    }

    public virtual void FixedUpdate() { }

    public virtual void OnCollisionEnter(Collision collision) { }
}