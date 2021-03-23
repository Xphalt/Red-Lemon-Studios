using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public GameObject StartGO, ExitGO;
    public string FirstLevel;
    public Canvas worldCanvas, localCanvas;

    private Camera mainCamera;

    private void Awake()
    {
        SaveManager.LoadFromFile();
        mainCamera = FindObjectOfType<Camera>();
        mainCamera = mainCamera.GetComponent<Camera>();
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

    public void Controls()
    {
        worldCanvas.enabled = false;
        localCanvas.enabled = true;
        mainCamera.transform.position = Vector3.zero;
    }
}

