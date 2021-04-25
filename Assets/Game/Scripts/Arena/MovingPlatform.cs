using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 moveVelocity;
    private float maxDistance;
    private int direction = 1;

    public Vector3 maxMovement;
    public float moveSpeed;

    void Start()
    {
        moveVelocity = maxMovement.normalized * moveSpeed;
        startPos = transform.localPosition;
        maxDistance = maxMovement.magnitude;
    }

    private void Update()
    {
        transform.Translate(moveVelocity * Time.deltaTime); //Rigidbody is kinematic
        if ((transform.localPosition - startPos).magnitude > maxDistance)
        {
            moveVelocity *= -1;
            transform.localPosition = startPos;
            transform.Translate(maxMovement * direction);
            direction *= -1;
        }
    }
}
