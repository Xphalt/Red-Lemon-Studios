using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EnumHelper;
using static SaveManager;
public class ArenaManager : MonoBehaviour
{
    public string ArenaName;
    private int checkpointCounter = 0;
    private string checkpointID;

    private List<string> previousCheckpoints = new List<string>();
    private string previousLevelCheckpoint;

    public bool restart; //Clears all saved values
    public bool resetArena; //Clears all checkpoints saved in current arena

    public bool autoFind = true;

    public List<Enemy> arenaEnemies;
    public List<RelicBase> arenaRelics;
    public List<PickUpBase> arenaPickUps;
    public Player arenaPlayer;

    private void Awake()
    {
        checkpointID = ArenaName + checkpointCounter.ToString();
        SaveManager.LoadFromFile();
        if (ArenaName == "") ArenaName = SceneManager.GetActiveScene().name;

        foreach (RelicBase arenarelic in arenaRelics) arenarelic.Awake();
    }

    void Start()
    {
        if (autoFind)
        {
            arenaEnemies.Clear();
            arenaPickUps.Clear();
            foreach (GameObject newEnemy in GameObject.FindGameObjectsWithTag("Enemy")) arenaEnemies.Add(newEnemy.GetComponent<Enemy>());
            foreach (GameObject newPickup in GameObject.FindGameObjectsWithTag("PickUp")) arenaPickUps.Add(newPickup.GetComponent<PickUpBase>());
        }

        if (SaveManager.HasStringList("PreviousCheckpoints"))
        {
            previousCheckpoints = SaveManager.GetStringList("PreviousCheckpoints");

            for (int checkpointIndex = previousCheckpoints.Count - 1; checkpointIndex >= 0; checkpointIndex--)
            {
                previousLevelCheckpoint = previousCheckpoints[checkpointIndex];
                if (!previousLevelCheckpoint.Contains(ArenaName)) break;
                else previousLevelCheckpoint = "";
            }
        }
        if (!restart) Load();
    }

    void Update()
    {
        if (checkpointCounter == 0)
        {
            bool arenaFinished = true;

            foreach (Enemy arenaenemy in arenaEnemies)
            {
                if (!arenaenemy.killed)
                {
                    arenaFinished = false;
                    break;
                }
            }

            if (arenaFinished) Save();
        }

        if (arenaPlayer.killed)
        {
            Load();
        }
    }

    public void Save()
    {
        checkpointCounter++;
        checkpointID = ArenaName + checkpointCounter.ToString();
        previousCheckpoints.Add(checkpointID);

        SaveManager.UpdateSavedInt(ArenaName + "CheckpointCounter", checkpointCounter);
        SaveManager.UpdateSavedString(ArenaName + "LastCheckpointID", checkpointID);
        SaveManager.UpdateSavedString("LastOverallCheckpointID", checkpointID);
        SaveManager.UpdateSavedStringList("PreviousCheckpoints", previousCheckpoints);

        for (int e = 0; e < arenaEnemies.Count; e++)
        {
            arenaEnemies[e].SaveEnemy(e.ToString() + checkpointID);
        }

        for (int p = 0; p < arenaPickUps.Count; p++)
        {
            arenaPickUps[p].SavePickUp(p.ToString() + checkpointID);
        }

        for (int r = 0; r < arenaRelics.Count; r++)
        {
            arenaRelics[r].SaveRelic(checkpointID);
        }

        arenaPlayer.SaveStats(checkpointID);

        SaveManager.UpdateSavedString("LastSavedLevel", SceneManager.GetActiveScene().name);
        SaveManager.UpdateSavedInt("LastSavedLevel", SceneManager.GetActiveScene().buildIndex);
    }

    public void Load(string checkpointOverride="")
    {
        if (!SaveManager.HasString("LastOverallCheckpointID")) return;

        string loadID = (checkpointOverride == "") ? SaveManager.GetString("LastOverallCheckpointID") : checkpointOverride;
        string arenaLoadID = "";

        if (!resetArena && SaveManager.HasString(ArenaName + "LastCheckpointID"))
        {
            arenaLoadID = SaveManager.GetString(ArenaName + "LastCheckpointID");
            checkpointCounter = SaveManager.GetInt(ArenaName + "CheckpointCounter");

            for (int e = 0; e < arenaEnemies.Count; e++)
            {
                arenaEnemies[e].LoadEnemy(e.ToString() + arenaLoadID);
            }

            for (int p = 0; p < arenaPickUps.Count; p++)
            {
                arenaPickUps[p].LoadPickUp(p.ToString() + arenaLoadID);
            }
        }
        else checkpointCounter = 0;

        string playerLoadID = (resetArena) ? previousLevelCheckpoint : loadID;

        if (playerLoadID != "")
        {
            for (int r = 0; r < arenaRelics.Count; r++)
            {
                arenaRelics[r].LoadRelic(playerLoadID); // if (!(resetArena && arenaRelics[r].inArena)) IN CASE RELICS GET RESET WITH ARENA
            }
            arenaPlayer.LoadStats(playerLoadID, arenaLoadID);
        }
    }

    private void OnDestroy()
    {
        SaveManager.SaveToFile();
    }
}
