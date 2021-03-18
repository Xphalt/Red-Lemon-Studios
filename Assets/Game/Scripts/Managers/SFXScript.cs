using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXScript : MonoBehaviour
{
    public List<string> SFXNameList = new List<string>();
    public List<AudioClip> AudioClipList = new List<AudioClip>();

    private Dictionary<string, AudioClip> SFXDict = new Dictionary<string, AudioClip>();
    public AudioSource SFXPrefab;

    private AudioSource Music;

    // Start is called before the first frame update
    void Awake()
    {
        for (int s_index = 0; s_index < SFXNameList.Count; s_index++)
        {
            SFXDict.Add(SFXNameList[s_index], AudioClipList[s_index]);
        }

        Music = GetComponent<AudioSource>();
    }

    public void PlaySFX(string sfx, bool loop = false)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            AudioSource newSFX = Instantiate(SFXPrefab).GetComponent<AudioSource>();
            newSFX.PlayOneShot(SFXDict[sfx]);
            newSFX.loop = loop;
            newSFX.tag = "SFX";
            if (!loop) Destroy(newSFX.gameObject, SFXDict[sfx].length);
        }
    }

    public void PlaySFX(string sfx, string instanceName, bool loop = false)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            AudioSource newSFX = Instantiate(SFXPrefab).GetComponent<AudioSource>();
            newSFX.PlayOneShot(SFXDict[sfx]);
            newSFX.name = instanceName;
            newSFX.loop = loop;
            newSFX.tag = "SFX";
            if (!loop) Destroy(newSFX.gameObject, SFXDict[sfx].length);
        }
    }

    public void PlaySFX(string sfx, float playTime, bool loop = false)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            AudioSource newSFX = Instantiate(SFXPrefab).GetComponent<AudioSource>();
            newSFX.PlayOneShot(SFXDict[sfx]);
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
