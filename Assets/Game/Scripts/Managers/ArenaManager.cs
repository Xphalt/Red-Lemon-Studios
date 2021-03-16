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
    private string checkpointAtStart;

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

        if (autoFind)
        {
            FindArenaItems();
        }
    }

    void Start()
    {
        GetCheckpointAtStart();

        if (!restart) Load();
        if (resetArena || restart)
        {
            checkpointCounter = -1;
            Save();
            resetArena = false;
        }
        else checkpointCounter = 0;
    }

    private void FindArenaItems()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        arenaEnemies.Clear();
        arenaPickUps.Clear();
        foreach (Enemy newEnemy in Resources.FindObjectsOfTypeAll<Enemy>())
            if (newEnemy.gameObject.scene == thisScene) arenaEnemies.Add(newEnemy);

        foreach (PickUpBase newPickup in Resources.FindObjectsOfTypeAll<PickUpBase>())
            if (newPickup.gameObject.scene == thisScene) arenaPickUps.Add(newPickup);
    }

    private void GetCheckpointAtStart()
    {
        if (SaveManager.HasStringList("PreviousCheckpoints"))
        {
            previousCheckpoints = SaveManager.GetStringList("PreviousCheckpoints");
            for (int checkpointIndex = previousCheckpoints.Count - 1; checkpointIndex >= 0; checkpointIndex--)
            {
                checkpointAtStart = previousCheckpoints[checkpointIndex];
                if (!checkpointAtStart.Contains(ArenaName)) break;
                else checkpointAtStart = "";
            }
        }
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

        if (arenaPlayer.killed) Load();
    }

    public void Save()
    {
        checkpointID = ArenaName + checkpointCounter.ToString();
        previousCheckpoints.Add(checkpointID);

        SaveManager.UpdateSavedInt(checkpointID + "CheckpointCounter", ++checkpointCounter);
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

        SaveManager.SaveToFile();
    }

    public void Load(string checkpointOverride="")
    {
        if (!SaveManager.HasString("LastOverallCheckpointID")) return;

        string loadID = (checkpointOverride == "") ? SaveManager.GetString("LastOverallCheckpointID") : checkpointOverride;
        string arenaLoadID = "";

        if (!resetArena && SaveManager.HasString(ArenaName + "LastCheckpointID"))
        {
            arenaLoadID = (loadID.Contains(ArenaName)) ? loadID : SaveManager.GetString(ArenaName + "LastCheckpointID");
            checkpointCounter = SaveManager.GetInt(arenaLoadID + "CheckpointCounter");

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

        string playerLoadID = (resetArena) ? checkpointAtStart : loadID;

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