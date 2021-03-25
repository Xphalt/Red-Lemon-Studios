﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    public Transform respawnPos;
    public int damage;
    public bool playerOnly;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out Player playerScript))
        {
            playerScript.TakeDamage(damage);
            playerScript.transform.position = respawnPos.position;
            collision.attachedRigidbody.velocity = Vector3.zero;
        }
        else if (!playerOnly && collision.TryGetComponent(out Enemy enemyScript)) enemyScript.Die();
    }
}
