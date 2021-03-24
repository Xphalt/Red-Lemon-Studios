using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionTrigger : MonoBehaviour
{
    public ArenaManager arenaManager;

    private bool active = false;

    private void Update()
    {
        if (!active)
        {
            active = arenaManager.bEnemiesCleared && arenaManager.bRelicCollected;
            if (active)
            {
                // trigger visual change (colour/animation etc.)
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            arenaManager.TransitionLevel();
        }
    }
}