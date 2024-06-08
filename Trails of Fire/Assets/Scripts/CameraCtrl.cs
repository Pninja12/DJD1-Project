using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    [SerializeField]
    private Transform followObject;
    [SerializeField]
    private float speed = 0.5f;
    Player player;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (followObject == null)
        {
            followObject = FindPlayer();
        }
        Vector3 newPosition = followObject.position;

        newPosition.z = transform.position.z;

        Vector3 delta = newPosition - transform.position;

        transform.position = transform.position + delta * speed;
    }

    private Transform FindPlayer()
    {
        player = FindObjectOfType <Player>();
        return player.transform;
    }
}
