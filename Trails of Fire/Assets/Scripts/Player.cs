using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] 
    private Faction faction;
    [SerializeField]
    private float maxSpeed = 100;
    [SerializeField]
    private float jumpSpeed = 200;
    [SerializeField]
    private float maxJumpTime = 0.1f;
    [SerializeField]
    private int maxHealth = 3;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundCheckRadius = 2;
    [SerializeField]
    private LayerMask groundCheckLayers;
    [SerializeField]
    private Collider2D airCollider;
    [SerializeField]
    private Collider2D groundCollider;
    [SerializeField]
    private float dashPower;
    [SerializeField]
    private float dashTime;
    [SerializeField]
    private float dashCooldown;
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
    private float invincibilityDurationSeconds;
    [SerializeField]
    private int maxGauge = 10;
    [SerializeField]
    private float attackCooldown;
    [SerializeField]
    private GameObject fireBall;
    [SerializeField]
    private GameObject ulti;
    [SerializeField]
    private Transform shootPlace;
    [SerializeField]
    private LayerMask nextLevelLayer;


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
    private ShootingPlacePlayer shootingPlacePlayer;
    public int direction = 1;
    

    // Start is called before the first frame update

    void Start()
    {
        health = maxHealth;
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
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = IsGrounded();
        /* if(isGrounded)
        {
            sr.color = Color.green;
        }
        else
        {
            sr.color = Color.red;
        } */

        airCollider.enabled = !isGrounded;
        groundCollider.enabled = isGrounded;

        //andar
        float deltaX = Input.GetAxis("Horizontal");

        Vector3 velocity = rb.velocity;

        velocity.x = deltaX * maxSpeed;

        //salto
        if ((Input.GetButtonDown("Jump")) && (isGrounded))
        {
            velocity.y = jumpSpeed;
            jumpTime = Time.time;
            rb.gravityScale = 1;
        }
        else if((Input.GetButton("Jump")) && ((Time.time - jumpTime) < maxJumpTime))
        {
            rb.gravityScale = 1;
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }

        rb.velocity = velocity;

        //Animation
        //animator.SetFloat("AbsVelocityX", Mathf.Abs(velocity.x));

        if ((velocity.x <0) && (transform.right.x >0))
        {
            transform.rotation = Quaternion.Euler(0,180,0);
            direction = -1;
        } 
        else if((velocity.x > 0) && (transform.right.x < 0))
        {
            transform.rotation = Quaternion.identity;
            direction = 1;
        } 

        if (isDashing)
        {
            return;
        }

        if(Input.GetButtonDown("Dash") && canDash)
        {
            StartCoroutine(Dash());
        }
        TouchEnemy();
        
        if((Input.GetButtonDown("Normal Attack")) && (cooldownTimer > attackCooldown))
        {
            Attack();
        }
        if((Input.GetButtonDown("Super Attack")) && (gauge >= maxGauge))
        {
            SuperAttack();
        }

        cooldownTimer += Time.deltaTime;
    }
    private void Attack()
    {
        Instantiate(fireBall, shootPlace.position, fireBall.transform.rotation);
        cooldownTimer = 0;
    }
    private void SuperAttack()
    {
        Instantiate(ulti, shootPlace.position, ulti.transform.rotation);
        gauge = 0;
    }
    

    //Toca no inimigo
    private void TouchEnemy()
    {
        //se estiver imortal
        if (isInvincible)
            return;
        Collider2D enemycollider = Physics2D.OverlapArea(sizeA.position, sizeB.position, enemyLayer);
        Collider2D bulletcollider = Physics2D.OverlapArea(sizeA.position, sizeB.position, bulletLayer);
        //se morrer
        if (((enemycollider != null) || (bulletcollider != null)) && (health <= 1))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //se ainda tiver uma vida
        else if (((enemycollider != null) || (bulletcollider != null)) && (health > 1))
        {
            health -= 1;
            StartCoroutine(BecomeTemporarilyInvincible());
        }

        Collider2D levelCollier = Physics2D.OverlapArea(sizeA.position, sizeB.position, nextLevelLayer);
        if (levelCollier != null)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
            
    }

    private bool IsGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundCheckLayers);

        if (collider == null)
        {
            return false;
        }

        return true;
    }
    
    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibilityDurationSeconds);

        isInvincible = false;
    }

    public Vector2 GivePosition()
    {
        Vector2 vector = rb.position;
        return vector;
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
        Vector3 velocity = rb.velocity;
        rb.velocity = velocity;
        if (velocity.x <0)
        {
            rb.velocity = new Vector2((- transform.localScale.x) * dashPower, 0f);
        }
        else if(velocity.x > 0)
        {
            rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        }
        
        tr.emitting = true;
        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
