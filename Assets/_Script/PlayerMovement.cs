using System.Net.Mime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    PlayerInput input;
    InputAction moveAction;
    InputAction jumpAction;
    Transform groundCheck;
    LayerMask groundLayer;

    [SerializeField] float Speed = 8.0f;

    [SerializeField] float Stamina = 100f;

    [SerializeField] float jumpPower = 15.0f;
    [SerializeField] float fallStop = 1.0f;
    [SerializeField] float fallSpeed = 1.0f;
    [SerializeField] float jumpCost = 50f;
    [SerializeField] float staminaRegenAmount = 10f;

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

        groundCheck = transform.Find("GroundCheck");
        groundLayer = LayerMask.GetMask("Ground");

        Stamina = 100f;


    }

    private void Update()
    {
        HandleJump();
        StaminaRegen();
        
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

        rb.linearVelocity = new Vector3(direction.x * Speed, rb.linearVelocity.y, direction.y * Speed);
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

    void StaminaRegen()
    {
        if (IsGrounded() && Stamina > 0f)
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