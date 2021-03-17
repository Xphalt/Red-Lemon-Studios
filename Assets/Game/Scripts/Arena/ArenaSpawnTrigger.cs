using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawnTrigger : MonoBehaviour
{
    public Transform spawnPos1;
    public Transform spawnPos2;
    public Transform spawnPos3;
    public Transform spawnPos4;

    public GameObject enemies;

    private bool bCanSpawnEnemies;

    private void Start()
    {
        bCanSpawnEnemies = false;
    }

    private void Update()
    {
        if (bCanSpawnEnemies)
        {
            enemies.SetActive(true);
            Debug.Log("Active");

            bCanSpawnEnemies = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collision");
            bCanSpawnEnemies = true;
        }
    }
}