using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public GameObject controlsPanel, creditsPanel, exitButton;
    public string FirstLevel;

    public Transition transition;
    public float transitionDelay;

    private VolumeManager volume;

    private void Awake()
    {
        SaveManager.LoadFromFile();
        volume = GetComponent<VolumeManager>();

#if UNITY_WEBGL
        exitButton.SetActive(false);
#endif
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
            StartCoroutine(transition.LoadLevel(FirstLevel));
        }
        else
            StartCoroutine(transition.LoadLevel(SaveManager.GetString("LastSavedScene")));
    }

    public void ExitScene()
    {
#if (UNITY_EDITOR)
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

    private void OnDestroy()
    {
        volume.SaveVolumeSettings();
        SaveManager.SaveToFile();
    }
}

