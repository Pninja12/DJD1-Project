using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    [SerializeField]
    private Transform followObject;
    [SerializeField]
    private float speed = 0.5f;
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newPosition = followObject.position;

        newPosition.z = transform.position.z;

        Vector3 delta = newPosition - transform.position;

        transform.position = transform.position + delta * speed;
    }
}
