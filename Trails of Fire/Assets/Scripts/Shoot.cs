using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Shoot : MonoBehaviour
{
    [SerializeField]
    private float speed = 4.5f;
    [SerializeField]
    private float timeToDestroy = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.position += -transform.right * Time.deltaTime * speed;
        Destroy(gameObject, timeToDestroy);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
