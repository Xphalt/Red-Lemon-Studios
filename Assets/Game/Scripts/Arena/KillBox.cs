using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    public Transform respawnPos;
    public int damage;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out Player playerScript))
        {
            playerScript.TakeDamage(damage);
            playerScript.transform.position = respawnPos.position;
        }
        else if (collision.TryGetComponent(out Enemy enemyScript)) enemyScript.Die();
    }
}
