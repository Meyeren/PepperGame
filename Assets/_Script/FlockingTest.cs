using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlockingTest : MonoBehaviour
{
    [Header("Enemy Flocking Settings")]
    public float neighborRadius = 5f;
    public float separationDistance = 2f;

    public float separationWeight = 1.5f;
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 1.0f;

    public float targetWeight = 0.5f;
    public float randomMovementWeight = 0.2f;
    public float avoidanceWeight = 2f; 
    public LayerMask obstacleMask;

    [Header("Enemy Behavior Settings")]
    public float enemySpeed = 5f;
    public int enemyDamage = 1;
    public float enemyAttackRange = 2f;

    [Header("State Settings")]
    public float chaseTransitionTime = 1f;
    public float attackCooldown = 1.5f;

    [Header("Flocking Variations")]
    public float chaseFlockingRatio = 0.7f;

    private float saveSpeed;
    public Transform target;

    Rigidbody rb;
    public StateSwitcher StateMachine { get; private set; }

    EnemyWaves waveManager;

    Animator animator;

    Color originalColor;

    Renderer playerMat;

    void Start()
    {
        saveSpeed = enemySpeed;
        StateMachine = new StateSwitcher();
        StateMachine.ChangeState(new Idle(this));
        target = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody>();

        waveManager = GameObject.Find("WaveManager").GetComponent<EnemyWaves>();

        animator = GetComponentInChildren<Animator>();

        playerMat = target.GetComponentInChildren<SkinnedMeshRenderer>();
        originalColor = target.GetComponentInChildren<SkinnedMeshRenderer>().material.color;


        
    }

    void Update()
    {
        StateMachine.Update(); 
    }

    public void ApplyFlocking(float flockingIntensity = 1f)
    {
        enemySpeed = saveSpeed;

        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 avoidance = Vector3.zero;

        int neighborCount = 0;
        int obstacleCount = 0;

        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius);

        if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out RaycastHit hit, 3f, obstacleMask))
        {
            avoidance += hit.normal * avoidanceWeight;
            obstacleCount++;
        }

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject || !neighbor.CompareTag(gameObject.tag)) continue;

            Vector3 toNeighbor = transform.position - neighbor.transform.position;
            float distance = toNeighbor.magnitude;

            if (distance < separationDistance)
            {
                separation += toNeighbor.normalized / Mathf.Max(distance, 0.1f);
            }

            FlockingTest other = neighbor.GetComponent<FlockingTest>();
            if (other != null)
            {
                alignment += other.transform.forward;
                cohesion += other.transform.position;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            alignment = (alignment / neighborCount).normalized;
            cohesion = ((cohesion / neighborCount) - transform.position).normalized;
        }

        Vector3 toTarget = target != null ? (target.position - transform.position).normalized * (1f - flockingIntensity) : Vector3.zero;

        Vector3 randomMovement = new Vector3(
        Random.Range(-1f, 1f),
        0,
        Random.Range(-1f, 1f)).normalized * randomMovementWeight;

        Vector3 moveDir = (separation.normalized * separationWeight + alignment * alignmentWeight + cohesion * cohesionWeight + toTarget * targetWeight + randomMovement + avoidance).normalized;

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        transform.position += transform.forward * enemySpeed * Time.deltaTime;
    }

    public bool IsPlayerNearby()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) < 400f; 
    }

    public bool IsInAttackRange()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) < enemyAttackRange; 
    }

    public void DoAttack()
    {
        if (IsInAttackRange())
        {
            animator.SetBool("isAttacking", true);
            if (!target.GetComponent<Combat>().isInvulnerable)
            {
                if (target.GetComponent<Combat>().hasDamageReduction)
                {
                    target.GetComponent<Combat>().playerHealth -= enemyDamage * target.GetComponent<Combat>().damageReduction;
                    target.GetComponent<Combat>().DamageVibration();
                    playerMat.material.color = Color.blue;
                    Invoke("EndColor", 0.1f);
                   
                    
                }
                else
                {
                    target.GetComponent<Combat>().playerHealth -= enemyDamage;
                    target.GetComponent<Combat>().DamageVibration();
                    playerMat.material.color = Color.red;
                    Invoke("EndColor", 0.1f);
                    
                }

            }
            else if (target.GetComponent<Combat>().isInvulnerable)
            {
                playerMat.material.color = new Color(0.6f, 0.2f, 0.8f);
                target.GetComponent<Combat>().DamageVibration();
                Invoke("EndColor", 0.1f);
                
            }
        }
        
        
            
    }

    public void StopMoving()
    {
        enemySpeed = 0f;
    }

    public void EnemyDeath()
    {
        
        
        if (waveManager != null)
        {
            waveManager.OnEnemyKilled();
        }

        Destroy(gameObject);
    }

    public void KnockBack(Vector3 attackerPosition, float knockBackAmount)
    {
        Vector3 direction = (transform.position - attackerPosition).normalized;
        direction.y = 0f;

        rb.AddForce(direction * knockBackAmount, ForceMode.Impulse);
        Invoke("ResetKnockBack",0.5f);
        
    }

    void ResetKnockBack()
    {
        rb.linearVelocity = new Vector3(0f, 0f, 0f);
    }

    public void EndAttackAnimation()
    {
        animator.SetBool("isAttacking", false);
    }

    public void StartDeathAnimation()
    {
        animator.SetTrigger("Death");
    }

    void EndColor()
    {
        playerMat.material.color = originalColor;
        
    }
}
