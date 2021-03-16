using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkyboxRotation : MonoBehaviour
{
    public float CameraSpeed, ColourDuration;
    public Material skyboxMateriel;
    public List<Color> ColourList = new List<Color>();


    private void Start()
    {
        for (int i = 0; i < ColourList.Count; i++)
        {
            ColourList.Add(ColourList[i]);
        }
    }

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * CameraSpeed);
        //Link to code: https://answers.unity.com/questions/651780/rotate-skybox-constantly.html


        ChangeColour();

    }

    public void ChangeColour()
    {

        for (int i = 0; i < ColourList.Count; i++)
        {

            float lerp = Mathf.PingPong(Time.time, ColourDuration) / ColourDuration;
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(ColourList[i], ColourList[i + 1], lerp));
        }
    }
}
