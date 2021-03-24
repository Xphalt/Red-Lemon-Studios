using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXScript : MonoBehaviour
{
    public List<string> SFXNameList = new List<string>();
    public List<AudioClip> AudioClipList = new List<AudioClip>();
    public List<float> SFXVolumes = new List<float>();

    private Dictionary<string, AudioClip> SFXDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, float> volumeDict = new Dictionary<string, float>();
    public AudioSource SFXPrefab2D;
    public AudioSource SFXPrefab3D;

    private AudioSource Music;

    // Start is called before the first frame update
    void Awake()
    {
        for (int s_index = 0; s_index < SFXNameList.Count; s_index++)
        {
            SFXDict.Add(SFXNameList[s_index], AudioClipList[s_index]);
        }
        for (int s_index = 0; s_index < SFXNameList.Count; s_index++)
        {
            volumeDict.Add(SFXNameList[s_index], SFXVolumes[s_index]);
        }
        Music = GetComponent<AudioSource>();
    }

    public void PlaySFX2D(string sfx, bool loop = false, float volume = 0)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            if (volume == 0) volume = volumeDict[sfx];
            AudioSource newSFX = Instantiate(SFXPrefab2D).GetComponent<AudioSource>();
            newSFX.PlayOneShot(SFXDict[sfx], volume);
            newSFX.loop = loop;
            newSFX.tag = "SFX";
            if (!loop) Destroy(newSFX.gameObject, SFXDict[sfx].length);
        }
    }

    public void PlaySFX2D(string sfx, string instanceName, bool loop = false, float volume = 0)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            if (volume == 0) volume = volumeDict[sfx];
            AudioSource newSFX = Instantiate(SFXPrefab2D).GetComponent<AudioSource>();
            newSFX.PlayOneShot(SFXDict[sfx], volume);
            newSFX.name = instanceName;
            newSFX.loop = loop;
            newSFX.tag = "SFX";
            if (!loop) Destroy(newSFX.gameObject, SFXDict[sfx].length);
        }
    }

    public void PlaySFX2D(string sfx, float playTime, bool loop = false, float volume = 0)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            if (volume == 0) volume = volumeDict[sfx];
            AudioSource newSFX = Instantiate(SFXPrefab2D).GetComponent<AudioSource>();
            newSFX.PlayOneShot(SFXDict[sfx], volume);
            newSFX.loop = loop;
            newSFX.tag = "SFX";
            if (!loop) Destroy(newSFX.gameObject, playTime);
        }
    }

    public void PlaySFX3D(string sfx, Vector3 position, bool loop = false, float volume = 0)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            if (volume == 0) volume = volumeDict[sfx];
            AudioSource newSFX = Instantiate(SFXPrefab3D).GetComponent<AudioSource>();
            newSFX.transform.position = position;
            newSFX.PlayOneShot(SFXDict[sfx], volume);
            newSFX.loop = loop;
            newSFX.tag = "SFX";
            if (!loop) Destroy(newSFX.gameObject, SFXDict[sfx].length);
        }
    }

    public void PlaySFX3D(string sfx, Vector3 position, string instanceName, bool loop = false, float volume = 0)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            if (volume == 0) volume = volumeDict[sfx];
            AudioSource newSFX = Instantiate(SFXPrefab3D).GetComponent<AudioSource>();
            newSFX.transform.position = position;
            newSFX.PlayOneShot(SFXDict[sfx], volume);
            newSFX.name = instanceName;
            newSFX.loop = loop;
            newSFX.tag = "SFX";
            if (!loop) Destroy(newSFX.gameObject, SFXDict[sfx].length);
        }
    }

    public void PlaySFX3D(string sfx, Vector3 position, float playTime, bool loop = false, float volume = 0)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            if (volume == 0) volume = volumeDict[sfx];
            AudioSource newSFX = Instantiate(SFXPrefab3D).GetComponent<AudioSource>();
            newSFX.transform.position = position;
            newSFX.PlayOneShot(SFXDict[sfx], volume);
            newSFX.loop = loop;
            newSFX.tag = "SFX";
            if (!loop) Destroy(newSFX.gameObject, playTime);
        }
    }

    public void StopSFX(string name)
    {
        GameObject SFXStopped = GameObject.Find(name);
        if (SFXStopped) Destroy(SFXStopped);
    }

    public void PlayMusic()
    {
        Music.Play();
    }

    public void StopMusic()
    {
        Music.Stop();
    }
}
