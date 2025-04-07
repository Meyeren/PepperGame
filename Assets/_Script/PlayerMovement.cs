using System.Net.Mime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    PlayerInput input;
    InputAction moveAction;
    InputAction jumpAction;
    Transform groundCheck;
    LayerMask groundLayer;

    [SerializeField] float Speed = 1.0f;
    [SerializeField] float jumpPower = 16.0f;

    bool isGrounded;
    private void Start()
    {
        input = GetComponent<PlayerInput>();
        moveAction = input.actions.FindAction("Move");
        jumpAction = input.actions.FindAction("Jump");

        rb = GetComponent<Rigidbody>();
        groundCheck = transform.Find("GroundCheck");
        groundLayer = LayerMask.GetMask("Ground");


    }

    private void Update()
    {
        MovePlayer();
        CheckGroundStatus();
        HandleJump();

    }

    

    private void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        rb.linearVelocity = new Vector3(direction.x * Speed, 0, direction.y * Speed);
    }

    void CheckGroundStatus()
    {
        isGrounded =  Physics.OverlapSphere(groundCheck.position, 0.2f, groundLayer).Length > 0;
    }

    void HandleJump()
    {
        if (isGrounded && jumpAction.triggered)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpPower, rb.linearVelocity.z);

        }
    }
}