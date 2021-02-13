using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementShooting : MonoBehaviour
{
    public GameObject BulletFire;
    public GameObject BulletWater;
    public GameObject BulletEarth;
    public GameObject BulletAir;
    private GameObject ChosenBullet;
    
    public GameObject GunPos;

    public float ShootForce;
    public float TargetDistance;

    public void Shoot()
    {
        CheckElement();
        GameObject newBullet = Instantiate(ChosenBullet, GunPos.transform);
        newBullet.GetComponent<Rigidbody>().AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * TargetDistance) - GunPos.transform.position).normalized * ShootForce);
        newBullet.transform.SetParent(null);
        Destroy(newBullet, 2);
    }

    void CheckElement()
    {
        switch (GetComponent<ElementsScript_MattNDaniel>().m_CurElement)
        {
            case ElementsScript_MattNDaniel.Elements.Fire:
                ChosenBullet = BulletFire;
                break;
            case ElementsScript_MattNDaniel.Elements.Water:
                ChosenBullet = BulletWater;
                break;
            case ElementsScript_MattNDaniel.Elements.Air:
                ChosenBullet = BulletAir;
                break;
            case ElementsScript_MattNDaniel.Elements.Earth:
                ChosenBullet = BulletEarth;
                break;
        }
    }
}
