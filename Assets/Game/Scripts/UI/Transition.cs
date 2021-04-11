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
    public ArenaManager arenaManager;

    private void Awake()
    {
        StartCoroutine(StartLevel());
    }

    public IEnumerator LoadLevel()
    {
        player.ToggleInput(true);
        transition.SetTrigger("exitScene");
        yield return new WaitForSecondsRealtime(delay);
        arenaManager.TransitionLevel();
    }

    IEnumerator StartLevel()
    {
        player.ToggleInput(true);
        yield return new WaitForSecondsRealtime(inputLockDelay);
        player.ToggleInput(false);
    }
}
