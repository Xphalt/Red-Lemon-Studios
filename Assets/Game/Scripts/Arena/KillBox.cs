using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    public List<Transform> respawnPos;
    public int damage;
    public bool playerOnly;
    public bool AutoFind;

    private void Awake()
    {
        if (AutoFind)
        {
            respawnPos.Clear();
            foreach (GameObject respawn in GameObject.FindGameObjectsWithTag("Respawn"))
            {
                if (respawn.transform.parent == transform) respawnPos.Add(respawn.transform);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Player playerScript))
        {
            playerScript.TakeDamage(damage);
            Vector3 point = collision.GetContact(0).point;
            Vector3 closestRespawn = respawnPos[0].position;
            float closestDistance = (respawnPos[0].position - point).magnitude;
            foreach (Transform nextRespawn in respawnPos)
            {
                if ((nextRespawn.position - point).magnitude < closestDistance)
                {
                    closestRespawn = nextRespawn.position;
                    closestDistance = (nextRespawn.position - point).magnitude;
                }
            }
            playerScript.transform.position = closestRespawn;
            collision.rigidbody.velocity = Vector3.zero;
        }
        else if (!playerOnly && collision.gameObject.TryGetComponent(out Enemy enemyScript)) enemyScript.Die();
    }
}
