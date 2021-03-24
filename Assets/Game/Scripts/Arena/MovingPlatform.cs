using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 maxMovement;
    public float moveSpeed;
    private Vector3 startPos;

    private Vector3 moveVelocity;

    private Dictionary<Transform, Transform> objectsPreviousParents = new Dictionary<Transform, Transform>();

    void Start()
    {
        moveVelocity = maxMovement.normalized * moveSpeed;
        startPos = transform.position;
    }

    private void Update()
    {
        transform.Translate(moveVelocity * Time.deltaTime); //Apologise for the translating but in this case is' easier to not have a rigidbody
        if ((transform.position - startPos).magnitude > maxMovement.magnitude) moveVelocity *= -1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!objectsPreviousParents.ContainsKey(collision.transform) && collision.transform.CompareTag("Player"))
        {
            objectsPreviousParents.Add(collision.transform, collision.transform.parent);
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (objectsPreviousParents.ContainsKey(collision.transform) && collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(objectsPreviousParents[collision.transform]);
            objectsPreviousParents.Remove(collision.transform);
        }
    }
}
