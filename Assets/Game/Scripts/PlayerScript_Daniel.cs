/*
 * DANIEL BIBBY
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript_Daniel : MonoBehaviour
{
    public GameObject BulletPrefab;
    public GameObject GunPos;

    public float ShootForce;
    public float TargetDistance;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject newBullet = Instantiate(BulletPrefab, GunPos.transform);
            newBullet.GetComponent<Rigidbody>().AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward*TargetDistance) - GunPos.transform.position).normalized * ShootForce);
            newBullet.transform.SetParent(null);
            Destroy(newBullet, 2);
        }
    }
}
