using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    private float volumeFade;
    private float maxVolume;

    public Animator transition;
    public float delay;
    public float inputLockDelay;
    public Player player;
    public bool fading = false;

    private void Awake()
    {
        StartCoroutine(StartLevel());
    }

    private void Update()
    {
        if (fading)
        {
            AudioListener.volume = Mathf.Clamp(AudioListener.volume + volumeFade * Time.unscaledDeltaTime, 0, maxVolume);
            if (AudioListener.volume <= 0 || AudioListener.volume >= maxVolume) fading = false;
        }
    }

    public IEnumerator LoadLevel(string level)
    {
        ToggleInput(true);
        transition.SetTrigger("exitScene");

        fading = true;
        maxVolume = AudioListener.volume;
        volumeFade = -AudioListener.volume / delay;

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
