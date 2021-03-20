using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaManager : MonoBehaviour
{
    #region Variables
    //reference to ArenaSpawnTrigger script___________________________________________________________
    
    public List<GameObject> enemies = new List<GameObject>();
    private List<Enemy> enemyScripts = new List<Enemy>();

    //enemies in arenas are now linked to their spawn positions_______________________________________
    public List<Transform> enemySpawnPos = new List<Transform>();

    public GameObject relic;
    private RelicBase relicScript;
    public Transform relicSpawnPos;

    public string nextScene;

    public bool bCanSpawnEnemies = true;

    internal bool bEnemiesCleared = false;
    internal bool bRelicCollected = false;

    #endregion

    #region UnityCallbacks
    //enemies in arenas are now moved to their spawn positions________________________________________
    private void Start()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyScripts.Add(enemies[i].GetComponent<Enemy>());
            if (!enemyScripts[i].spawned) enemies[i].transform.position = enemySpawnPos[i].position;
            enemies[i].SetActive(enemyScripts[i].spawned && !enemyScripts[i].killed);
        }

        relic.transform.position = relicSpawnPos.position;
        relicScript = relic.GetComponent<RelicBase>();
        relicScript.inArena = true;

        if (nextScene == "") nextScene = SceneManager.GetSceneAt((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings).name;
    }

    private void Update()
    {
        if (!bEnemiesCleared)
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
                bEnemiesCleared = true;
                relicScript.spawned = true;
                relic.SetActive(true);
            }
        }

        if (!bRelicCollected) bRelicCollected = relicScript.collected;

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
        if (bCanSpawnEnemies) // && !bArenaComplete could possibly fix a bug
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].SetActive(true);
                enemyScripts[i].spawned = true;
                bCanSpawnEnemies = false;
            }
        }
    }

    public void TransitionLevel()
    {
        if (bEnemiesCleared && bRelicCollected)
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    #region Saving
    public void SaveArenaStatus(string saveID)
    {
        saveID = "SpawnTrigger" + saveID;
        SaveManager.UpdateSavedBool(saveID + "CanSpawn", bCanSpawnEnemies);
        SaveManager.UpdateSavedBool(saveID + "Complete", bEnemiesCleared);
    }

    public void LoadArenaStatus(string loadID)
    {
        loadID = "SpawnTrigger" + loadID;
        bCanSpawnEnemies = SaveManager.GetBool(loadID + "CanSpawn");
        bEnemiesCleared = SaveManager.GetBool(loadID + "Complete");
    }
    #endregion
}