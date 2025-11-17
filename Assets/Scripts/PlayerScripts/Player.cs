using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move / Jump")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Audio")]
    public AudioClip jumpClip;
    public AudioClip[] meowClips;     // assign multiple meows in Inspector
    [Range(0f, 1f)] public float meowVolume = 1f;
    public float meowCooldown = 0.25f;

    // Optional: assign in Inspector (Physics Material 2D with 0 friction)
    public PhysicsMaterial2D zeroFrictionMaterial;

    [Header("Ladder")]
    [Tooltip("Radius around player to check for ladder overlap.")]
    public float ladderCheckRadius = 0.25f; // replaces `distance`
    public LayerMask whatIsLadder;
    [Tooltip("Units/sec while climbing.")]
    public float climbSpeed = 4.5f;

    private float nextMeowTime = 0f;
    private int lastMeowIndex = -1;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isGrounded;
    private bool onYellowPlatform = false;
    private Animator animator;
    private bool gravityFlipped;
    private float originalGravity;
    private float moveInputRaw;          // A/D or arrows (horizontal)
    private bool isClimbing;             // currently attached to ladder
    private float wsVertical;            // W/S only (+1 up, -1 down)
    private float climbStrafeSpeed = 4.5f;
    
    [Header("One-Way Platform Drop")]
    [SerializeField] private LayerMask oneWayPlatformMask; // layer for LadderTopZone & other 1-way platforms
    [SerializeField] private float dropThroughDuration = 0.25f; // seconds to ignore collision
    [SerializeField] private float dropNudge = 2f;               // small downward push when dropping

