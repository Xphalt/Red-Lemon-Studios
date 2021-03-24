using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public GameObject StartGO, ExitGO;
    public string FirstLevel;

    private void Awake()
    {
        SaveManager.LoadFromFile();
    }

    public void StartGame(bool newGame)
    {
        if (newGame || !(SaveManager.HasString("LastOverallCheckpointID")))
        {
            SaveManager.ClearSaves();
            SceneManager.LoadScene(FirstLevel);
        }
        else
            SceneManager.LoadScene(SaveManager.GetInt("LastSavedScene"));
    }

    public void ExitScene()
    {
        ExitGO.SetActive(true);
        print("exited scene");
        Application.Quit();
    }
}

