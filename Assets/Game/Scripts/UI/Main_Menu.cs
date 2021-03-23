using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public GameObject StartGO, ExitGO, controlsPanel, creditsPanel;
    public string FirstLevel;

    private void Awake()
    {
        SaveManager.LoadFromFile();
    }

    private void Start()
    {
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
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

    public void Controls(bool setActive)
    {
        if (setActive)
            controlsPanel.SetActive(true);
        else
            controlsPanel.SetActive(false);
    }

    public void Credits(bool setActive)
    {
        if (setActive)
            creditsPanel.SetActive(true);
        else
            creditsPanel.SetActive(false);
    }

}

