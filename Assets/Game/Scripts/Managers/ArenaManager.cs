using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EnumHelper;
using static SaveManager;
public class ArenaManager : MonoBehaviour
{
    public string ArenaName;
    private bool checkpointed = false;

    public bool restart;
    public bool clearCheckpoint;

    public bool autoFind = true;

    public List<Enemy> arenaEnemies;
    public List<RelicBase> arenaRelics;
    public List<PickUpBase> arenaPickUps;
    public Player arenaPlayer;

    private void Awake()
    {
        SaveManager.LoadFromFile();
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
        
        if (!restart) Load();
    }

    void Update()
    {
        if (!checkpointed)
        {
            bool arenaFinished = true;

            foreach (Enemy arenaenemy in arenaEnemies)
            {
                if (arenaenemy.gameObject.activeInHierarchy)
                {
                    arenaFinished = false;
                    break;
                }
            }

            if (arenaFinished) Save();
        }

        if (arenaPlayer.killed)
        {
            if (checkpointed) Load();
            else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    public void Save()
    {
        SaveManager.UpdateSavedBool(ArenaName + "Checkpointed", true);

        for (int e = 0; e < arenaEnemies.Count; e++)
        {
            arenaEnemies[e].SaveEnemy(e.ToString() + ArenaName);
        }

        for (int r = 0; r < arenaRelics.Count; r++)
        {
            arenaRelics[r].SaveRelic();
        }

        for (int p = 0; p < arenaPickUps.Count; p++)
        {
            arenaPickUps[p].SavePickUp(p.ToString() + ArenaName);
        }

        arenaPlayer.SaveStats(ArenaName);

        checkpointed = true;

        SaveManager.UpdateSavedString("LastSavedLevel", SceneManager.GetActiveScene().name);
        SaveManager.UpdateSavedInt("LastSavedLevel", SceneManager.GetActiveScene().buildIndex);
    }

    public void Load()
    {
        if (SaveManager.HasBool(ArenaName + "Checkpointed") && !clearCheckpoint)
        {
            checkpointed = true;

            for (int e = 0; e < arenaEnemies.Count; e++)
            {
                arenaEnemies[e].LoadEnemy(e.ToString() + ArenaName);
            }

            for (int p = 0; p < arenaPickUps.Count; p++)
            {
                arenaPickUps[p].LoadPickUp(p.ToString() + ArenaName);
            }

        }
        else checkpointed = false;

        for (int r = 0; r < arenaRelics.Count; r++)
        {
            if (!(clearCheckpoint && arenaRelics[r].inArena)) arenaRelics[r].LoadRelic();
        }
        arenaPlayer.LoadStats(checkpointed, ArenaName);
    }

    private void OnDestroy()
    {
        SaveManager.SaveToFile();
    }
}
