using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Faction faction;
    [SerializeField]
    private float maxSpeed = 10f;
    [SerializeField]
    private float jumpSpeed = 5f;
    [SerializeField]
    private float maxJumpTime = 0.1f;
    [SerializeField]
    private int maxHealth = 3;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundCheckRadius = 0.2f;
    [SerializeField]
    private LayerMask groundCheckLayers;
    [SerializeField]
    private Collider2D airCollider;
    [SerializeField]
    private Collider2D groundCollider;
    [SerializeField]
    private float dashPower = 20f; // Increased dash power
    [SerializeField]
    private float dashTime = 0.2f;
    [SerializeField]
    private float dashCooldown = 1f;
    [SerializeField]
    private TrailRenderer tr;
    [SerializeField]
    private Transform sizeA;
    [SerializeField]
    private Transform sizeB;
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private LayerMask bulletLayer;
    [SerializeField]
    private float invincibilityDurationSeconds = 2f;
    [SerializeField]
    private int maxGauge = 10;
    [SerializeField]
    private float attackCooldown = 1f;
    [SerializeField]
    private GameObject fireBall;
    [SerializeField]
    private GameObject ulti;
    [SerializeField]
    private Transform shootPlace;
    [SerializeField]
    private LayerMask nextLevelLayer;
    [SerializeField]
    private AudioClip attackSound;
    [SerializeField]
    private AudioClip ultiSound;
    [SerializeField]
    private AudioClip gameStartSound;
    [SerializeField]
    private AudioClip deathSound;

    public int health;
    public Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private float defaultGravity;
    private float jumpTime;
    private bool canDash = true;
    private bool isDashing;
    private bool isInvincible = false;
    public int score = 0;
    public int gauge = 0;
    private float cooldownTimer = Mathf.Infinity;
    public int direction = 1;
    private float horizontalInput;

    void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("There is no Rigid Body on this object!");
        }
        else
        {
            defaultGravity = rb.gravityScale;
        }
        sr = GetComponent<SpriteRenderer>();
        SoundManager.instance.PlaySound(gameStartSound);
    }

    void Update()
    {
        bool isGrounded = IsGrounded();

        airCollider.enabled = !isGrounded;
        groundCollider.enabled = isGrounded;

        // Movement
        horizontalInput = Input.GetAxis("Horizontal");
        Vector3 velocity = rb.velocity;
        velocity.x = horizontalInput * maxSpeed;

        // Jumping
        if (isGrounded)
        {
            animator.SetBool("IsJumping", false);  // Update IsJumping to false when grounded
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpSpeed;
            jumpTime = Time.time;
            rb.gravityScale = 1;
            animator.SetBool("IsJumping", true);
        }
        else if (Input.GetButton("Jump") && (Time.time - jumpTime) < maxJumpTime)
        {
            rb.gravityScale = 1;
        }
        else if (!isGrounded)
        {
            rb.gravityScale = defaultGravity;
        }

        rb.velocity = velocity;

        // Animation
        animator.SetFloat("AbsVelocityX", Mathf.Abs(velocity.x));
        animator.SetBool("IsRunning", Mathf.Abs(horizontalInput) > 0 && isGrounded);  // Update IsRunning based on horizontalInput and isGrounded

        if (velocity.x < 0 && transform.right.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            direction = -1;
        }
        else if (velocity.x > 0 && transform.right.x < 0)
        {
            transform.rotation = Quaternion.identity;
            direction = 1;
        }

        if (isDashing)
        {
            return;
        }

        // Trigger dash when space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }

        TouchEnemy();

        if (Input.GetButtonDown("Normal Attack") && cooldownTimer > attackCooldown)
        {
            Attack();
        }
        if (Input.GetButtonDown("Super Attack") && gauge >= maxGauge)
        {
            SuperAttack();
        }

        cooldownTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * maxSpeed, rb.velocity.y);
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    private void Attack()
    {
        Instantiate(fireBall, shootPlace.position, fireBall.transform.rotation);
        SoundManager.instance.PlaySound(attackSound);
        cooldownTimer = 0;
    }

    private void SuperAttack()
    {
        Instantiate(ulti, shootPlace.position, ulti.transform.rotation);
        SoundManager.instance.PlaySound(ultiSound);
        gauge = 0;
    }

    private void TouchEnemy()
    {
        if (isInvincible)
            return;

        Collider2D enemyCollider = Physics2D.OverlapArea(sizeA.position, sizeB.position, enemyLayer);
        Collider2D bulletCollider = Physics2D.OverlapArea(sizeA.position, sizeB.position, bulletLayer);

        if ((enemyCollider != null || bulletCollider != null) && health <= 1)
        {
            SoundManager.instance.PlaySound(deathSound);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if ((enemyCollider != null || bulletCollider != null) && health > 1)
        {
            health -= 1;
            StartCoroutine(BecomeTemporarilyInvincible());
        }

        Collider2D levelCollider = Physics2D.OverlapArea(sizeA.position, sizeB.position, nextLevelLayer);
        if (levelCollider != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private bool IsGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundCheckLayers);
        return collider != null;
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDurationSeconds);
        isInvincible = false;
    }

    public Vector2 GivePosition()
    {
        return rb.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        Vector2 dashVelocity = new Vector2(direction * dashPower, rb.velocity.y);
        rb.velocity = dashVelocity;

        tr.emitting = true;
        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
