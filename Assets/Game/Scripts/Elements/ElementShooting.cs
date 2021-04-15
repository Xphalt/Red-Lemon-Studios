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
    public GameObject wielder;
    private GameObject ChosenBullet;
    public GameObject GunPos;

    public SFXScript sfxScript = null;

    private CharacterBase wielderScript;

    public float damage;

    public List<ElementTypes> BulletTypes = new List<ElementTypes>();
    public List<GameObject> BulletPrefabs = new List<GameObject>();
    private Dictionary<ElementTypes, GameObject> BulletColourDictionary = new Dictionary<ElementTypes, GameObject>();

    public List<string> soundNames = new List<string>();
    private Dictionary<ElementTypes, string> shootSoundsDictionary = new Dictionary<ElementTypes, string>();

    public float ShootSpeed;
    public float range;

    private void Awake()
    {
        wielderScript = wielder.GetComponent<CharacterBase>();
        if (sfxScript == null) sfxScript = FindObjectOfType<SFXScript>();
        for (int index = 0; index < BulletTypes.Count; index++)
        {
            BulletColourDictionary.Add(BulletTypes[index], BulletPrefabs[index]);
        }

        for (int index = 0; index < BulletTypes.Count; index++)
        {
            shootSoundsDictionary.Add(BulletTypes[index], soundNames[index]);
        }
    }

    public void Shoot(ElementTypes shotType, Vector3 target)
    {      
        CheckElement(shotType);
        GameObject newBullet = Instantiate(ChosenBullet, GunPos.transform);
        newBullet.GetComponent<Rigidbody>().velocity = (target - GunPos.transform.position).normalized * ShootSpeed;
        newBullet.transform.SetParent(null);

        ElementHazardAilments newBulletInfo = newBullet.GetComponent<ElementHazardAilments>();
        newBulletInfo.Initialise(damage, wielderScript);
        newBulletInfo.sfxScript = sfxScript;

        if (shootSoundsDictionary.ContainsKey(shotType)) sfxScript.PlaySFX3D(shootSoundsDictionary[shotType], GunPos.transform.position);

        Destroy(newBullet, range/ShootSpeed);
    }

    //Assigns the correct prefab to the selected elemental ammo
    void CheckElement(ElementTypes shotType)
    {
        ChosenBullet = BulletColourDictionary[shotType];
    }
}