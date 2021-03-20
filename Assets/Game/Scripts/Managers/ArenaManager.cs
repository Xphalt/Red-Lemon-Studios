using System;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    #region Variables
    //reference to ArenaSpawnTrigger script___________________________________________________________Merged as this script was entirely dependant on that one
    
    public List<GameObject> enemies = new List<GameObject>();
    private List<Enemy> enemyScripts = new List<Enemy>();

    //enemies in arenas are now linked to their spawn positions_______________________________________If it's a 1-1 relationship, you don't need the spawn pos
    public List<Transform> enemySpawnPos = new List<Transform>();

    public GameObject relic;
    private RelicBase relicScript;
    public Transform relicSpawnPos;

    public bool bCanSpawnEnemies = true;

    internal bool bArenaComplete = false;
    internal bool bRelicCollected = false;

    #endregion

    #region UnityCallbacks
    //enemies in arenas are now moved to their spawn positions________________________________________Second for loop was unecessary
    private void Start()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].transform.position = enemySpawnPos[i].position; //Don't need to reference the transform of a transform
            enemyScripts.Add(enemies[i].GetComponent<Enemy>()); //Need reference to the 'killed' attribute
            enemies[i].SetActive(false);
        }
        relic.transform.position = relicSpawnPos.position;
        relicScript = relic.GetComponent<RelicBase>();
        relicScript.inArena = true;
        relic.SetActive(false);

        bArenaComplete = false;
        bRelicCollected = false;
        bCanSpawnEnemies = true;
    }

    private void Update()
    {
        //this code is an eyesore but it works for now_______________________________________________Fixed it
        if (!bArenaComplete)
        {
            bool enemiesRemaining = false;

            foreach (Enemy enemy in enemyScripts)
            {
                if (!enemy.killed)
                {
                    enemiesRemaining = true;
                    break;
                }
            }

            if (!enemiesRemaining)
            {
                bArenaComplete = true;
                relic.SetActive(true);
            }
        }

        else if (!bRelicCollected) bRelicCollected = relicScript.collected;

        //bool relicsRemaining = false; (In case of mulitple relics per arena)

        //foreach (RelicBase relic in relics)
        //{
        //    if (relic.inArena && !relic.collected)
        //    {
        //        relicsRemaining = true;
        //        break;
        //    }
        //}
    }
    #endregion

    public void ActivateEnemies()
    {
        if (bCanSpawnEnemies)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].SetActive(true);
                bCanSpawnEnemies = false;
            }
        }
    }

    #region Saving
    public void SaveArenaStatus(string saveID)
    {
        saveID = "SpawnTrigger" + saveID;
        SaveManager.UpdateSavedBool(saveID + "CanSpawn", bCanSpawnEnemies);
        SaveManager.UpdateSavedBool(saveID + "Complete", bArenaComplete);
    }

    public void LoadArenaStatus(string loadID)
    {
        loadID = "SpawnTrigger" + loadID;
        bCanSpawnEnemies = SaveManager.GetBool(loadID + "CanSpawn");
        bArenaComplete = SaveManager.GetBool(loadID + "Complete");
    }
    #endregion
}