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

    public GameObject SFXManager = null;
    private SFXScript sfxScript;

    private CharacterBase wielderScript;

    public float damage;

    public List<ElementTypes> BulletTypes = new List<ElementTypes>();
    public List<GameObject> BulletPrefabs = new List<GameObject>();
    private Dictionary<ElementTypes, GameObject> BulletColourDictionary = new Dictionary<ElementTypes, GameObject>();

    public List<ElementTypes> soundTypes = new List<ElementTypes>();
    public List<string> soundNames = new List<string>();
    private Dictionary<ElementTypes, string> shootSoundsDictionary = new Dictionary<ElementTypes, string>();

    public float ShootSpeed;

    private void Start()
    {
        wielderScript = wielder.GetComponent<CharacterBase>();

        for (int index = 0; index < BulletTypes.Count; index++)
        {
            BulletColourDictionary.Add(BulletTypes[index], BulletPrefabs[index]);
        }

        for (int index = 0; index < soundTypes.Count; index++)
        {
            shootSoundsDictionary.Add(soundTypes[index], soundNames[index]);
        }

        if (SFXManager == null) SFXManager = GameObject.FindGameObjectWithTag("SFXManager");
        sfxScript = SFXManager.GetComponent<SFXScript>();
    }

    //TODO cache the rigidbody reference to boost performance
    public void Shoot(ElementTypes shotType, Vector3 target)
    {      
        CheckElement(shotType);
        GameObject newBullet = Instantiate(ChosenBullet, GunPos.transform);
        newBullet.GetComponent<Rigidbody>().velocity = (target - GunPos.transform.position).normalized * ShootSpeed;
        newBullet.transform.SetParent(null);

        ElementHazardAilments newBulletInfo = newBullet.GetComponent<ElementHazardAilments>();
        newBulletInfo.Initialise(damage, wielderScript);

        if (shootSoundsDictionary.ContainsKey(shotType)) sfxScript.PlaySFX(shootSoundsDictionary[shotType]);

        Destroy(newBullet, 2);
    }

    //Assigns the correct prefab to the selected elemental ammo
    void CheckElement(ElementTypes shotType)
    {
        ChosenBullet = BulletColourDictionary[shotType];
    }
}