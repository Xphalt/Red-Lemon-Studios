﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EnumHelper;
using static SaveManager;
public class CheckpointManager : MonoBehaviour
{
    Scene thisScene;
    public string ArenaName;
    private int checkpointsReached = 0;

    private List<string> previousCheckpoints = new List<string>();
    private string checkpointAtStart;

    public bool newGame = false; //If true, clears all saved values
    public bool resetArena = false; //If true, clears all checkpoints saved in current arena

    public bool autoFind = true;

    public List<Enemy> arenaEnemies;
    public List<RelicBase> arenaRelics;
    public List<PickUpBase> arenaPickUps;
    public Player arenaPlayer;

    private void Awake()
    {
        thisScene = SceneManager.GetActiveScene();
        if (ArenaName == "") ArenaName = thisScene.name;
        
        SaveManager.LoadFromFile();

        checkpointAtStart = GetCheckpointAtStart();
        
        if (!newGame) newGame = !SaveManager.HasString("LastOverallCheckpointID");
        if (!resetArena) resetArena = !SaveManager.HasString(ArenaName + "LastCheckpointID");

        foreach (RelicBase arenarelic in arenaRelics) arenarelic.Awake();

        if (autoFind)
        {
            FindArenaItems();
        }
    }

    void Start()
    {
        if (!newGame) Load(!resetArena);
        if (resetArena || newGame)
        {
            checkpointsReached = -1;
            Save();
        }
    }

    private void FindArenaItems()
    {
        arenaEnemies.Clear();
        arenaPickUps.Clear();
        foreach (Enemy newEnemy in Resources.FindObjectsOfTypeAll<Enemy>())
            if (newEnemy.gameObject.scene == thisScene) arenaEnemies.Add(newEnemy);

        foreach (PickUpBase newPickup in Resources.FindObjectsOfTypeAll<PickUpBase>())
            if (newPickup.gameObject.scene == thisScene) arenaPickUps.Add(newPickup);
    }

    private string GetCheckpointAtStart()
    {
        string previousSceneCheckpoint = "";
        if (SaveManager.HasStringList("PreviousCheckpoints"))
        {
            previousCheckpoints = SaveManager.GetStringList("PreviousCheckpoints");
            for (int checkpointIndex = previousCheckpoints.Count - 1; checkpointIndex >= 0; checkpointIndex--)
            {
                previousSceneCheckpoint = previousCheckpoints[checkpointIndex];
                if (!previousSceneCheckpoint.Contains(ArenaName)) return previousSceneCheckpoint;
                else previousSceneCheckpoint = "";
            }
        }
        return previousSceneCheckpoint;
    }

    void Update()
    {
        if (checkpointsReached == 0)
        {
            bool enemiesRemaining = false;

            foreach (Enemy arenaenemy in arenaEnemies)
            {
                if (!arenaenemy.killed)
                {
                    enemiesRemaining = true;
                    break;
                }
            }

            if (!enemiesRemaining) Save();
        }

        if (arenaPlayer.killed) Load();
    }

    public void Save(string checkpointOverride = "")
    {
        string saveID = (checkpointOverride == "") ? ArenaName + checkpointsReached.ToString() : checkpointOverride;
        if (previousCheckpoints.Contains(saveID)) previousCheckpoints.Remove(saveID);
        previousCheckpoints.Add(saveID);

        SaveManager.UpdateSavedInt(saveID + "CheckpointsReached", ++checkpointsReached);
        SaveManager.UpdateSavedString(saveID + "Scene", thisScene.name);
        SaveManager.UpdateSavedInt(saveID + "Scene", thisScene.buildIndex);

        SaveManager.UpdateSavedString(ArenaName + "LastCheckpointID", saveID);
        SaveManager.UpdateSavedString("LastOverallCheckpointID", saveID);
        SaveManager.UpdateSavedStringList("PreviousCheckpoints", previousCheckpoints);

        for (int e = 0; e < arenaEnemies.Count; e++)
        {
            arenaEnemies[e].SaveEnemy(e.ToString() + saveID);
        }

        for (int p = 0; p < arenaPickUps.Count; p++)
        {
            arenaPickUps[p].SavePickUp(p.ToString() + saveID);
        }

        for (int r = 0; r < arenaRelics.Count; r++)
        {
            arenaRelics[r].SaveRelic(saveID);
        }

        arenaPlayer.SaveStats(saveID);

        SaveManager.UpdateSavedString("LastSavedScene", thisScene.name);
        SaveManager.UpdateSavedInt("LastSavedScene", thisScene.buildIndex);

        SaveManager.SaveToFile();
    }

    public void Load(bool loadArena=true, string checkpointOverride="")
    {
        if (!SaveManager.HasString("LastOverallCheckpointID")) return;

        string loadID = (checkpointOverride == "") ? SaveManager.GetString("LastOverallCheckpointID") : checkpointOverride;
        string arenaLoadID = "";

        if (loadArena)
        {
            arenaLoadID = (loadID.Contains(ArenaName)) ? loadID : SaveManager.GetString(ArenaName + "LastCheckpointID");
            checkpointsReached = SaveManager.GetInt(arenaLoadID + "CheckpointsReached");

            for (int e = 0; e < arenaEnemies.Count; e++)
            {
                arenaEnemies[e].LoadEnemy(e.ToString() + arenaLoadID);
            }

            for (int p = 0; p < arenaPickUps.Count; p++)
            {
                arenaPickUps[p].LoadPickUp(p.ToString() + arenaLoadID);
            }
        }
        else checkpointsReached = 0;

        string playerLoadID = (loadArena) ? loadID : checkpointAtStart;

        if (playerLoadID != "")
        {
            for (int r = 0; r < arenaRelics.Count; r++)
            {
                arenaRelics[r].LoadRelic(playerLoadID); // if (!(resetArena && arenaRelics[r].inArena)) IN CASE RELICS GET RESET WITH ARENA
            }
            arenaPlayer.LoadStats(playerLoadID, arenaLoadID);
        }

        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet")) Destroy(bullet);
        foreach (GameObject sfx in GameObject.FindGameObjectsWithTag("SFX")) Destroy(sfx);
    }

    private void OnDestroy()
    {
        SaveManager.SaveToFile();
    }
}