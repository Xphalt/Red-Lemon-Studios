using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;


public class ElementShooting : MonoBehaviour
{
    private ElementsScript_MattNDaniel elementManager;

    public GameObject player;
    private PlayerScript_Daniel playerScript;
    
    private GameObject ChosenBullet;
    
    public GameObject GunPos;

    public List<GameObject> BulletPrefabs = new List<GameObject>();

    public float ShootForce;
    public float TargetDistance;

    private void Start()
    {
        elementManager = GetComponent<ElementsScript_MattNDaniel>();
        playerScript = player.GetComponent<PlayerScript_Daniel>();
    }

    public void Shoot()
    {
        if (playerScript.AmmoCheck())
        {
            CheckElement();
            GameObject newBullet = Instantiate(ChosenBullet, GunPos.transform);
            newBullet.GetComponent<Rigidbody>().AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * TargetDistance) - GunPos.transform.position).normalized * ShootForce);
            newBullet.transform.SetParent(null);
            Destroy(newBullet, 2);

            playerScript.SubstractAmmo(1);
        }    
       
    }

    void CheckElement()
    {
        ChosenBullet = BulletPrefabs[(int)elementManager.m_CurElement];
    }
}
