using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionTrigger : MonoBehaviour
{
    private bool active = false;

    public ArenaManager arenaManager;
    public CheckpointManager checkpointManager = null;
    public List<GameObject> images;
    public AudioSource audioSource;

    private void Update()
    {
        if (!active)
        {
            if (audioSource.isPlaying) audioSource.Stop();
            active = arenaManager.bEnemiesCleared && arenaManager.bEndRelicCollected;
            if (active)
            {
                foreach (GameObject image in images) image.SetActive(true);
                audioSource.Play();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && active)
        {
            if (checkpointManager != null) checkpointManager.Save(checkpointManager.ArenaName + "End");
            arenaManager.TransitionLevel();
        }
    }
}