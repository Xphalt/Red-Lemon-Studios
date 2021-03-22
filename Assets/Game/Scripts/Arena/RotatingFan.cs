using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingFan : MonoBehaviour
{
    public float RotateSpeedX;
    public float RotateSpeedY;
    public float RotateSpeedZ;
    void Update()
    {
            transform.Rotate(RotateSpeedX, RotateSpeedY, RotateSpeedZ);
    }
}
