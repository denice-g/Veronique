using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public AudioClip jumpClip;
    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private bool gravityFlipped;
    private float originalGravity;


    private AudioSource audioSource;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        originalGravity = rb.gravityScale;
    }


    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (PauseScript.GameisPaused)
            return;

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            float jumpDirection = gravityFlipped ? -1f : 1f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpDirection);
            PlaySFX(jumpClip);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleGravity();
        }

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        SetAnimation(moveInput);
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ToggleGravity()
    {
        gravityFlipped = !gravityFlipped;

        // Flip gravity (keep same magnitude)
        rb.gravityScale = gravityFlipped ? -Mathf.Abs(originalGravity) : Mathf.Abs(originalGravity);

        // Rotate the whole player 180° around Z
        transform.Rotate(0f, 180f, 180f);

        // Clear vertical speed and nudge away so you don't stick
        Vector2 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;
        rb.AddForce((Vector2)transform.up * 0.01f, ForceMode2D.Impulse);
    }



    private void SetAnimation(float moveInput)
    {
        if (isGrounded)
        {
            if (moveInput == 0)
            {
                animator.Play("Player_Idle");
            }

            else
            {
                animator.Play("Player_Walk");
            }
        }

            else
            {
                float vAlongUp = Vector2.Dot(rb.linearVelocity, transform.up);

                if (vAlongUp > 0f)
                {
                    animator.Play("Player_Jump");
                }
                else
                {
                    animator.Play("Player_Fall");
                }
            }
    }

    private void PlaySFX(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}

