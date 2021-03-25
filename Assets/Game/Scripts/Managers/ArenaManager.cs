using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaManager : MonoBehaviour
{
    #region Variables
    public bool AutoFind = false;

    public Player player;

    public List<GameObject> enemies = new List<GameObject>();
    private List<Enemy> enemyScripts = new List<Enemy>();

    //enemies in arenas are now linked to their spawn positions_______________________________________
    [Tooltip("Number of enemies/number of spawn positions rounded up = number of waves")]
    public List<Transform> enemySpawnPos = new List<Transform>();

    internal int waveCounter = 0;
    private int maxWaves;

    public float waveDelay;
    private float waveTimer = 0;

    public GameObject relic;
    private RelicBase relicScript;

    public string nextScene;

    public bool bWavesStarted = false;

    internal bool bEnemiesCleared = false;
    internal bool bRelicCollected = false;

    internal bool bCheckpointReady = false;

    public SFXScript sfxScript;
    public string enemiesDeadSound;
    public string enemySpawnSound;
    public string gameCompleteSound;

    #endregion

    #region UnityCallbacks
    //enemies in arenas are now moved to their spawn positions________________________________________
    private void Awake()
    {
        if (AutoFind)
        {
            Scene thisScene = SceneManager.GetActiveScene();
            enemies.Clear();
            foreach (Enemy newEnemy in Resources.FindObjectsOfTypeAll<Enemy>())
                if (newEnemy.gameObject.scene == thisScene) enemies.Add(newEnemy.gameObject);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemyScripts.Add(enemies[i].GetComponent<Enemy>());
            if (enemySpawnPos.Count > 0 && !enemyScripts[i].spawned) enemies[i].transform.position = enemySpawnPos[i % enemySpawnPos.Count].position;
            enemies[i].SetActive(enemyScripts[i].spawned && !enemyScripts[i].killed);
        }

        if (enemySpawnPos.Count > 0) maxWaves = Mathf.CeilToInt((float)enemies.Count / enemySpawnPos.Count);

        if (relic != null)
        {
            relicScript = relic.GetComponent<RelicBase>();
            relicScript.inArena = true;
        }
        else bRelicCollected = true;

        if (sfxScript == null) sfxScript = GameObject.FindGameObjectWithTag("SFXManager").GetComponent<SFXScript>();
    }

    private void Update()
    {
        bCheckpointReady = false;

        if (!bEnemiesCleared)
        {
            if (waveCounter < maxWaves && bWavesStarted)
            {
                waveTimer += Time.deltaTime;
                if (waveTimer > waveDelay) SpawnNextWave();
            }

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
                if (relic != null)
                {
                    relicScript.spawned = true;
                    relic.SetActive(true);
                }
                bCheckpointReady = true;

                sfxScript.PlaySFX2D(enemiesDeadSound);
            }
        }

        else if (!bRelicCollected)
        {
            bRelicCollected = relicScript.collected;
            bCheckpointReady = bRelicCollected;
        }


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

    public void SpawnNextWave()
    {
        if (waveCounter < maxWaves) // && !bArenaComplete could possibly fix a bug
        {
            for (int e = waveCounter * enemySpawnPos.Count; e < (waveCounter + 1) * enemySpawnPos.Count; e++)
            {
                if (e >= enemies.Count) break;
                enemies[e].SetActive(true);
                enemyScripts[e].spawned = true;
                sfxScript.PlaySFX2D(enemySpawnSound);
            }

            bWavesStarted = true;
            waveCounter++;
            waveTimer = 0;
        }
    }

    public void TransitionLevel()
    {
        if (bEnemiesCleared && bRelicCollected)
        {
            if (nextScene != "") SceneManager.LoadScene(nextScene);
            else
            {
                sfxScript.PlaySFX2D(gameCompleteSound);
                player.CompleteGame();
            }
        }
    }

    #region Saving
    public void SaveArenaStatus(string saveID)
    {
        saveID = "ArenaManager" + saveID;
        SaveManager.UpdateSavedBool(saveID + "WavesStarted", bWavesStarted);
        SaveManager.UpdateSavedBool(saveID + "Complete", bEnemiesCleared);
        SaveManager.UpdateSavedInt(saveID + "WaveCounter", waveCounter);
    }

    public void LoadArenaStatus(string loadID)
    {
        loadID = "ArenaManager" + loadID;
        bWavesStarted = SaveManager.GetBool(loadID + "WavesStarted");
        bEnemiesCleared = SaveManager.GetBool(loadID + "Complete");
        waveCounter = SaveManager.GetInt(loadID + "WaveCounter");
    }
    #endregion
}