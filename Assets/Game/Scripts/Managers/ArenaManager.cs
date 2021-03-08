using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;
using static SaveManager;

//Created with some example code to test saving functionality - DANIEL BIBBY

public class ArenaManager : MonoBehaviour
{
    public List<GameObject> arenaEnemies;
    public List<GameObject> arenaRelics;
    public GameObject arenaPlayer;

    private void Awake()
    {
        SaveManager.LoadFromFile();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Save()
    {
        for (int r = 0; r < arenaRelics.Count; r++)
        {
            arenaRelics[r].GetComponent<RelicBase>().SaveRelic(r);
        }
    }

    public void Load()
    {
        for (int r = 0; r < arenaRelics.Count; r++)
        {
            RelicBase relicScript = arenaRelics[r].GetComponent<RelicBase>();
            relicScript.LoadRelic(r);
            if (relicScript.collected) relicScript.SetUser(arenaPlayer); //if other characters use relics, an identification method will be needed.
        }
    }

    private void OnDestroy()
    {
        Save();
        SaveManager.SaveToFile();
    }
}
