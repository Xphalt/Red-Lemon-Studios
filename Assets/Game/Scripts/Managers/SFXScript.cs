using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXScript : MonoBehaviour
{
    private AudioSource Music;
    private Dictionary<string, AudioClip> SFXDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, float> volumeDict = new Dictionary<string, float>();
    private int index2D = 0, index3D = 0;

    public List<AudioClip> AudioClipList = new List<AudioClip>();
    public List<string> SFXNameList = new List<string>();
    public List<float> SFXVolumes = new List<float>();
    public List<AudioSource> sfx2D, sfx3D;
    public bool AutoFind = false;

    // Start is called before the first frame update
    void Awake()
    {
        Music = GetComponent<AudioSource>();
        if (AutoFind)
        {
            foreach (AudioSource source in GetComponentsInChildren<AudioSource>())
            {
                if (source != Music)
                {
                    if (source.spatialBlend == 0) sfx2D.Add(source);
                    else sfx3D.Add(source);
                }
            }
        }

        for (int s_index = 0; s_index < SFXNameList.Count; s_index++)
        {
            SFXDict.Add(SFXNameList[s_index], AudioClipList[s_index]);
        }
        for (int s_index = 0; s_index < SFXNameList.Count; s_index++)
        {
            volumeDict.Add(SFXNameList[s_index], SFXVolumes[s_index]);
        }
    }

    void Incrememnt2D()
    {
        ++index2D;
        index2D %= sfx2D.Count;
    }

    void Incrememnt3D()
    {
        ++index3D;
        index3D %= sfx3D.Count; 
    }

    public void PlaySFX2D(string sfx, bool loop = false, float volume = 0)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            if (volume == 0) volume = volumeDict[sfx];
            sfx2D[index2D].PlayOneShot(SFXDict[sfx], volume);
            sfx2D[index2D].loop = loop;
            sfx2D[index2D].tag = "SFX";
            Incrememnt2D();
        }
    }

    public void PlaySFX2D(string sfx, string instanceName, bool loop = false, float volume = 0)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            if (volume == 0) volume = volumeDict[sfx];
            sfx2D[index2D].PlayOneShot(SFXDict[sfx], volume);
            sfx2D[index2D].name = instanceName;
            sfx2D[index2D].loop = loop;
            sfx2D[index2D].tag = "SFX";
            Incrememnt2D();
        }
    }

    public void PlaySFX2D(string sfx, float playTime, bool loop = false, float volume = 0)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            if (volume == 0) volume = volumeDict[sfx];
            sfx2D[index2D].PlayOneShot(SFXDict[sfx], volume);
            sfx2D[index2D].loop = loop;
            sfx2D[index2D].tag = "SFX";
            StopAfter(sfx2D[index2D].name, playTime);
            Incrememnt2D();
        }
    }

    public void PlaySFX3D(string sfx, Vector3 position, bool loop = false, float volume = 0)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            if (volume == 0) volume = volumeDict[sfx];
            sfx3D[index3D].transform.position = position;
            sfx3D[index3D].PlayOneShot(SFXDict[sfx], volume);
            sfx3D[index3D].loop = loop;
            sfx3D[index3D].tag = "SFX";
            Incrememnt3D();
        }
    }

    public void PlaySFX3D(string sfx, Vector3 position, string instanceName, bool loop = false, float volume = 0)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            if (volume == 0) volume = volumeDict[sfx];
            sfx3D[index3D].transform.position = position;
            sfx3D[index3D].PlayOneShot(SFXDict[sfx], volume);
            sfx3D[index3D].name = instanceName;
            sfx3D[index3D].loop = loop;
            sfx3D[index3D].tag = "SFX";
            Incrememnt3D();
        }
    }

    public void PlaySFX3D(string sfx, Vector3 position, float playTime, bool loop = false, float volume = 0)
    {
        if (SFXDict.ContainsKey(sfx))
        {
            if (volume == 0) volume = volumeDict[sfx];
            sfx3D[index3D].transform.position = position;
            sfx3D[index3D].PlayOneShot(SFXDict[sfx], volume);
            sfx3D[index3D].loop = loop;
            sfx3D[index3D].tag = "SFX";
            StopAfter(sfx3D[index3D].name, playTime);
            Incrememnt3D();
        }
    }

    public void PauseSFX()
    {
        foreach (AudioSource source in sfx2D) source.Pause();
        foreach (AudioSource source in sfx3D) source.Pause();
    }

    public void UnPauseSFX()
    {
        foreach (AudioSource source in sfx2D) source.UnPause();
        foreach (AudioSource source in sfx3D) source.UnPause();
    }

    public void StopSFX()
    {
        foreach (AudioSource source in sfx2D) source.Stop();
        foreach (AudioSource source in sfx3D) source.Stop();
    }

    public void StopSFX(string name)
    {
        AudioSource SFXStopped = GameObject.Find(name).GetComponent<AudioSource>();
        if (SFXStopped) SFXStopped.Stop();
    }

    IEnumerator StopAfter(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        StopSFX(name);
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
