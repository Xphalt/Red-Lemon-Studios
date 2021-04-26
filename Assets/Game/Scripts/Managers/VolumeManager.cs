using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Image soundIcon;
    public Sprite defaultSoundSprite, mutedSoundSprite;

    private float storedVolume;
    private bool muted;

    void Start()
    {
        if (!SaveManager.HasFloat("Volume")) SaveManager.AddNewFloat("Volume", 1);
        if (!SaveManager.HasBool("Muted")) SaveManager.AddNewBool("Muted", false);

        storedVolume = SaveManager.GetFloat("Volume");
        volumeSlider.value = storedVolume;

        muted = SaveManager.GetBool("Muted");
        SetVolume(muted ? 0 : storedVolume);
        SetIcon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) ToggleMute();
    }

    public void ToggleMute()
    {
        muted = !muted;
        SetIcon();
        SetVolume(muted ? 0 : storedVolume);
    }

    public void Unmute()
    {
        if (muted) ToggleMute();
    }

    private void SetIcon()
    {
        if (soundIcon)
        {
            soundIcon.sprite = muted ? mutedSoundSprite : defaultSoundSprite;
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
    public void UpdateStoredVolume(float volume)
    {
        storedVolume = volume;
    }

    public void SaveVolumeSettings()
    {
        SaveManager.UpdateSavedBool("Muted", muted);
        SaveManager.UpdateSavedFloat("Volume", storedVolume);
    }
}
