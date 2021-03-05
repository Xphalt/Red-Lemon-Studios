using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 maxMovement;
    public float moveSpeed;
    private Rigidbody platformRigid;
    private Vector3 startPos;

    void Start()
    {
        platformRigid = GetComponent<Rigidbody>();
        platformRigid.velocity = maxMovement.normalized * moveSpeed;
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        if ((transform.position - startPos).magnitude > maxMovement.magnitude) platformRigid.velocity *= -1;
    }
}
