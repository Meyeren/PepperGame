using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    PlayerInput input;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction lookAction;
    InputAction dashAction;

    Animator animator;

    Transform groundCheck;
    LayerMask groundLayer;
    Transform cam;

    [Header("Player")]
    public float Speed = 8.0f;
    public float Stamina = 100f;
    public float maxStamina;
    [SerializeField] float staminaRegenAmount = 10f;
    public bool noStaminaRegen;
        public bool canRotate = true;

    [Header("Camera")]
    public float sensitivity = 100f;
    public float FOV = 80f;
    
    float Xrotation;

    [Header("Jump")]
    [SerializeField] float jumpPower = 15.0f;
    [SerializeField] float jumpCost = 50f;

    [SerializeField] float fallStop = 1.0f;
    [SerializeField] float fallSpeed = 1.0f;
    [SerializeField] float fallgravity = 15f;

    public bool doubleJump;
    public bool allowDoubleJump;


    [Header("Dash")]
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashLength = 10f;
    public float dashCost = 100f;
    public bool isDashing;
    


    Slider staminaSlider;
    CanvasGroup staminaCanvasGroup;

    FootstepAudio footstepAudio;

    private bool canMove = true;


    bool isAttacking;
    bool isGroundSlamming;
    bool wasGroundedLastFrame;

    [Header("VFX")]
    public VisualEffect jumpEffect;
    public VisualEffect landEffect;
    public GameObject dashEffectPrefab;

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip dashSound;
    private AudioSource audioSource;

    public int amountOfDash;

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    public void FreezePlayerImmediately()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        canMove = false;
        animator.SetFloat("speed", 0f);
        animator.SetFloat("sideSpeed", 0f);
        animator.SetBool("isIdling", true);
    }

    private void Start()
    {
        noStaminaRegen = false;
        doubleJump = false;
        allowDoubleJump = false;
        staminaSlider = GameObject.Find("StaminaBar").GetComponent<Slider>();
        staminaCanvasGroup = staminaSlider.GetComponentInParent<CanvasGroup>();
        staminaCanvasGroup.alpha = 0f;
        staminaSlider.maxValue = Stamina;
        canRotate = true;
        maxStamina = 100f;

        animator = GetComponent<Animator>();

        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        moveAction = input.actions.FindAction("Move");
        jumpAction = input.actions.FindAction("Jump");
        lookAction = input.actions.FindAction("Look");
        dashAction = input.actions.FindAction("Dash");

        groundCheck = transform.Find("GroundCheck");
        groundLayer = LayerMask.GetMask("Ground");

        cam = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isDashing = false;

        footstepAudio = GetComponentInChildren<FootstepAudio>();

        if (Gamepad.current != null)
        {
            sensitivity += 1f;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        wasGroundedLastFrame = IsGrounded();
    }

    private void Update()
    {
        staminaSlider.maxValue = maxStamina;
        
        if (!canMove) return;

        isAttacking = GetComponent<Combat>().isAttacking;
        isGroundSlamming = GetComponent<Combat>().isGroundSlamming;

        HandleJump();

        if (dashAction.triggered && Stamina >= dashCost && !isDashing && IsGrounded() && !isGroundSlamming)
        {
            if (!isAttacking)
            {
                StartDash();

                
            }
            else if (GetComponent<Combat>().attackWhileDash)
            {
                StartDash();
                GetComponent<Combat>().isInvulnerable = true;
                GetComponent<Combat>().basicDamage += GetComponent<Combat>().dashAttackDamage;
                Stamina -= 100;
                noStaminaRegen = true;
                Invoke("EndDashInvul", 1f);
            }
        }
        

        staminaSlider.value = Stamina;

        if (IsGrounded() && Stamina >= 0f)
        {
            StaminaRegen();
        }

        if (Stamina == maxStamina)
        {
            FadeOutStaminaBar();
        }
        else
        {
            staminaCanvasGroup.alpha = 1f;
        }

        if (IsGrounded() && footstepAudio.isJumping)
        {
            footstepAudio.SetJumping(false);
        }

        if (!wasGroundedLastFrame && IsGrounded())
        {
            if (landEffect != null)
            {
                landEffect.transform.position = groundCheck.position;
                landEffect.Play();
            }
        }

        wasGroundedLastFrame = IsGrounded();

        if (!canMove || input.currentActionMap.name == "UI") return;
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        MovePlayer();
        RotatePlayer();
    }

    private void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * direction.x + transform.forward * direction.y;

        rb.linearVelocity = new Vector3(move.x * Speed, rb.linearVelocity.y, move.z * Speed);
        animator.SetFloat("speed", direction.y);
        animator.SetFloat("sideSpeed", direction.x);

        animator.SetBool("isIdling", direction == Vector2.zero);
    }

    void RotatePlayer()
    {
        if (canRotate)
        {
            Vector2 rotation = lookAction.ReadValue<Vector2>();
            transform.Rotate(Vector3.up * rotation.x * sensitivity);

            Xrotation -= rotation.y * sensitivity;
            Xrotation = Mathf.Clamp(Xrotation, -20f, 20f);
            cam.localRotation = Quaternion.Euler(Xrotation, 0f, 0f);
        }

    }

    public bool IsGrounded()
    {
        return Physics.OverlapSphere(groundCheck.position, 0.2f, groundLayer).Length > 0f;
    }

    void HandleJump()
    {
        if (IsGrounded() && jumpAction.triggered && Stamina >= jumpCost && !isAttacking && !isGroundSlamming)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpPower, rb.linearVelocity.z);
            Stamina -= jumpCost;
            footstepAudio.SetJumping(true);
            animator.SetBool("Jumping", true);

            if (jumpEffect != null)
            {
                jumpEffect.transform.position = groundCheck.position;
                jumpEffect.Play();
            }

            
            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
            if (allowDoubleJump)
            {
                doubleJump = true;
            }
        }
        else if (jumpAction.triggered && doubleJump && allowDoubleJump)
        {
            doubleJump = false;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpPower, rb.linearVelocity.z);
            footstepAudio.SetJumping(true);
            animator.SetBool("Jumping", true);

            if (jumpEffect != null)
            {
                jumpEffect.transform.position = groundCheck.position;
                jumpEffect.Play();
            }


            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
            
        }
      
        else
        {
            animator.SetBool("Jumping", false);

        }

        Physics.gravity = rb.linearVelocity.y > 0f
            ? new Vector3(0, -fallgravity, 0)
            : new Vector3(0, -9.81f, 0);

        if (jumpAction.WasReleasedThisFrame() && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * fallStop - fallSpeed, rb.linearVelocity.z);
        }
    }

    void StartDash()
    {
        Stamina -= dashCost;
        isDashing = true;
        animator.SetBool("isDashing", isDashing);

        amountOfDash++;

        Vector3 dashDirection = rb.linearVelocity;
        if (dashDirection.magnitude < 0.01f)
        {
            dashDirection = transform.forward;
        }
        dashDirection.y = 0f;
        GameObject dashVFX = Instantiate(dashEffectPrefab, transform.position, Quaternion.LookRotation(dashDirection.normalized * dashSpeed));

        Destroy(dashVFX, 2f);

        if (dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }

        StartCoroutine(Dash(dashDirection));

        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0.5f, 1f);
        }
    }

    IEnumerator Dash(Vector3 dashDirection)
    {
        float elapsedTime = 0f;

        while (elapsedTime < dashLength && isDashing)
        {
            rb.AddForce(dashDirection.normalized * dashSpeed, ForceMode.Impulse);
            
            elapsedTime += Time.deltaTime;
            yield return null;
            Camera.main.fieldOfView += 0.3f;
        }

        
        isDashing = false;
        animator.SetBool("isDashing", isDashing);
        StartCoroutine(EndDash());

        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isDashing = false;
            animator.SetBool("isDashing", isDashing);
        }
    }

    void StaminaRegen()
    {
        if (IsGrounded() && Stamina >= 0f && !isDashing && !noStaminaRegen)
        {
            Stamina += staminaRegenAmount * Time.deltaTime;
            Stamina = Mathf.Clamp(Stamina, 0, maxStamina);
        }
    }

    void FadeOutStaminaBar()
    {
        if (staminaCanvasGroup.alpha > 0f)
        {
            staminaCanvasGroup.alpha -= Time.deltaTime * 0.5f;
        }
    }

    IEnumerator EndDash()
    {
        while (Camera.main.fieldOfView > FOV)
        {
            Camera.main.fieldOfView -= 0.2f;
            yield return null;
        }
        Camera.main.fieldOfView = FOV;
    }

    public void EndDashInvul()
    {
        GetComponent<Combat>().isInvulnerable = false;
        GetComponent<Combat>().basicDamage -= 50;
        noStaminaRegen = false;
    }
}
