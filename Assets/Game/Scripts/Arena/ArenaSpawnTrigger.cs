using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawnTrigger : MonoBehaviour
{
    public ArenaManager arenaManager;
    public bool active = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && active)
        {
            arenaManager.SpawnNextWave();
            active = false; //could SetActive(false) if we have no further use for it
        }
    }
}