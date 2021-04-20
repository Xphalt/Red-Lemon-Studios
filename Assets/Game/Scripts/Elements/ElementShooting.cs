using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class ElementShooting : MonoBehaviour
{
    private Dictionary<ElementTypes, string> shootSoundsDictionary = new Dictionary<ElementTypes, string>();
    private Dictionary<ElementTypes, GameObject> BulletColourDictionary = new Dictionary<ElementTypes, GameObject>();
    private CharacterBase wielderScript;
    private GameObject ChosenBullet;

    public GameObject wielder;
    public GameObject GunPos;
    public SFXScript sfxScript = null;
    public List<ElementTypes> BulletTypes = new List<ElementTypes>();
    public List<GameObject> BulletPrefabs = new List<GameObject>();
    public List<string> soundNames = new List<string>();
    public float ShootSpeed;
    public float range;
    public float damage;

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
        newBullet.transform.SetParent(null);
        ElementHazardAilments newBulletInfo = newBullet.GetComponent<ElementHazardAilments>();
        newBulletInfo.Initialise(damage, wielderScript, (target - GunPos.transform.position).normalized * ShootSpeed);
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