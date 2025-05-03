using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class FootstepAudio : MonoBehaviour
{
    public AudioClip footstepClip;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    public float speedThreshold = 0.1f;
    public float stepInterval = 0.5f;

    public Rigidbody playerRigidbody;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;
    public Transform groundCheck;

    [Header("Vibration Settings")]
    public float footstepVibrationIntensity = 0.2f;
    public float footstepVibrationDuration = 0.1f;

    public float jumpVibrationIntensity = 0.3f;
    public float jumpVibrationDuration = 0.15f;

    public float landVibrationIntensity = 0.4f;
    public float landVibrationDuration = 0.2f;

    private AudioSource audioSource;
    private float stepTimer;
    private bool wasGrounded = true;
    public bool isJumping = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponentInParent<Rigidbody>();
        }

        if (groundCheck == null)
        {
            groundCheck = transform;
        }
    }

    void Update()
    {
        if (footstepClip == null || playerRigidbody == null) return;

        bool grounded = IsGrounded();

        if (!wasGrounded && grounded && isJumping)
        {

            isJumping = false;
        }

        wasGrounded = grounded;

        if (!grounded || isJumping)
        {
            stepTimer = 0f;
            return;
        }

        Vector3 horizontalVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z);
        float speed = horizontalVelocity.magnitude;

        if (speed > speedThreshold)
        {
            stepTimer -= Time.deltaTime * speed;

            if (stepTimer <= 0f)
            {
                PlayFootstep();

                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (GetComponent<PlayerMovement>().isDashing)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(footstepClip);

        }


    }

    public bool IsGrounded()
    {
        return Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    public void SetJumping(bool state)
    {
        if (!isJumping && state)
        {

        }
        isJumping = state;
    }


}