using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAttach : MonoBehaviour
{
    private Dictionary<Transform, Transform> objectsPreviousParents = new Dictionary<Transform, Transform>();
    private Dictionary<Transform, Vector3> previousScales = new Dictionary<Transform, Vector3>(); //For some reason scale was not consistent when unparenting

    public Transform parentTransform;
    public bool playerOnly = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (!objectsPreviousParents.ContainsKey(collision.transform) && (collision.transform.CompareTag("Player") || !playerOnly))
        {
            objectsPreviousParents.Add(collision.transform, collision.transform.parent);
            previousScales.Add(collision.transform, collision.transform.localScale);
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (objectsPreviousParents.ContainsKey(collision.transform) && (collision.transform.CompareTag("Player") || !playerOnly))
        {
            collision.transform.SetParent(objectsPreviousParents[collision.transform]);
            collision.transform.localScale = previousScales[collision.transform];
            objectsPreviousParents.Remove(collision.transform);
            previousScales.Remove(collision.transform);
        }
    }
}
