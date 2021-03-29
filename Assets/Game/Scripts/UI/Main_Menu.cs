using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public GameObject controlsPanel, creditsPanel;
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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Controls(bool setActive)
    {
        controlsPanel.SetActive(setActive);
    }

    public void Credits(bool setActive)
    {
        creditsPanel.SetActive(setActive);
    }

}

