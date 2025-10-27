using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // Optional: assign in Inspector (Physics Material 2D with 0 friction)
    public PhysicsMaterial2D zeroFrictionMaterial;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isGrounded;
    private Animator animator;
    private bool gravityFlipped;
    private float originalGravity;
    private float moveInputRaw; // store input sampled in Update

    //Audio manager for player sounds
    private AudioManager audioManager;

    //To get access to audioManager
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        originalGravity = rb.gravityScale;

        // Recommended runtime safety: ensure no friction
        if (col && zeroFrictionMaterial != null)
            col.sharedMaterial = zeroFrictionMaterial;

        // Tweak physics stability
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // smoother rendering
    }

    void Update()
    {
        // Pause check
        if (PauseScript.GameisPaused) return;

        // Sample raw input in Update
        moveInputRaw = Input.GetAxisRaw("Horizontal"); // -1, 0, or 1 for keyboard

        // Deadzone to avoid micro-values messing with movement
        if (Mathf.Abs(moveInputRaw) < 0.01f) moveInputRaw = 0f;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            float dir = gravityFlipped ? -1f : 1f;
            // Apply jump in FixedUpdate-style way: set vertical velocity once
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * dir);

            //Play jump SFX
            audioManager.playerSFX(audioManager.jump);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleGravity();
        }

        if (moveInputRaw > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInputRaw < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        SetAnimation(moveInputRaw);
    }

    private void FixedUpdate()
    {
        // Ground check should be in FixedUpdate to sync with physics
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Apply horizontal velocity in FixedUpdate (physics-friendly)
        rb.linearVelocity = new Vector2(moveInputRaw * moveSpeed, rb.linearVelocity.y);

        // Optional: ensure no wall-sticking when airborne (frictionless always is usually enough)
        // If you later add ground friction, you can toggle materials like this:
        // col.sharedMaterial = isGrounded ? groundFrictionMaterial : zeroFrictionMaterial;
    }

    private void ToggleGravity()
    {
        gravityFlipped = !gravityFlipped;

        // Flip gravity magnitude sign only
        rb.gravityScale = gravityFlipped ? -Mathf.Abs(originalGravity) : Mathf.Abs(originalGravity);

        // Rotate sprite: 180° Z gives upside-down; 180° Y flips facing
        transform.Rotate(0f, 180f, 180f);

        // Clear vertical velocity to avoid “sticky” contacts when flipping
        Vector2 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        // Tiny nudge away from surfaces after flip
        rb.AddForce((Vector2)transform.up * 0.01f, ForceMode2D.Impulse);
    }

    private void SetAnimation(float moveInput)
    {
        if (isGrounded)
        {
            if (moveInput == 0)
                animator.Play("Player_Idle");
            else
                animator.Play("Player_Walk");
        }
        else
        {
            float vAlongUp = Vector2.Dot(rb.linearVelocity, transform.up);
            if (vAlongUp > 0f) animator.Play("Player_Jump");
            else animator.Play("Player_Fall");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
