using UnityEngine;

public class FlockingTest : MonoBehaviour
{
    public float neighborRadius = 5f;
    public float separationDistance = 2f;
    public float enemySpeed = 5f;
    public float saveSpeed;

    public float separationWeight = 1.5f;
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 1.0f;

    public Transform target;

    public StateSwitcher StateMachine { get; private set; }

    void Start()
    {
        StateMachine = new StateSwitcher();
        StateMachine.ChangeState(new Idle(this));
    }

    void Update()
    {
        StateMachine.Update(); 
    }

    public void ApplyFlocking()
    {
        enemySpeed = 5f;

        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        int neighborCount = 0;

        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius);

        foreach (var hit in neighbors)
        {
            if (hit.gameObject == gameObject || !hit.CompareTag(gameObject.tag)) continue;

            Vector3 toNeighbor = transform.position - hit.transform.position;
            float distance = toNeighbor.magnitude;

            if (distance < separationDistance)
            {
                separation += toNeighbor.normalized / distance;
            }

            FlockingTest other = hit.GetComponent<FlockingTest>();
            if (other != null)
            {
                alignment += other.transform.forward;
                cohesion += other.transform.position;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            alignment /= neighborCount;
            cohesion = (cohesion / neighborCount - transform.position);
        }

        Vector3 toTarget = (target.position - transform.position).normalized;

        Vector3 moveDir =
              separation * separationWeight
            + alignment.normalized * alignmentWeight
            + cohesion.normalized * cohesionWeight
            + toTarget * 0.5f;

        transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * 5f);
        transform.position += transform.forward * enemySpeed * Time.deltaTime;
    }

    public bool IsPlayerNearby()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) < 20f; 
    }

    public bool IsInAttackRange()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) < 2f; 
    }

    public void DoAttack()
    {
        Debug.Log("attacks");
    }

    public void StopMoving()
    {
        enemySpeed = 0f;
    }

    public void EnemyDeath()
    {
        Destroy(gameObject);
    }
}
