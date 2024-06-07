using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private bool gunner = false;
    [SerializeField]
    private bool melee = false;
    [SerializeField]
    private float maxSpeed = 100;
    [SerializeField]
    private float jumpSpeed = 200;
    [SerializeField, ShowIf(nameof(melee))]
    private Transform wallCheck;
    [SerializeField, ShowIf(nameof(melee))]
    private float wallCheckRadius = 2;
    // Start is called before the first frame update

    private Rigidbody2D thisRB;
    Player player;
    void Start()
    {
        thisRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType <Player>();
            if (player != null)
            Debug.LogError($"Kill myself");
        }
        Move();
        
    }

    void Move()
    {
        Vector2 currentlyVelocity = thisRB.velocity;

        Debug.LogError("I should walk");
        if (melee)
        {
            Vector2 playerposition = player.GiveX();
            Debug.LogError($"P:{playerposition.x}\nE:{thisRB.position.x}\nA{playerposition.x < transform.position.x}");
            if (playerposition.x > transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            if (playerposition.x < transform.position.x)
            {
                transform.rotation = Quaternion.identity;
                
            }

            /* if ((Input.GetButtonDown("Jump")) && (isGrounded))
            {
                velocity.y = jumpSpeed;
                jumpTime = Time.time;
                rb.gravityScale = 1;
            } */
            currentlyVelocity.x = maxSpeed * Mathf.Sign(transform.right.x);
            thisRB.velocity = -currentlyVelocity;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if(wallCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(wallCheck.position, wallCheckRadius);
        }
    }
}
