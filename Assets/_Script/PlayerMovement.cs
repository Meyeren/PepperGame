using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    PlayerInput input;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction lookAction;
    InputAction dashAction;

    Transform groundCheck;
    LayerMask groundLayer;
    Transform cam;

    [SerializeField] float Speed = 8.0f;

    [SerializeField] float Stamina = 100f;

    [SerializeField] float sensitivity = 100f;
    float Xrotation;

    [SerializeField] float jumpPower = 15.0f;
    [SerializeField] float fallStop = 1.0f;
    [SerializeField] float fallSpeed = 1.0f;
    [SerializeField] float jumpCost = 50f;
    [SerializeField] float staminaRegenAmount = 10f;

    [SerializeField] float dashCost = 100f;
    [SerializeField] float dashSpeed = 20f;

    Slider staminaSlider;
    CanvasGroup staminaCanvasGroup;

    private void Start()
    {
        staminaSlider = GameObject.Find("StaminaBar").GetComponent<Slider>();
        staminaCanvasGroup = staminaSlider.GetComponentInParent<CanvasGroup>();
        staminaCanvasGroup.alpha = 0f;

        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        moveAction = input.actions.FindAction("Move");
        jumpAction = input.actions.FindAction("Jump");
        lookAction = input.actions.FindAction("Look");
        dashAction = input.actions.FindAction("Dash");

        groundCheck = transform.Find("GroundCheck");
        groundLayer = LayerMask.GetMask("Ground");

        Stamina = 100f;

        cam = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Update()
    {
        HandleJump();
        StaminaRegen();
        RotatePlayer();
        Dash();


        staminaSlider.value = Stamina;
        if (Stamina == 100)
        {
            FadeOutStaminaBar();
        }
        else
        {
            staminaCanvasGroup.alpha = 1f;
        }
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }



    private void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        direction.Normalize();
        
        Vector3 move = transform.right * direction.x + transform.forward * direction.y;
        rb.linearVelocity = new Vector3(move.x * Speed, rb.linearVelocity.y, move.z * Speed);

    }

    void RotatePlayer()
    {
        Vector2 rotation = lookAction.ReadValue<Vector2>();
        rotation.Normalize();

        transform.Rotate(Vector3.up * rotation.x * sensitivity * Time.deltaTime);
        
        Xrotation -= rotation.y * sensitivity * Time.deltaTime;
        Xrotation = Mathf.Clamp(Xrotation, -20f, 20f);

        cam.localRotation = Quaternion.Euler(Xrotation, 0f, 0f);
    }

    bool IsGrounded()
    {
        return Physics.OverlapSphere(groundCheck.position, 0.2f, groundLayer).Length > 0f;
    }

    void HandleJump()
    {
        if (IsGrounded() && jumpAction.triggered && Stamina >= jumpCost)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x,jumpPower, rb.linearVelocity.z);
            Stamina -= jumpCost;
        }
        if (jumpAction.WasReleasedThisFrame() && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * fallStop + (-fallSpeed), rb.linearVelocity.z);
        }
    }

    void Dash()
    {
        Debug.Log(dashAction.triggered);
        if (dashAction.triggered && Stamina >= dashCost)
        {
            Debug.Log("Dash");
            Stamina -= dashCost;
            Vector3 dashDirection = cam.transform.forward;
            dashDirection.y = 0f;
            dashDirection.Normalize();

            transform.Translate(dashDirection * dashSpeed);
        }
    }

    void StaminaRegen()
    {
        if (IsGrounded() && Stamina >= 0f)
        {
            Stamina += staminaRegenAmount * Time.deltaTime;
            Stamina = Mathf.Clamp(Stamina, 0, 100);
        }
    }

    void FadeOutStaminaBar()
    {
        if (staminaCanvasGroup.alpha > 0f)
        {
            staminaCanvasGroup.alpha -= Time.deltaTime * 0.5f;
        }
    }
}