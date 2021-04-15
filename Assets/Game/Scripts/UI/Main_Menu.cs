using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public GameObject controlsPanel, creditsPanel;
    public string FirstLevel;

    public Transition transition;
    public float transitionDelay;

    private void Awake()
    {
        SaveManager.LoadFromFile();
        if (SaveManager.HasBool("Muted")) AudioListener.volume = SaveManager.GetBool("Muted") ? 0 : 1;
    }

    private void Start()
    {
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            Mute();
    }

    public void Mute()
    {
        AudioListener.volume = (AudioListener.volume == 1) ? 0 : 1;
        SaveManager.UpdateSavedBool("Muted", AudioListener.volume == 0);
    }

    public void StartGame(bool newGame)
    {
        if (newGame || !(SaveManager.HasString("LastOverallCheckpointID")))
        {
            SaveManager.ClearSaves();
            SaveManager.UpdateSavedBool("Muted", AudioListener.volume == 0);
            StartCoroutine(transition.LoadLevel(FirstLevel));
        }
        else
            StartCoroutine(transition.LoadLevel(SaveManager.GetString("LastSavedScene")));
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

    private void OnDestroy()
    {
        SaveManager.SaveToFile();
    }
}

