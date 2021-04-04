using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public AudioListener audioListener;
    public GameObject controlsPanel, creditsPanel;
    public string FirstLevel;

    private void Awake()
    {
        SaveManager.LoadFromFile();
        if (audioListener && SaveManager.HasBool("Muted")) audioListener.enabled = SaveManager.GetBool("Muted");
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
        audioListener.enabled = !audioListener.enabled;
        SaveManager.UpdateSavedBool("Muted", audioListener.enabled);
    }

    public void StartGame(bool newGame)
    {
        if (newGame || !(SaveManager.HasString("LastOverallCheckpointID")))
        {
            SaveManager.ClearSaves();
            SaveManager.UpdateSavedBool("Muted", audioListener.enabled);
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

    private void OnDestroy()
    {
        SaveManager.SaveToFile();
    }
}

