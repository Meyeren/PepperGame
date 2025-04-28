using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class Combat : MonoBehaviour
{
    public float playerHealth = 100f;
    public float MaxPlayerHealth = 100f;

    Slider healthSlider;
    CanvasGroup healthCanvas;
    Image fillImage;
    Image fillImage2;

    Camera cam;
    Animator animator;
    PlayerInput input;
    InputAction attackAction;
    InputAction specialAttackAction;
    GameObject sword;

    public bool isAttacking;
    bool isGrounded;
    bool isDashing;
    public bool isGroundSlamming;
    public float damageReduction = 0.5f;
    public bool hasDamageReduction;

    float Stamina;

    public bool attackWhileDash;
    public bool isInvulnerable;

    public int basicDamage = 50;
    public int dashAttackDamage = 50;

    [SerializeField] float attackRange = 5f;
    [SerializeField] float specialAttackRange = 5f;
    [SerializeField] int specialDamage = 100;
    [SerializeField] float knockBackAmount = 5f;
    [SerializeField] float speedReduction = 5f;
    [SerializeField] float specialAttackCost = 100f;
    [SerializeField] float basicKnockbackAmount = 2f;

    LayerMask enemyLayer;

    // Audio
    public AudioClip attackMissClip;
    public AudioClip attackHitClip;
    public AudioClip playerHurtClip;
    public AudioClip enemyHurtClip;
    public AudioClip specialAttackClip;
    private AudioSource audioSource;

    [Header("Sound Delays")]
    [SerializeField] private float attackMissDelay = 0f;
    [SerializeField] private float attackHitDelay = 0f;
    [SerializeField] private float playerHurtDelay = 0f;
    [SerializeField] private float enemyHurtDelay = 0f;
    [SerializeField] private float specialAttackDelay = 0f;

    // Animation triggers
    [SerializeField] private string playerHurtTrigger = "Hurt";
    [SerializeField] private string enemyHurtTrigger = "Hurt";
    [SerializeField] private string specialAbilityTrigger = "SpecialAbility";

    // Health flash
    [SerializeField] private float flashDuration = 0.1f;

    private void Start()
    {
        isAttacking = false;
        attackWhileDash = false;
        isInvulnerable = false;
        isGroundSlamming = false;

        sword = GameObject.FindGameObjectWithTag("Sword");
        animator = GetComponent<Animator>();

        healthSlider = GameObject.Find("Healthbar").GetComponent<Slider>();
        fillImage = healthSlider.fillRect.GetComponentInChildren<Image>();
        fillImage2 = healthSlider.GetComponentInChildren<Image>();

        healthCanvas = healthSlider.GetComponent<CanvasGroup>();
        healthSlider.maxValue = MaxPlayerHealth;
        healthCanvas.alpha = 0f;

        input = GetComponent<PlayerInput>();
        attackAction = input.actions.FindAction("Attack");
        specialAttackAction = input.actions.FindAction("SpecialAttack");

        enemyLayer = LayerMask.GetMask("Enemy");

        cam = Camera.main;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (isInvulnerable)
        {
            fillImage.color = new Color(0.6f, 0.2f, 0.8f, 1f);
            fillImage2.color = new Color(0.6f, 0.2f, 0.8f, 1f);
        }
        else if (hasDamageReduction)
        {
            fillImage2.color = new Color(0.1f, 0.2f, 0.5f, 0.7f);
            fillImage.color = new Color(0.3f, 0.6f, 0.9f);
        }
        else
        {
            fillImage2.color = new Color(0.1f, 0.35f, 0.15f, 0.5f);
            fillImage.color = new Color(0.25f, 0.7f, 0.3f);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            attackWhileDash = true;
        }
        isGrounded = GetComponent<PlayerMovement>().IsGrounded();
        isDashing = GetComponent<PlayerMovement>().isDashing;
        healthSlider.value = playerHealth;
        Stamina = GetComponent<PlayerMovement>().Stamina;

        if (attackAction.triggered && isGrounded && !isAttacking && !isGroundSlamming)
        {
            if (!isDashing)
            {
                Attack();
            }
        }

        if (playerHealth == MaxPlayerHealth && !isInvulnerable && !hasDamageReduction)
        {
            FadeOutHealth();
        }
        else
        {
            healthCanvas.alpha = 1f;
        }

        if (specialAttackAction.triggered && isGrounded && !isAttacking && !isGroundSlamming && Stamina >= specialAttackCost)
        {
            SpecialAttack();
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("isAttacking");
    }

    void PerformHit()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(sword.transform.position, attackRange, enemyLayer);

        if (hitEnemies.Length != 0)
        {
            PlaySoundWithDelay(attackHitClip, attackHitDelay);

            if (Gamepad.current != null)
            {
                Gamepad.current.SetMotorSpeeds(0.1f, 0.2f);
                StartCoroutine(cam.GetComponent<CameraShake>().Shake(0.15f, 0.1f));
                Invoke("EndVibration", 0.2f);
            }

            foreach (Collider enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyHealth>().TakeDamage(basicDamage);
                enemy.GetComponent<FlockingTest>().KnockBack(transform.position, basicKnockbackAmount);

                Animator enemyAnimator = enemy.GetComponent<Animator>();
                if (enemyAnimator != null)
                {
                    enemyAnimator.SetTrigger(enemyHurtTrigger);
                }

                StartCoroutine(FlashHealth(enemy.GetComponentInChildren<Slider>()));
            }
        }
        else
        {
            PlaySoundWithDelay(attackMissClip, attackMissDelay);
        }

        isAttacking = false;
    }

    void SpecialAttack()
    {
        hasDamageReduction = true;
        isGroundSlamming = true;
        animator.SetTrigger("isGroundSlamming");
        animator.SetTrigger(specialAbilityTrigger);
        PlaySoundWithDelay(specialAttackClip, specialAttackDelay);

        GetComponent<PlayerMovement>().Speed -= speedReduction;
        GetComponent<PlayerMovement>().Stamina -= 100f;
        GetComponent<PlayerMovement>().noStaminaRegen = true;
    }

    void PerformSpecialAttackHit()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0.5f, 1f);
        }
        StartCoroutine(cam.GetComponent<CameraShake>().Shake(0.3f, 1f));

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, specialAttackRange, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(specialDamage);
            enemy.GetComponent<FlockingTest>().KnockBack(transform.position, knockBackAmount);

            Animator enemyAnimator = enemy.GetComponent<Animator>();
            if (enemyAnimator != null)
            {
                enemyAnimator.SetTrigger(enemyHurtTrigger);
            }

            StartCoroutine(FlashHealth(enemy.GetComponentInChildren<Slider>()));
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        playerHealth -= damage;
        PlaySoundWithDelay(playerHurtClip, playerHurtDelay);

        if (animator != null)
        {
            animator.SetTrigger(playerHurtTrigger);
        }

        StartCoroutine(FlashHealth(healthSlider));

        if (playerHealth <= 0)
        {
            Die();
        }
    }

    void FadeOutHealth()
    {
        if (healthCanvas.alpha > 0f)
        {
            healthCanvas.alpha -= Time.deltaTime * 0.5f;
        }
    }

    void EndSlam()
    {
        hasDamageReduction = false;
        isGroundSlamming = false;
        GetComponent<PlayerMovement>().noStaminaRegen = false;
        GetComponent<PlayerMovement>().Speed += speedReduction;

        if (Gamepad.current != null)
        {
            EndVibration();
        }
    }

    void EndVibration()
    {
        Gamepad.current.SetMotorSpeeds(0f, 0f);
    }

    void PlaySoundWithDelay(AudioClip clip, float delay)
    {
        if (clip == null) return;
        StartCoroutine(PlayDelayed(clip, delay));
    }

    IEnumerator PlayDelayed(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(clip);
    }

    IEnumerator FlashHealth(Slider healthbar)
    {
        if (healthbar == null) yield break;

        Image fill = healthbar.fillRect.GetComponentInChildren<Image>();
        Color originalColor = fill.color;

        fill.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        fill.color = originalColor;
    }

    void Die()
    {
        Debug.Log("Player is dead.");
        // TODO: Tilføj død-animation og logik
    }
}
