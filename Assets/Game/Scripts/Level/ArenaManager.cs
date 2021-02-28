using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;
using static SaveManager;

//Created with some example code to test saving functionality - DANIEL BIBBY

public class ArenaManager : MonoBehaviour
{
    public List<GameObject> enemies;

    private void Awake()
    {
        SaveManager.LoadFromFile();
        Debug.Log(SaveManager.GetVector2("TestData"));
        SaveManager.AddNewData("TestData", new Vector2(6, 9));
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
