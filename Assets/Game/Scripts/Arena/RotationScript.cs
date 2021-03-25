using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    public Vector3 RotateSpeed = Vector3.up;
    public float blowRadius = 5;
    public float blowRange = 50;
    public float blowSpeed = 0;

    void Update()
    {
        transform.Rotate(RotateSpeed);

        if (blowSpeed != 0)
        {
            foreach (RaycastHit hit in Physics.CapsuleCastAll(transform.position, transform.position + Vector3.up * blowRange,
                Mathf.Min(blowRadius, blowRange), Vector3.up, blowRange))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    hit.transform.GetComponent<Rigidbody>().velocity += Vector3.up * blowSpeed * Time.deltaTime;
                    break;
                }
            }
        }
    }
}
