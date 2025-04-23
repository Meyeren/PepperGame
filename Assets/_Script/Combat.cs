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
            Debug.Log("atta");
            isAttacking = true;
            animator.SetBool("isAttacking", true);
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("Sge");
        Collider[] hitEnemies = Physics.OverlapSphere(sword.transform.position, attackRange, enemyLayer);
        Invoke("EndAttack", sAnimationLength);
        foreach (Collider enemy in hitEnemies)
        {
            Destroy(enemy.gameObject);
            
        }
        
    }

    void EndAttack()
    {
        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    /*void OnDrawGizmo()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(sword.transform.position, attackRange);
    }*/
}
