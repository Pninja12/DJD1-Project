using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorUp : MonoBehaviour
{
    [SerializeField]
    private Transform sizeA;
    [SerializeField]
    private Transform sizeB;
    [SerializeField]
    private LayerMask playerLayer;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType <Player>();
        }
        if(PlayerIn())
        {
            player.health += 1;
            Destroy(gameObject);
        }
    }
    private bool PlayerIn()
    {
        Collider2D collider = Physics2D.OverlapArea(sizeA.position, sizeB.position, playerLayer);

        if (collider == null)
        {
            return false;
        }

        return true;
    }
}
