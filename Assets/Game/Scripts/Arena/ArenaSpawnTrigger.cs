using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawnTrigger : MonoBehaviour
{
    public ArenaManager arenaManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            arenaManager.ActivateEnemies();
        }
    }
}