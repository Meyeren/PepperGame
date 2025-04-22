using UnityEngine;

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

    private AudioSource audioSource;
    private float stepTimer;

    bool isJumping = false;
    bool isDashing = false;

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

        if (!IsGrounded() || isJumping || isDashing)
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
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(footstepClip);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    public void SetJumping(bool state)
    {
        isJumping = state;
    }

    public void SetDashing(bool state)
    {
        isDashing = state;
    }
}
