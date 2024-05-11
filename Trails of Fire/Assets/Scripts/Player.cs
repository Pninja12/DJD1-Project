using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lio : MonoBehaviour
{
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

    private int health;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private float defaultGravity;
    private float jumpTime;

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
        if(isGrounded)
        {
            sr.color = Color.green;
        }
        else
        {
            sr.color = Color.red;
        }

        airCollider.enabled = !isGrounded;
        groundCollider.enabled = isGrounded;

        // Ver qual o input
        float deltaX = Input.GetAxis("Horizontal");

        // Mover objecto nessa direc??o
        //Vector3 moveVector = new Vector3(deltaX * maxSpeed * Time.deltaTime, 0, 0);
        //transform.position = transform.position + moveVector;

        Vector3 velocity = rb.velocity;

        velocity.x = deltaX * maxSpeed;

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

        if ((velocity.x <0) && (transform.right.x >0)) transform.rotation = Quaternion.Euler(0,180,0);
        else if((velocity.x > 0) && (transform.right.x < 0)) transform.rotation = Quaternion.identity;
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

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
