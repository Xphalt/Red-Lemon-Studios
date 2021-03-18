using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawnTrigger : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();
    public bool bArenaComplete;
    public bool bCanSpawnEnemies;

    private void Start()
    {
        bArenaComplete = false;
        bCanSpawnEnemies = true;

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (bCanSpawnEnemies)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].SetActive(true);
                    bCanSpawnEnemies = false;
                }
            }
        }
    }
}