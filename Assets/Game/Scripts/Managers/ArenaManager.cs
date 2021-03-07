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

    }

    public void Load()
    {

    }

    private void OnDestroy()
    {
        SaveManager.SaveToFile();
    }
}
