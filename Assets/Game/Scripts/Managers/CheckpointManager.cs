﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static EnumHelper;
using static SaveManager;
public class CheckpointManager : MonoBehaviour
{
    Scene thisScene;

    private List<string> previousCheckpoints = new List<string>();
    private int checkpointsReached = 0;
    private string checkpointAtStart;

    public Transform revisitSpawnPoint;
    public List<Enemy> arenaEnemies;
    public List<RelicBase> arenaRelics;
    public List<PickUpBase> arenaPickUps;
    public List<Interactable_Items> arenaInteractables;
    public Player arenaPlayer;
    public ArenaManager arenaManager = null;
    public SFXScript sfxScript = null;
    public bool newGame = false; //If true, clears all saved values
    public bool resetArena = false; //If true, clears all checkpoints saved in current arena
    public bool autoFind = true;
    public string ArenaName;
    public Image saveIcon;
    public float saveDuration;

    private bool saving = false;
    private float saveTimer = 0;
    private Color iconColour;

    private void Awake()
    {
        if (arenaManager == null) arenaManager = GetComponent<ArenaManager>();
        if (sfxScript == null) sfxScript = FindObjectOfType<SFXScript>();

        thisScene = SceneManager.GetActiveScene();
        if (ArenaName == "") ArenaName = thisScene.name;

        if (!SaveManager.loaded) SaveManager.LoadFromFile();

        newGame = !SaveManager.HasString("LastOverallCheckpointID") || newGame;
        resetArena = !SaveManager.HasString(ArenaName + "LastCheckpointID") || resetArena;

        if (!newGame) checkpointAtStart = GetCheckpointAtStart();
        else if (SaveManager.HasBool("PlayerSaved")) SaveManager.ClearSaves();

        foreach (RelicBase arenarelic in arenaRelics) arenarelic.Awake();

        if (autoFind)
        {
            FindArenaItems();
        }

        iconColour = saveIcon.color;
    }

    private void Start()
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
        arenaInteractables.Clear();
        foreach (Enemy newEnemy in Resources.FindObjectsOfTypeAll<Enemy>())
            if (newEnemy.gameObject.scene == thisScene) arenaEnemies.Add(newEnemy);

        foreach (PickUpBase newPickup in Resources.FindObjectsOfTypeAll<PickUpBase>())
            if (newPickup.gameObject.scene == thisScene) arenaPickUps.Add(newPickup);

        foreach (Interactable_Items newInteractable in Resources.FindObjectsOfTypeAll<Interactable_Items>())
            if (newInteractable.gameObject.scene == thisScene) arenaInteractables.Add(newInteractable);

        arenaEnemies = arenaEnemies.OrderBy(enemy => enemy.name).ToList();
        arenaPickUps = arenaPickUps.OrderBy(pickup => pickup.name).ToList();
        arenaInteractables = arenaInteractables.OrderBy(interactable => interactable.name).ToList();
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
        if (arenaManager.bCheckpointReady) Save();

        if (saving)
        {
            saveTimer += Time.deltaTime;
            if (saveTimer > saveDuration / 2)
            {
                saveIcon.color = Color.Lerp(iconColour, Color.clear, saveTimer * 2 - saveDuration);
                if (saveTimer > saveDuration) HideIcon();
            }
        }
    }

    public void Save(string checkpointOverride = "")
    {
        ShowIcon();
        
        string saveID = (checkpointOverride == "") ? ArenaName + checkpointsReached.ToString() : checkpointOverride;
        if (previousCheckpoints.Contains(saveID)) previousCheckpoints.Remove(saveID);
        previousCheckpoints.Add(saveID);

        SaveManager.UpdateSavedInt(saveID + "CheckpointsReached", ++checkpointsReached);
        SaveManager.UpdateSavedString(saveID + "Scene", thisScene.name);
        SaveManager.UpdateSavedInt(saveID + "Scene", thisScene.buildIndex);

        SaveManager.UpdateSavedString(ArenaName + "LastCheckpointID", saveID);
        SaveManager.UpdateSavedString("LastOverallCheckpointID", saveID);
        SaveManager.UpdateSavedStringList("PreviousCheckpoints", previousCheckpoints);

        for (int e = 0; e < arenaEnemies.Count; e++) arenaEnemies[e].SaveEnemy(e.ToString() + saveID);

        for (int i = 0; i < arenaInteractables.Count; i++) arenaInteractables[i].SaveInteractable(i.ToString() + saveID);
        
        for (int p = 0; p < arenaPickUps.Count; p++) arenaPickUps[p].SavePickUp(p.ToString() + saveID);

        for (int r = 0; r < arenaRelics.Count; r++) arenaRelics[r].SaveRelic(saveID);

        arenaPlayer.SaveStats(saveID);

        arenaManager.SaveArenaStatus(saveID);

        SaveManager.UpdateSavedString("LastSavedScene", thisScene.name);
        SaveManager.UpdateSavedInt("LastSavedScene", thisScene.buildIndex);

        SaveManager.SaveToFile();
    }

    public void Restart()
    {
        Load(true);
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

            for (int e = 0; e < arenaEnemies.Count; e++) arenaEnemies[e].LoadEnemy(e.ToString() + arenaLoadID);

            for (int i = 0; i < arenaInteractables.Count; i++) arenaInteractables[i].LoadInteractable(i.ToString() + arenaLoadID);

            for (int p = 0; p < arenaPickUps.Count; p++) arenaPickUps[p].LoadPickUp(p.ToString() + arenaLoadID);

            arenaManager.LoadArenaStatus(arenaLoadID);
        }
        else checkpointsReached = 0;

        string playerLoadID = (loadArena) ? loadID : checkpointAtStart;

        if (playerLoadID != "")
        {
            for (int r = 0; r < arenaRelics.Count; r++)
            {
                if (!arenaRelics[r].inArena) arenaRelics[r].LoadRelic(playerLoadID);
                else if (loadArena) arenaRelics[r].LoadRelic(arenaLoadID);
            }
            arenaPlayer.LoadStats(playerLoadID, arenaLoadID);
            if (arenaLoadID.Contains("End")) arenaPlayer.transform.position = revisitSpawnPoint.position;
        }

        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet")) Destroy(bullet);
        sfxScript.StopSFX();
    }

    public void ShowIcon()
    {
        saving = true;
        saveTimer = 0;
        saveIcon.gameObject.SetActive(true);
        saveIcon.color = iconColour;
    }

    public void HideIcon()
    {
        saving = false;
        saveTimer = 0;
        saveIcon.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        SaveManager.SaveToFile();
    }
}