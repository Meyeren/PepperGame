using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Combat : MonoBehaviour
{
    [Header("Player")]
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

    PlayerClass playerClass;

    GameObject sword;

    [Header("Attacking")]
    public bool isAttacking;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isDashing;
    public bool isGroundSlamming;
    [SerializeField] float attackRange = 5f;

    public bool hasDamageReduction;
    public bool attackWhileDash;
    public bool hasGroundSlam;
    public bool isInvulnerable;
    public bool hasInvulnerableAbility;

    float Stamina;
    public float damageReduction = 0.5f;

    float invulAbilityCost = 100f;

    public float basicDamage = 50f;
    public float dashAttackDamage = 50f;

    [SerializeField] float specialAttackRange = 5f;
    [SerializeField] int specialDamage = 100;
    [SerializeField] float knockBackAmount = 5f;
    [SerializeField] float speedReduction = 5f;
    [SerializeField] float specialAttackCost = 100f;
    [SerializeField] float basicKnockbackAmount = 2f;

    [SerializeField] float lifeStealAmount = 0.5f;

    LayerMask enemyLayer;

    [Header("Audio Clips")]
    public AudioClip swingSoundClip;
    public AudioClip hitEnemySoundClip;
    public AudioClip specialAttackSoundClip;
    public AudioClip playerHurtSoundClip;

    [Header("Audio Delays (seconds)")]
    public float swingSoundDelay = 0f;  // Delay for swing sound
    public float hitEnemySoundDelay = 0f;  // Delay for hit enemy sound
    public float specialAttackSoundDelay = 0f;  // Delay for special attack sound
    public float playerHurtSoundDelay = 0f;  // Delay for player hurt sound

    private AudioSource audioSource;

    [Header("VFX Prefabs")]
    public GameObject hitEnemyEffectPrefab;
    public GameObject playerHurtEffectPrefab;

    SkinnedMeshRenderer[] renderers;
    Color[] originalColors;

    private void Start()
    {
        isAttacking = false;
        attackWhileDash = false;
        isInvulnerable = false;
        isGroundSlamming = false;
        hasGroundSlam = false;
        hasInvulnerableAbility = false;

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

        playerClass = GetComponent<PlayerClass>();

        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }

        audioSource = GetComponent<AudioSource>();  // Initialize the AudioSource
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

        if (playerHealth >= MaxPlayerHealth && !isInvulnerable && !hasDamageReduction)
        {
            FadeOutHealth();
        }
        else
        {
            healthCanvas.alpha = 1f;
        }

        if (specialAttackAction.triggered && isGrounded && !isAttacking && !isGroundSlamming && Stamina >= specialAttackCost && hasGroundSlam)
        {
            SpecialAttack();
        }

        if (specialAttackAction.triggered && Stamina >= invulAbilityCost && hasInvulnerableAbility)
        {
            isInvulnerable = true;
            Invoke("EndInvul", 2f);
            GetComponent<PlayerMovement>().Stamina -= invulAbilityCost;
            GetComponent<PlayerMovement>().noStaminaRegen = true;
        }

        if (playerHealth <= 0)
        {
            SceneManager.LoadScene("Hubben");
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("isAttacking");
        PlaySound(swingSoundClip, swingSoundDelay);  // Play the swing sound with delay
    }

    void PerformHit()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(sword.transform.position, attackRange, enemyLayer);
        if (hitEnemies.Length != 0)
        {
            if (Gamepad.current != null)
            {
                Gamepad.current.SetMotorSpeeds(0.1f, 0.2f);
                Invoke("EndVibration", 0.2f);
            }
            StartCoroutine(cam.GetComponent<CameraShake>().Shake(0.15f, 0.1f));
            PlaySound(hitEnemySoundClip, hitEnemySoundDelay);  // Play the hit enemy sound with delay
            foreach (Collider enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyHealth>().TakeDamage(basicDamage);
                enemy.GetComponent<FlockingTest>().KnockBack(transform.position, basicKnockbackAmount);

                if (playerClass.hasLifeSteal && playerHealth <= playerClass.lifeStealHealh)
                {
                    playerHealth += lifeStealAmount;
                }
                else if (playerHealth > playerClass.lifeStealHealh && playerClass.hasLifeSteal)
                {
                    playerHealth = playerClass.lifeStealHealh;
                }

                if (hitEnemyEffectPrefab)
                {
                    Instantiate(hitEnemyEffectPrefab, enemy.transform.position, Quaternion.identity);
                }
                StartCoroutine(FlashRed(enemy));
            }
        }
        isAttacking = false;
    }

    void SpecialAttack()
    {
        hasDamageReduction = true;
        isGroundSlamming = true;
        animator.SetTrigger("isGroundSlamming");
        GetComponent<PlayerMovement>().Speed -= speedReduction;
        GetComponent<PlayerMovement>().Stamina -= 100f;
        GetComponent<PlayerMovement>().noStaminaRegen = true;

        PlaySound(specialAttackSoundClip, specialAttackSoundDelay);  // Play the special attack sound with delay
    }

    void PerformSpecialAttackHit()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0.5f, 1f);
        }

        StartCoroutine(cam.GetComponent<CameraShake>().Shake(0.3f, 1f));

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, specialAttackRange, enemyLayer);
        if (hitEnemies.Length >= 8)
        {
            GetComponent<PlayerMovement>().Stamina += 25f;
        }
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(specialDamage);
            enemy.GetComponent<FlockingTest>().KnockBack(transform.position, knockBackAmount);

            if (hitEnemyEffectPrefab)
            {
                Instantiate(hitEnemyEffectPrefab, enemy.transform.position, Quaternion.identity);
            }
            StartCoroutine(FlashRed(enemy));
        }
    }

    public void TakeDamage(float damage)
    {
        playerHealth -= damage;

        PlaySound(playerHurtSoundClip, playerHurtSoundDelay);  // Play the player hurt sound with delay
        if (playerHurtEffectPrefab)
        {
            Instantiate(playerHurtEffectPrefab, transform.position, Quaternion.identity);
        }

        // Start the red flash effect on player when taking damage
        StartCoroutine(FlashRed());

        if (playerHealth <= 0f)
        {
            // Death logic
        }
    }

    IEnumerator FlashRed(Collider enemy = null)
    {
        // If enemy is null, apply to player
        if (enemy == null)
        {
            var playerRenderer = GetComponent<SkinnedMeshRenderer>();
            Color flameRed = new Color(1f, 0f, 0f, 1f); // Bright red
            if (playerRenderer)
            {
                playerRenderer.material.color = flameRed; // Change to flame red
            }

            yield return new WaitForSeconds(0.2f);  // Wait for 0.2 seconds with the flame red color

            // After the wait, return to the original color
            if (playerRenderer)
            {
                playerRenderer.material.color = originalColors[0]; // Return to the original color
            }
        }
        else
        {
            var enemyRenderer = enemy.GetComponent<SkinnedMeshRenderer>();
            if (enemyRenderer)
            {
                enemyRenderer.material.color = Color.red;
            }

            yield return new WaitForSeconds(0.2f);

            if (enemyRenderer)
            {
                enemyRenderer.material.color = originalColors[0]; // Adjust for multiple materials if needed
            }
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

    void EndInvul()
    {
        isInvulnerable = false;
        GetComponent<PlayerMovement>().noStaminaRegen = false;
    }

    // Helper method to play a sound with delay
    void PlaySound(AudioClip clip, float delay)
    {
        if (delay > 0f)  // Check if there is a delay
        {
            StartCoroutine(PlaySoundWithDelay(clip, delay));  // Start coroutine to play sound with delay
        }
        else
        {
            audioSource.PlayOneShot(clip);  // Play sound immediately if no delay
        }
    }

    // Coroutine to play sound with a specified delay
    IEnumerator PlaySoundWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for the specified delay
        audioSource.PlayOneShot(clip);  // Play the sound after the delay
    }
}
