﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 maxMovement;
    public float moveSpeed;
    private Vector3 startPos;

    private Vector3 moveVelocity;

    void Start()
    {
        moveVelocity = maxMovement.normalized * moveSpeed;
        startPos = transform.position;
    }

    private void Update()
    {
        transform.Translate(moveVelocity * Time.deltaTime); //Apologise for the translating but in this case is' easier to not have a rigidbody
        if ((transform.position - startPos).magnitude > maxMovement.magnitude) moveVelocity *= -1;
    }
}
