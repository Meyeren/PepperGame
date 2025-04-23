using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{
    public int playerHealth = 100;

    Slider Health;

    Animator animator;
    PlayerInput input;
    InputAction attackAction;

    GameObject player;
    GameObject sword;
    
    bool isAttacking;
    [SerializeField] float attackRange = 5f;
    [SerializeField] int basicDamage = 50;
    [SerializeField] float capsuleHeight = 5f;

    LayerMask enemyLayer;

    [SerializeField] float sAnimationLength = 1.0f;



    private void Start()
    {
        isAttacking = false;

        player = GameObject.FindGameObjectWithTag("Player");
        sword = GameObject.FindGameObjectWithTag("Sword");
        animator = player.GetComponent<Animator>();

        Health = GameObject.Find("Healthbar").GetComponent<Slider>();
        Health.maxValue = playerHealth;

        input = player.GetComponent<PlayerInput>();
        attackAction = input.actions.FindAction("Attack");

        enemyLayer = LayerMask.GetMask("Enemy");
        
    }

    private void Update()
    {
        Health.value = playerHealth;

        if (attackAction.triggered && player.GetComponent<PlayerMovement>().IsGrounded() && !isAttacking)
        {
            Attack();
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("isAttacking");
        Invoke("EndAttack", sAnimationLength);
        Vector3 capsuleStart = sword.transform.position;
        Vector3 capsuleEnd = capsuleStart + sword.transform.position * capsuleHeight;
        Collider[] hitEnemies = Physics.OverlapCapsule(capsuleStart, capsuleEnd, attackRange, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(basicDamage);
        }
       
        
        
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        
            // Definer kapselens start og slutposition
            Vector3 capsuleStart = sword.transform.position;
            Vector3 capsuleEnd = capsuleStart + sword.transform.forward * attackRange; // Justér længden af kapslen
            float capsuleRadius = attackRange;

            // Indstil farven for Gizmoen
            Gizmos.color = Color.red;

            // Tegn de to ender som små kugler (capsuleEnd og capsuleStart)
            Gizmos.DrawWireSphere(capsuleStart, capsuleRadius);  // Startposition
            Gizmos.DrawWireSphere(capsuleEnd, capsuleRadius);    // Slutposition

            // Tegn forbindelseslinjen mellem de to kugler for at simulere kapselens "krop"
            Gizmos.DrawLine(capsuleStart, capsuleEnd);
        
    }
}
