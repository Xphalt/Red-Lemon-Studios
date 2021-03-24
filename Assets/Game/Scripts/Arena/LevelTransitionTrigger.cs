using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionTrigger : MonoBehaviour
{
    public ArenaManager arenaManager;
    public List<GameObject> images;
    private bool active = false;

    private void Update()
    {
        if (!active)
        {
            active = arenaManager.bEnemiesCleared && arenaManager.bRelicCollected;
            if (active)
            {
                foreach (GameObject image in images) image.SetActive(true);
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