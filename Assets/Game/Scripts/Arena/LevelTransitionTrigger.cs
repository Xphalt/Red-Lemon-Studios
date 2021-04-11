using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionTrigger : MonoBehaviour
{
    public ArenaManager arenaManager;
    public CheckpointManager checkpointManager = null;
    public Transition transition;
    public List<GameObject> images;
    private bool active = false;

    public AudioSource audioSource;

    private void Update()
    {
        if (!active)
        {
            audioSource.Pause();
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
        if (other.gameObject.CompareTag("Player"))
        {
            if (checkpointManager != null) checkpointManager.Save(checkpointManager.ArenaName + "End");
            StartCoroutine(transition.LoadLevel());
        }
    }
}