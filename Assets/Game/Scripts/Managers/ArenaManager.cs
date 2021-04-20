using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaManager : MonoBehaviour
{
    #region Variables
    private List<Enemy> enemyScripts = new List<Enemy>();
    private RelicBase startRelicScript;
    private RelicBase endRelicScript;
    private int maxWaves;
    private float waveTimer = 0;

    public Player player;
    public GUI_Manager userInterface;
    public List<GameObject> enemies = new List<GameObject>();
    [Tooltip("Number of enemies/number of spawn positions rounded up = number of waves")]
    public List<Transform> enemySpawnPos = new List<Transform>();
    public GameObject startRelic;
    public GameObject endRelic;
    public Transition transition;
    public SFXScript sfxScript;
    public string enemiesDeadSound;
    public string enemySpawnSound;
    public string gameCompleteSound;
    public bool AutoFind = false;
    public float waveDelay;
    public string nextScene;

    internal int waveCounter = 0;
    internal bool bWavesStarted = false;
    internal bool bStartRelicCollected = false;
    internal bool bEnemiesCleared = false;
    internal bool bEndRelicCollected = false;
    internal bool bCheckpointReady = false;
    #endregion

    #region UnityCallbacks
    private void Awake()
    {
        if (AutoFind)
        {
            Scene thisScene = SceneManager.GetActiveScene();
            enemies.Clear();
            enemySpawnPos.Clear();
            foreach (Enemy newEnemy in Resources.FindObjectsOfTypeAll<Enemy>())
                if (newEnemy.gameObject.scene == thisScene) enemies.Add(newEnemy.gameObject);

            foreach (GameObject newSpawn in GameObject.FindGameObjectsWithTag("SpawnPos")) enemySpawnPos.Add(newSpawn.transform);
        }

        if (startRelic != null)
        {
            startRelicScript = startRelic.GetComponent<RelicBase>();
            startRelicScript.inArena = true;
            startRelicScript.spawned = true;
        }
        else bStartRelicCollected = true;

        for (int i = 0; i < enemies.Count; i++)
        {
            enemyScripts.Add(enemies[i].GetComponent<Enemy>());
            if (enemySpawnPos.Count > 0 && !enemyScripts[i].spawned) enemies[i].transform.position = enemySpawnPos[i % enemySpawnPos.Count].position;
            enemies[i].SetActive(enemyScripts[i].spawned && !enemyScripts[i].killed);
        }

        if (enemySpawnPos.Count > 0) maxWaves = Mathf.CeilToInt((float)enemies.Count / enemySpawnPos.Count);

        if (endRelic != null)
        {
            endRelicScript = endRelic.GetComponent<RelicBase>();
            endRelicScript.inArena = true;
        }
        else bEndRelicCollected = true;

        if (sfxScript == null) sfxScript = FindObjectOfType<SFXScript>();
    }

    private void Update()
    {
        bCheckpointReady = false;

        if (!bStartRelicCollected)
        {
            bStartRelicCollected = startRelicScript.collected;
            bCheckpointReady = bStartRelicCollected;
        }

        else if (!bEnemiesCleared && bWavesStarted)
        {
            if (waveCounter < maxWaves)
            {
                waveTimer += Time.deltaTime;
                userInterface.UpdateWaveTimer(Mathf.CeilToInt(waveDelay - waveTimer));
                if (waveTimer > waveDelay) SpawnNextWave();
            }

            int enemiesSpawned = 0;
            int enemiesRemaining = 0;
            bool enemiesCleared = true;

            foreach (Enemy enemy in enemyScripts)
            {
                if (!enemy.killed) enemiesCleared = false;
                if (enemy.spawned)
                {
                    enemiesSpawned++;
                    if (!enemy.killed) enemiesRemaining++;
                }
            }

            if (enemiesCleared)
            {
                bEnemiesCleared = true;
                if (endRelic != null)
                {
                    endRelicScript.spawned = true;
                    endRelic.SetActive(true);
                }
                bCheckpointReady = true;

                sfxScript.PlaySFX2D(enemiesDeadSound);

                userInterface.ClearEnemyCounter();
            }

            else userInterface.UpdateEnemyCounter(enemiesRemaining, enemiesSpawned);
        }

        else if (!bEndRelicCollected)
        {
            bEndRelicCollected = endRelicScript.collected;
            bCheckpointReady = bEndRelicCollected;
        }
    }
    #endregion

    public void SpawnNextWave()
    {
        if (waveCounter < maxWaves && bStartRelicCollected)
        {
            for (int e = waveCounter * enemySpawnPos.Count; e < (waveCounter + 1) * enemySpawnPos.Count; e++)
            {
                if (e >= enemies.Count) break;
                enemies[e].SetActive(true);
                enemyScripts[e].spawned = true;
                sfxScript.PlaySFX2D(enemySpawnSound);
            }

            if (!bWavesStarted)
            {
                userInterface.ActivateEnemyCounter();
                bWavesStarted = true;
            }
            waveCounter++;
            waveTimer = 0;

            userInterface.UpdateWaveCounter(waveCounter, maxWaves);
        }
    }

    public void TransitionLevel()
    {
        if (bEnemiesCleared && bEndRelicCollected)
        {
            if (nextScene != "") StartCoroutine(transition.LoadLevel(nextScene));
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

        if (bWavesStarted && !bEnemiesCleared)
        {
            userInterface.ActivateEnemyCounter();
            userInterface.UpdateWaveCounter(waveCounter, maxWaves);
        }
        else userInterface.ClearEnemyCounter();
    }
    #endregion
}