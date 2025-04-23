using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{
    public int playerHealth = 100;
    public int MaxPlayerHealth = 100;

    Slider healthSlider;
    CanvasGroup healthCanvas;

    Animator animator;
    PlayerInput input;
    InputAction attackAction;

    GameObject player;
    GameObject sword;
    
    bool isAttacking;
    [SerializeField] float attackRange = 5f;
    [SerializeField] int basicDamage = 50;

    LayerMask enemyLayer;




    private void Start()
    {
        isAttacking = false;

        

        player = GameObject.FindGameObjectWithTag("Player");
        sword = GameObject.FindGameObjectWithTag("Sword");
        animator = player.GetComponent<Animator>();

        healthSlider = GameObject.Find("Healthbar").GetComponent<Slider>();
        
        healthCanvas = healthSlider.GetComponent<CanvasGroup>();
        healthSlider.maxValue = MaxPlayerHealth;
        healthCanvas.alpha = 0f;

        input = player.GetComponent<PlayerInput>();
        attackAction = input.actions.FindAction("Attack");

        enemyLayer = LayerMask.GetMask("Enemy");
        
    }

    private void Update()
    {
        healthSlider.value = playerHealth;

        if (attackAction.triggered && player.GetComponent<PlayerMovement>().IsGrounded() && !isAttacking)
        {
            Attack();
        }

        if (playerHealth == MaxPlayerHealth)
        {
            FadeOutHealth();
        }
        else
        {
            healthCanvas.alpha = 1f;
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("isAttacking");
        
        
        
        
 
        
        
    }

    void PerformHit()
    {
        Debug.Log("Attack performed");
        Collider[] hitEnemies = Physics.OverlapSphere(sword.transform.position, attackRange, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(basicDamage);
            
        }
        isAttacking = false;
    }

    void FadeOutHealth()
    {
        if (healthCanvas.alpha > 0f)
        {
            healthCanvas.alpha -= Time.deltaTime * 0.5f;
        }
    }

}
