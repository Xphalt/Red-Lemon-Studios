using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkyboxRotation : MonoBehaviour
{
    public float cameraSpeed, colourDuration;
    public Material skyboxMateriel;
    public List<Color> ColourList = new List<Color>();

    private float colourChangeTimer = 0;
    private int index;

    private void Start()
    {
        for (int i = 0; i < ColourList.Count; i++)
        {
            ColourList.Add(ColourList[i]);
        }
    }

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * cameraSpeed);
        //Link to code: https://answers.unity.com/questions/651780/rotate-skybox-constantly.html

        ChangeColour();
    }

    public void ChangeColour()
    {
        colourChangeTimer += Time.deltaTime;

        float lerp = Mathf.PingPong(Time.time, colourDuration) / colourDuration;
        RenderSettings.skybox.SetColor("_Tint", Color.Lerp(ColourList[index], ColourList[(index + 1) % ColourList.Count], lerp));

        if (colourChangeTimer >= colourDuration)
        {
            ++index;
            //Ensures index doesn't go above list length.
            index %= ColourList.Count;
            colourChangeTimer = 0;
        }

    }
}
