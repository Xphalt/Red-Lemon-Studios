using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingFan : MonoBehaviour
{
    public Vector3 RotateSpeed = Vector3.up * 360;
    public float blowRadius = 5;
    public float blowRange = 50;
    public float blowSpeed = 0;

    void Update()
    {
        transform.Rotate(RotateSpeed * Time.deltaTime);

        if (blowSpeed != 0)
        {
            foreach (Collider hit in Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * blowRange,
                Mathf.Min(blowRadius, blowRange)))
            {
                if (hit.transform.TryGetComponent(out Player player))
                {
                    if (!player.movementLocked) player.GetComponent<Rigidbody>().velocity += Vector3.up * blowSpeed * Time.deltaTime;
                    break;
                }
            }
        }
    }
}
