using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public Animator transition;
    public float delay;

    public void LoadNextLevel()
    {
        //This loads the next scene by calling the next index of the build scenes
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("exitScene");
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(levelIndex);
    }
}
