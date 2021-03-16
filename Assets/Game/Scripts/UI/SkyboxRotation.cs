using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotation : MonoBehaviour
{
    public float CameraSpeed;
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * CameraSpeed);
        //Link to code: https://answers.unity.com/questions/651780/rotate-skybox-constantly.html
    }
}
