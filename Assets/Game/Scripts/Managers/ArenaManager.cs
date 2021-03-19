using System;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    #region Variables
    //reference to ArenaSpawnTrigger script___________________________________________________________
    public ArenaSpawnTrigger arenaSpawnTrigger;

    //enemies in arenas are now linked to their spawn positions_______________________________________
    public List<Transform> enemyPos = new List<Transform>();
    public List<Transform> spawnPos = new List<Transform>();
    #endregion

    #region UnityCallbacks
    //enemies in arenas are now moved to their spawn positions________________________________________
    private void Start()
    {
        for (int i = 0; i < enemyPos.Count; i++)
        {
            for (int x = 0; x < spawnPos.Count; x++)
            {
                enemyPos[i].transform.position = spawnPos[i].transform.position;
            }
        }
    }

    private void Update()
    {
        //this code is an eyesore but it works for now_______________________________________________
        if (!arenaSpawnTrigger.bCanSpawnEnemies && !arenaSpawnTrigger.bArenaComplete)
        {
            if (!arenaSpawnTrigger.enemies[0].activeSelf)
            {
                if (!arenaSpawnTrigger.enemies[1].activeSelf)
                {
                    if (!arenaSpawnTrigger.enemies[2].activeSelf)
                    {
                        if (!arenaSpawnTrigger.enemies[3].activeSelf)
                        {
                            arenaSpawnTrigger.bArenaComplete = true;
                            Debug.Log("TEst");
                        }
                    }
                }
            }
        }
    }
    #endregion
}