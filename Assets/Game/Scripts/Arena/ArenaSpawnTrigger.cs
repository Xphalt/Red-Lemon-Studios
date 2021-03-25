using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawnTrigger : MonoBehaviour
{
    public ArenaManager arenaManager;
    public bool active = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && arenaManager.waveCounter == 0)
        {
            arenaManager.SpawnNextWave();
        }
    }
}