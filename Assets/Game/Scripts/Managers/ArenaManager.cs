using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;
using static SaveManager;
public class ArenaManager : MonoBehaviour
{
    public string ArenaName;
    private bool checkpointed = false;

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
            arenaRelics[r].SaveRelic(r.ToString() + ArenaName);
        }

        for (int p = 0; p < arenaPickUps.Count; p++)
        {
            arenaPickUps[p].SavePickUp(p.ToString() + ArenaName);
        }

        arenaPlayer.SaveStats();

        checkpointed = true;
    }

    public void Load()
    {
        checkpointed = SaveManager.GetBool(ArenaName + "Checkpointed");

        for (int e = 0; e < arenaEnemies.Count; e++)
        {
            arenaEnemies[e].LoadEnemy(e.ToString() + ArenaName);
        }

        for (int r = 0; r < arenaRelics.Count; r++)
        {
            arenaRelics[r].LoadRelic(r.ToString() + ArenaName);
        }

        for (int p = 0; p < arenaPickUps.Count; p++)
        {
            arenaPickUps[p].LoadPickUp(p.ToString() + ArenaName);
        }

        arenaPlayer.LoadStats();
    }

    private void OnDestroy()
    {
        Save();
        SaveManager.SaveToFile();
    }
}
