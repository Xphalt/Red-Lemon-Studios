﻿/// <summary>
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
    public GameObject wielder;
    private GameObject ChosenBullet;
    public GameObject GunPos;

    private CharacterBase wielderScript;

    public float damage;

    public List<ElementTypes> BulletTypes = new List<ElementTypes>();
    public List<GameObject> BulletPrefabs = new List<GameObject>();
    private Dictionary<ElementTypes, GameObject> BulletColourDictionary = new Dictionary<ElementTypes, GameObject>();

    public float ShootForce;

    private void Start()
    {
        wielderScript = wielder.GetComponent<CharacterBase>();

        for (int index = 0; index < BulletTypes.Count; index++)
        {
            BulletColourDictionary.Add(BulletTypes[index], BulletPrefabs[index]);
        }
    }

    //TODO cache the rigidbody reference to boost performance
    public void Shoot(ElementTypes shotType, Vector3 target)
    {
        CheckElement(shotType);
        GameObject newBullet = Instantiate(ChosenBullet, GunPos.transform);
        newBullet.GetComponent<Rigidbody>().AddForce((target - GunPos.transform.position).normalized * ShootForce);
        newBullet.transform.SetParent(null);

        ElementAmmoAilments newBulletInfo = newBullet.GetComponent<ElementAmmoAilments>();
        newBulletInfo.user = wielderScript;
        newBulletInfo.Initialise(damage); 

        Destroy(newBullet, 2);
    }

    //Assigns the correct prefab to the selected elemental ammo
    void CheckElement(ElementTypes shotType)
    {
        ChosenBullet = BulletColourDictionary[shotType];
    }
}