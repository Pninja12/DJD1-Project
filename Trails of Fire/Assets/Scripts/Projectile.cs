using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private LayerMask wallLayer;
    [SerializeField]
    private LayerMask bulletLayer;
    [SerializeField]
    private Transform sizeA;
    [SerializeField]
    private Transform sizeB;
    [SerializeField]
    private float timeToDestroy = 2f;
    [SerializeField]
    private bool ulti;
    

    private bool hit;
    private float direction;
    private Rigidbody2D rb;
    private Player player;
    private bool isInvincible = false;



    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            player = FindObjectOfType <Player>();
        }
        

    }

    void Start()
    {
        direction = player.direction;
    }

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
            Destroy(gameObject, 0.01f);
        } 
        //float movementSpeed = speed * Time.deltaTime * direction;
        if (direction <0) transform.rotation = Quaternion.Euler(0,180,0);
        else if(direction > 0) transform.rotation = Quaternion.identity;
        if(!ulti)TouchSomething();

        Vector3 velocity = rb.velocity;

        velocity.x = direction * speed;

        rb.velocity = velocity;
        
        Destroy(gameObject, timeToDestroy);
        
    }
    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;

        yield return new WaitForSeconds(0.01f);

        isInvincible = false;
    }

    private void TouchSomething()
    {
        Collider2D enemyCollider = Physics2D.OverlapArea(sizeA.position, sizeB.position, enemyLayer);
        Collider2D wallCollider = Physics2D.OverlapArea(sizeA.position, sizeB.position, wallLayer);
        Collider2D bulletCollider = Physics2D.OverlapArea(sizeA.position, sizeB.position, bulletLayer);
        if ((enemyCollider != null) || (wallCollider != null) || (bulletCollider != null))
        {
            hit = true;
        }
            
    }
}
