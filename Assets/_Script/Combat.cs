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
            Debug.Log("Hit");
        }
       
        
        
    }

    void EndAttack()
    {
        isAttacking = false;
    }


}