private Coroutine dropRoutine;

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

        if (col && zeroFrictionMaterial != null)
            col.sharedMaterial = zeroFrictionMaterial;

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        if (PauseScript.GameisPaused) return;

        // --- Input ---
        moveInputRaw = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(moveInputRaw) < 0.01f) moveInputRaw = 0f;

        wsVertical = 0f;                         // W/S only (not arrow keys)
        if (Input.GetKey(KeyCode.W)) wsVertical += 1f;
        if (Input.GetKey(KeyCode.S)) wsVertical -= 1f;

        // Flip sprite whether grounded, falling, or climbing:
        if (moveInputRaw > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInputRaw < 0)
            transform.localScale = new Vector3(-1, 1, 1);


        // --- Jump (disabled while climbing) ---
        if (!isClimbing && Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            float dir = gravityFlipped ? -1f : 1f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * dir);

            //Play jump SFX
            audioManager.playerSFX(audioManager.jump);
        }

        // --- Gravity flip (disabled while climbing) ---
        if (!isClimbing && Input.GetKeyDown(KeyCode.F))
            ToggleGravity();

        // --- Meow ---
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            PlayRandomMeow();

        // Press S while grounded to drop through a one-way platform (e.g., LadderTopZone)
        if (Input.GetKeyDown(KeyCode.S) && isGrounded)
        {
            TryDropThroughOneWay();
        }


        // Anim handled in SetAnimation below
        SetAnimation(moveInputRaw, wsVertical);
    }

    private void FixedUpdate()
    {
        // Ground check with physics step
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Are we overlapping any ladder right now?
        bool onLadder = Physics2D.OverlapCircle(transform.position, ladderCheckRadius, whatIsLadder);

        // Attach conditions: overlap ladder AND pressing W/S
        if (!isClimbing && onLadder && Mathf.Abs(wsVertical) > 0.01f)
        {
            BeginClimb();
        }

        // Detach conditions: left ladder volume OR pressed Space
        if (isClimbing && (!onLadder || Input.GetKeyDown(KeyCode.Space)))
        {
            EndClimb();
        }

        if (isClimbing)
        {
            rb.gravityScale = 0f;

            // vertical along ladder
            Vector2 up = transform.up;
            Vector2 v = up * (wsVertical * climbSpeed);

            // horizontal strafe (world X). Use transform.right if your ladders can rotate.
            Vector2 h = Vector2.right * (moveInputRaw * climbStrafeSpeed);

            rb.linearVelocity = h + v;
        }
        else
        {
            rb.gravityScale = gravityFlipped ? -Mathf.Abs(originalGravity) : Mathf.Abs(originalGravity);
            rb.linearVelocity = new Vector2(moveInputRaw * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void BeginClimb()
    {
        isClimbing = true;
        // Kill any residual velocity when attaching
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
    }

    private void EndClimb()
    {
        isClimbing = false;
        rb.gravityScale = gravityFlipped ? -Mathf.Abs(originalGravity) : Mathf.Abs(originalGravity);
        // keep current horizontal vel; vertical will be handled by gravity
    }

    private void ToggleGravity()
    {
        gravityFlipped = !gravityFlipped;

        rb.gravityScale = gravityFlipped ? -Mathf.Abs(originalGravity) : Mathf.Abs(originalGravity);

        // Rotate sprite: 180° Z makes us upside-down; 180° Y flips facing
        transform.Rotate(0f, 180f, 180f);

        // Clear vertical velocity to avoid sticky contacts on flip
        Vector2 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        // Tiny nudge away from surfaces after flip
        rb.AddForce((Vector2)transform.up * 0.01f, ForceMode2D.Impulse);
    }

    private void SetAnimation(float moveInput, float verticalWS)
    {
        if (isClimbing)
        {
            if (Mathf.Abs(verticalWS) > 0.01f) animator.Play("Player_Climb");
            else                               animator.Play("Player_ClimbIdle");
            return;
        }

        if (isGrounded)
        {
            if (moveInput == 0) animator.Play("Player_Idle");
            else                animator.Play("Player_Walk");
        }
        else
        {
            float vAlongUp = Vector2.Dot(rb.linearVelocity, transform.up);
            if (vAlongUp > 0f) animator.Play("Player_Jump");
            else               animator.Play("Player_Fall");
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip) audioSource.PlayOneShot(clip);
    }

    void PlayRandomMeow()
    {
        if (PauseScript.GameisPaused) return;
        if (Time.time < nextMeowTime) return;
        if (meowClips == null || meowClips.Length == 0 || audioSource == null) return;

        int idx = (meowClips.Length == 1) ? 0 : GetNonRepeatingIndex();

        float originalPitch = audioSource.pitch;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(meowClips[idx], meowVolume);
        audioSource.pitch = originalPitch;

        lastMeowIndex = idx;
        nextMeowTime = Time.time + meowCooldown;
    }

    private void TryDropThroughOneWay()
    {
        // Look for a 1-way platform right under our feet
        Collider2D platform = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius * 1.2f,
            oneWayPlatformMask
        );

        if (!platform) return;

        // Must actually be a one-way PlatformEffector2D surface
        if (!platform.GetComponent<PlatformEffector2D>()) return;

        if (dropRoutine != null) StopCoroutine(dropRoutine);
        dropRoutine = StartCoroutine(DropThroughRoutine(platform));
    }

    private System.Collections.IEnumerator DropThroughRoutine(Collider2D platform)
    {
        // Temporarily ignore collision with THIS specific platform
        Physics2D.IgnoreCollision(col, platform, true);

        // Small downward nudge so we separate from the platform immediately
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -Mathf.Abs(dropNudge));

        // Keep collisions ignored briefly so we fully pass through
        yield return new WaitForSeconds(dropThroughDuration);

        // Re-enable collision
        Physics2D.IgnoreCollision(col, platform, false);
    }


    private int GetNonRepeatingIndex()
    {
        int idx;
        do { idx = Random.Range(0, meowClips.Length); } while (idx == lastMeowIndex);
        return idx;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // visualize ladder overlap radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, ladderCheckRadius);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("YellowPlatform"))
        {
            onYellowPlatform = true; // Disable jumping when on a "NoJumpPlatform"
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("YellowPlatform"))
        {
            onYellowPlatform = false; // Re-enable jumping when leaving a "NoJumpPlatform"
        }
    }
}
