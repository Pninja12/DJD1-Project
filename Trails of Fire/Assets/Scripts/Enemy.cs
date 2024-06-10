using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform sizeA;
    [SerializeField]
    private Transform sizeB;
    [SerializeField] 
    private Faction faction;
    [SerializeField]
    private bool gunner = false;
    [SerializeField]
    private bool melee = false;
    [SerializeField]
    private bool car = false;
    [SerializeField]
    private float maxSpeed = 100;
    [SerializeField]
    private Transform playerCheck;
    [SerializeField]
    private float playerCheckRadius;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private LayerMask ultiLayer;
    [SerializeField]
    private LayerMask shotLayer;
    [SerializeField, ShowIf(nameof(melee))]
    private Transform wallCheck;
    [SerializeField, ShowIf(nameof(melee))]
    private float wallCheckRadius = 2;
    [SerializeField, ShowIf(nameof(melee))]
    private Transform groundCheck;
    [SerializeField, ShowIf(nameof(melee))]
    private float groundCheckRadius = 2;
    [SerializeField, ShowIf(nameof(melee))]
    private LayerMask theCheckLayer;
    [SerializeField, ShowIf(nameof(melee))]
    private float jumpSpeed = 200;
    [SerializeField, ShowIf(nameof(melee))]
    private float maxJumpTime = 0.1f;
    [SerializeField, ShowIf(nameof(melee))]
    private AudioClip meleeSound;
    [SerializeField, ShowIf(nameof(gunner))]
    private float attackCooldown = 4;
    [SerializeField, ShowIf(nameof(gunner))]
    private GameObject bullet;
    [SerializeField, ShowIf(nameof(gunner))]
    private Transform shootPlace;
    [SerializeField, ShowIf(nameof(gunner))]
    private AudioClip gunnerSound;
    [SerializeField, ShowIf(nameof(car))]
    private float timeToMove = 0;
    [SerializeField, ShowIf(nameof(car))]
    private AudioClip carSound;

    // Start is called before the first frame update

    private Rigidbody2D thisRB;
    private Player player;
    private bool isPlayerInside;
    private float jumpTime;
    private float defaultGravity;
    private bool isInvincible;
    private bool hit = false;
    private float cooldownTimer = Mathf.Infinity;
    private float cooldown = Mathf.Infinity;
    private bool rotate = false;
    void Start()
    {
        thisRB = GetComponent<Rigidbody2D>();
        defaultGravity = thisRB.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType <Player>();
        }
        Action();

        if(!car)
            TouchPlayer();
        TouchUlti();

        if(hit)
            Destroy(gameObject,0.05f);


        
    }

    void Action()
    {
        Vector2 currentlyVelocity = thisRB.velocity;
        isPlayerInside = PlayerIn();
        if (melee)
        {
            if(isPlayerInside)
            {
                Vector2 playerposition = player.GivePosition();
                if (playerposition.x > transform.position.x)
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                if (playerposition.x < transform.position.x)
                {
                    transform.rotation = Quaternion.identity;
                    
                }
                currentlyVelocity.x = -(maxSpeed * Mathf.Sign(transform.right.x));
            }


            if (MaybeJump())
            {
                currentlyVelocity.y = jumpSpeed;
                jumpTime = Time.time;
                thisRB.gravityScale = 1;
                if ((Time.time - jumpTime) > maxJumpTime)
                    thisRB.gravityScale = defaultGravity;
            }
            
            
            
            thisRB.velocity = currentlyVelocity;
        }

        if(gunner)
        {
            Vector2 playerposition = player.GivePosition();
            if (playerposition.x > transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            if (playerposition.x < transform.position.x)
            {
                transform.rotation = Quaternion.identity;
                
            }

            if(isPlayerInside)
            {
                if(cooldownTimer > attackCooldown)
                {
                    Shoot();
                }
            }
            cooldownTimer += Time.deltaTime;
        }

        if(car)
        {
            
            if(cooldown > timeToMove)
            {
                if(rotate)
                {
                    transform.rotation = Quaternion.Euler(0,180,0);
                    SoundManager.instance.PlaySound(carSound);
                }    
                else
                {
                    transform.rotation = Quaternion.identity;
                    SoundManager.instance.PlaySound(carSound);
                }
                cooldown = 0;
                rotate = !rotate;
            }
            currentlyVelocity.x = -(maxSpeed * Mathf.Sign(transform.right.x));
            thisRB.velocity = currentlyVelocity;
            cooldown += Time.deltaTime;
        }

        
    }

    private void Shoot()
    {
        Instantiate(bullet, shootPlace.position, bullet.transform.rotation);
        SoundManager.instance.PlaySound(gunnerSound);
        cooldownTimer = 0;
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;

        yield return new WaitForSeconds(0.01f);

        isInvincible = false;
    }

    private bool MaybeJump()
    {
        Collider2D groundcollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, theCheckLayer);
        Collider2D wallcollider = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, theCheckLayer);
        if ((groundcollider == null) || (wallcollider == null))
        {
            return false;
        }

        return true;
    }

    private bool PlayerIn()
    {
        Collider2D collider = Physics2D.OverlapCircle(playerCheck.position, playerCheckRadius, playerLayer);

        if (collider == null)
        {
            return false;
        }

        return true;
    }

    private void TouchPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapArea(sizeA.position, sizeB.position, playerLayer);
        Collider2D shotCollider = Physics2D.OverlapArea(sizeA.position, sizeB.position, shotLayer);
        if ((playerCollider != null) || (shotCollider != null))
        {
            if(playerCollider != null)
            {
                SoundManager.instance.PlaySound(meleeSound);
            }
            player.gauge += 1;
            hit = true;
        }
            
    }
    private void TouchUlti()
    {
        Collider2D shotCollider = Physics2D.OverlapArea(sizeA.position, sizeB.position, ultiLayer);
        if ((shotCollider != null))
        {
            player.gauge += 1;
            hit = true;
        }
            
    }

    private void OnDrawGizmosSelected()
    {
        if(wallCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(wallCheck.position, wallCheckRadius);
        }
        if(groundCheck != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        }
        if(playerCheck != null)
        {
            Gizmos.color = new Color(1.0f,0f,0.0f,0.25f);
            Gizmos.DrawSphere(playerCheck.position, playerCheckRadius);
        }
    }
}
