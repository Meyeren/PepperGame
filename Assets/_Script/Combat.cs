using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class Combat : MonoBehaviour
{
    Slider healthSlider;
    CanvasGroup healthCanvas;
    Image fillImage;
    Image fillImage2;

    public GameObject DeathScreen;
    [SerializeField] TextMeshProUGUI KillCountText;
    [SerializeField] TextMeshProUGUI SkillPointText;
    [SerializeField] GameObject ContinueButton;
    public bool isControl;
    

    Camera cam;
    Animator animator;
    PlayerInput input;
    InputAction attackAction;
    InputAction specialAttackAction;
    InputAction testAction;

    PlayerClass playerClass;
    GameObject sword;

    public GameObject Waves;

    SkinnedMeshRenderer[] renderers;
    Color[] originalColors;

    float Stamina;


    float invulAbilityCost = 100f;

    skillTreeManager skillManager;

    [SerializeField]int nextSkillThreshhold = 50;
    
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;

    [Header("Player")]
    public float playerHealth = 100f;
    public float MaxPlayerHealth = 100f;
    public int killCount;
    

    [Header("Attack Values")]
    [SerializeField] float attackRange = 5f;
    public float basicDamage = 50f;
    [SerializeField] float baseAttackMoveRed = 2f;

    public float damageReduction = 0.5f;
    
    public float dashAttackDamage = 50f;

    [Header("Bool Checks")]
    public bool isAttacking;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isDashing;
    public bool isGroundSlamming;
    public bool hasDamageReduction;
    public bool isInvulnerable;

    [Header("Abilites")]
    public bool attackWhileDash;
    public bool hasGroundSlam;
    public bool hasInvulnerableAbility;


    [Header("SpecialAttacks")]
    [SerializeField] float specialAttackRange = 5f;
    [SerializeField] int specialDamage = 100;
    [SerializeField] float knockBackAmount = 5f;
    [SerializeField] float speedReduction = 5f;
    [SerializeField] float specialAttackCost = 100f;

    [SerializeField] float lifeStealAmount = 0.5f;

    LayerMask enemyLayer;

    [Header("Audio Clips")]
    public AudioClip swingSoundClip;
    public AudioClip hitEnemySoundClip;
    public AudioClip specialAttackSoundClip;
    public AudioClip specialAttackHitSoundClip;
    public AudioClip playerHurtSoundClip;

    [Header("Audio Delays (seconds)")]
    public float swingSoundDelay = 0f;
    public float hitEnemySoundDelay = 0f;
    public float specialAttackSoundDelay = 0f;
    public float specialAttackHitSoundDelay = 0f;
    public float playerHurtSoundDelay = 0f;

    private AudioSource audioSource;

    [Header("VFX Prefabs")]
    public GameObject hitEnemyEffectPrefab;
    public GameObject playerHurtEffectPrefab;

    [Header("Special Ability VFX")]
    public GameObject specialAttackEffectPrefab;




    private void Start()
    {
        sword = GameObject.FindGameObjectWithTag("Sword");
        animator = GetComponent<Animator>();
        

        healthSlider = GameObject.Find("Healthbar").GetComponent<Slider>();
        fillImage = healthSlider.fillRect.GetComponentInChildren<Image>();
        fillImage2 = healthSlider.GetComponentInChildren<Image>();

        healthCanvas = healthSlider.GetComponent<CanvasGroup>();
        
        healthCanvas.alpha = 0f;

        input = GetComponent<PlayerInput>();
        attackAction = input.actions.FindAction("Attack");
        specialAttackAction = input.actions.FindAction("SpecialAttack");
        testAction = input.actions.FindAction("Goon");

        skillManager = GameObject.Find("SkillManager").GetComponent<skillTreeManager>();

        enemyLayer = LayerMask.GetMask("Enemy", "weak Enemy");
        cam = Camera.main;

        playerClass = GetComponent<PlayerClass>();

        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();


        playerActionMap = input.actions.FindActionMap("Player");
        uiActionMap = input.actions.FindActionMap("UI");

        originalColors = new Color[renderers.Length];
        
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }

        audioSource = GetComponent<AudioSource>();

        DeathScreen.SetActive(false);



    }

    private void Update()
    {
        if (testAction.triggered && !isControl)
        {
            isControl = true;
        }
        else if (testAction.triggered && isControl)
        {
            isControl = false;
        }
        
        healthSlider.maxValue = MaxPlayerHealth;
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
            playerHealth = MaxPlayerHealth;
            GetComponent<PlayerMovement>().SetCanMove(false);
            isAttacking = true;
            DeathScreenEnable();
            Test.killCounts.Add(killCount);
            Test.death.Add("Yes");
            Test.amountDash.Add(GetComponent<PlayerMovement>().amountOfDash);
            GetComponent<PlayerMovement>().amountOfDash = 0;

            Waves.GetComponent<EnemyWaves>().StopClearWave();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].material.color = originalColors[i];
                }
            }
            StopAllCoroutines();
        }

        if (killCount >= nextSkillThreshhold && !isControl)
        {
            skillManager.skillPoint++;
            nextSkillThreshhold += 20;
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("isAttacking");
        PlaySound(swingSoundClip, swingSoundDelay);
        GetComponent<PlayerMovement>().Speed -= 2f;
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
            PlaySound(hitEnemySoundClip, hitEnemySoundDelay);
            foreach (Collider enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyHealth>().TakeDamage(basicDamage);
                

                if (playerClass.hasLifeSteal && playerHealth <= MaxPlayerHealth)
                {
                    playerHealth += lifeStealAmount;
                }
                else if (playerHealth > MaxPlayerHealth && playerClass.hasLifeSteal)
                {
                    playerHealth = MaxPlayerHealth;
                }

                if (hitEnemyEffectPrefab)
                {
                    Instantiate(hitEnemyEffectPrefab, enemy.transform.position, Quaternion.identity);
                }
                StartCoroutine(FlashRed(enemy));
            }
        }
        isAttacking = false;
        GetComponent<PlayerMovement>().Speed += baseAttackMoveRed;
    }
    
    void SpecialAttack()
    {
        hasDamageReduction = true;
        isGroundSlamming = true;
        animator.SetTrigger("isGroundSlamming");
        GetComponent<PlayerMovement>().Speed -= speedReduction;
        GetComponent<PlayerMovement>().Stamina -= 100f;
        GetComponent<PlayerMovement>().noStaminaRegen = true;

        PlaySound(specialAttackSoundClip, specialAttackSoundDelay); // ✅ lyd ved aktivering
    }

    void PerformSpecialAttackHit()
    {
        PlaySound(specialAttackHitSoundClip, specialAttackHitSoundDelay); // ✅ lyd ved hit

        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0.5f, 1f);
        }

        StartCoroutine(cam.GetComponent<CameraShake>().Shake(0.3f, 1f));

        if (specialAttackEffectPrefab)
        {
            Instantiate(specialAttackEffectPrefab, transform.position, Quaternion.identity);
        }

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
        

        PlaySound(playerHurtSoundClip, playerHurtSoundDelay);

        if (playerHurtEffectPrefab != null)
        {
            GameObject bloodEffect = Instantiate(playerHurtEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
            ParticleSystem ps = bloodEffect.GetComponent<ParticleSystem>() ?? bloodEffect.GetComponentInChildren<ParticleSystem>();

            if (ps != null)
            {
                ps.Play();
            }

            Destroy(bloodEffect, 2f);
        }

        StartCoroutine(FlashRed());

    }

    IEnumerator FlashRed(Collider enemy = null)
    {
        if (enemy == null)
        {
            var playerRenderer = GetComponent<SkinnedMeshRenderer>();
            Color flameRed = new Color(1f, 0f, 0f, 1f);
            if (playerRenderer) playerRenderer.material.color = flameRed;
            yield return new WaitForSeconds(0.2f);
            if (playerRenderer) playerRenderer.material.color = originalColors[0];
        }
        else
        {
            var enemyRenderer = enemy.GetComponent<SkinnedMeshRenderer>();
            if (enemyRenderer) enemyRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            if (enemyRenderer) enemyRenderer.material.color = originalColors[0];
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
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }

    }

    void EndInvul()
    {
        isInvulnerable = false;
        GetComponent<PlayerMovement>().noStaminaRegen = false;
    }

    void PlaySound(AudioClip clip, float delay)
    {
        if (clip == null) return;
        if (delay > 0f)
        {
            StartCoroutine(PlaySoundWithDelay(clip, delay));
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }

    IEnumerator PlaySoundWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(clip);
    }

    public void DamageVibration()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0.1f, 0.2f);
        }
        Invoke("EndVibration", 0.1f);
    }


    void DeathScreenEnable()
    {
        DeathScreen.SetActive(true);
        if (!isControl)
        {
            KillCountText.text = "Kills: " + killCount.ToString();
            SkillPointText.text = "Skillpoints: " + skillManager.skillPoint.ToString();
        }
        else
        {
            KillCountText.text = "";
            SkillPointText.text = "";
        }
        if (Gamepad.current == null)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Gamepad.current != null)
        {
            playerActionMap.Disable();
            uiActionMap.Enable();   
            EventSystem.current.SetSelectedGameObject(ContinueButton);
        }
    }

    public void Continue()
    {
        playerActionMap.Enable();
        uiActionMap.Disable();
        transform.position = new Vector3(-40f, 42, 0);
        isAttacking = false;
        GetComponent<PlayerMovement>().SetCanMove(true);
        DeathScreen.SetActive(false);
        killCount = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
