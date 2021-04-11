using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public Animator transition;
    public float delay;
    public float inputLockDelay;
    public Player player;

    private void Awake()
    {
        StartCoroutine(StartLevel());
    }

    public IEnumerator LoadLevel(string level)
    {
        ToggleInput(true);
        transition.SetTrigger("exitScene");
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene(level);
    }

    IEnumerator StartLevel()
    {
        ToggleInput(true);
        yield return new WaitForSecondsRealtime(inputLockDelay);
        ToggleInput(false);
    }

    public void ToggleInput(bool lockInput)
    {
        if (player != null) player.ToggleInput(true, lockInput);
        else Time.timeScale = (lockInput) ? 0 : 1;
    }
}
