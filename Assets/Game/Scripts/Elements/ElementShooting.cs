/// <summary>
/// 
/// Script made by Daniel
/// 
/// This was initially in the player script
/// but Linden and Daniel de-coupled the 
/// shooting from it to allow for a more
/// modular implementation
/// 
/// When adding new ammo's, this script wont
/// need to be touched due to Linden and
/// Daniel's refactoring
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class ElementShooting : MonoBehaviour
{
    private GameObject player;
    private GameObject ChosenBullet;
    public GameObject GunPos;

    private Elements elementManager;
    private Player playerScript;
    
    public List<GameObject> BulletPrefabs = new List<GameObject>();

    public float ShootForce;
    public float TargetDistance;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        elementManager = GetComponent<Elements>();
        playerScript = player.GetComponent<Player>();
    }

    //TODO cache the rigidbody reference to boost performance
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

    //Assigns the correct prefab to the selected elemental ammo
    void CheckElement()
    {
        ChosenBullet = BulletPrefabs[(int)elementManager.m_CurElement];
    }
}