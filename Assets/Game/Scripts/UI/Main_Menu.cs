using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{

    public List<string> SceneName = new List<string>();
    public List<GameObject> ButtonGOs = new List<GameObject>();
    public GameObject ExitGO;

    public void SceneSelect(int SceneID)
    {
        ButtonGOs[SceneID].SetActive(true);
        SceneManager.LoadScene(SceneName[SceneID]);
    }

    public void ExitScene()
    {
        ExitGO.SetActive(true);
        print("exited scene");
        Application.Quit();
    }
}

