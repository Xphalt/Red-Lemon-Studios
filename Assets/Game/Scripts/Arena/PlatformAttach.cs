using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAttach : MonoBehaviour
{
    public Transform parentTransform;
    private Dictionary<Transform, Transform> objectsPreviousParents = new Dictionary<Transform, Transform>();
    public bool playerOnly = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (!objectsPreviousParents.ContainsKey(collision.transform) && (collision.transform.CompareTag("Player") || !playerOnly))
        {
            objectsPreviousParents.Add(collision.transform, collision.transform.parent);
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (objectsPreviousParents.ContainsKey(collision.transform) && (collision.transform.CompareTag("Player") || !playerOnly))
        {
            collision.transform.SetParent(objectsPreviousParents[collision.transform]);
            objectsPreviousParents.Remove(collision.transform);
        }
    }
}
