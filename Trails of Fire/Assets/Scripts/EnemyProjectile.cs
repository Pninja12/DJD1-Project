using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
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
    private float timeToDestroy = 3f;
    

    private bool hit;
    private Vector2 direction;
    private int facing;
    private Rigidbody2D rb;
    private ShootingPlacePlayer shootingPlacePlayer;
    private bool isInvincible = false;



    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (shootingPlacePlayer == null)
        {
            shootingPlacePlayer = FindObjectOfType <ShootingPlacePlayer>();
        }
        

    }

    void Start()
    {
        direction = (shootingPlacePlayer.transform.position - transform.position).normalized * speed;
        rb.velocity = new Vector2(direction.x, direction.y);
        float whichDirection = shootingPlacePlayer.transform.position.x - transform.position.x;

        transform.right = shootingPlacePlayer.transform.position - transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
            Destroy(gameObject, 0.01f);
        }
        //float movementSpeed = speed * Time.deltaTime * direction;
        
        

        TouchSomething();

        
        
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
        Collider2D bulletCollider = Physics2D.OverlapArea(sizeA.position, sizeB.position, bulletLayer);
        if ((enemyCollider != null) || (bulletCollider != null))
        {
            hit = true;
        }
            
    }
}