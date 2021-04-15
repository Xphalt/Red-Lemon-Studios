//Script created by Zack

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Items : MonoBehaviour
{
    private float DropPercentage; //chance of dropping a pickup

    public int ammoDropPercent = 20;
    public int healDropPercent = 20;

    private bool destroyed = false;

    public List<PickUpBase> ammoDrops;
    public PickUpBase healthDrop;

    public SFXScript sfxScript;
    public string destroySound;

    private void Awake()
    {
        if (sfxScript == null) sfxScript = FindObjectOfType<SFXScript>();
    }

    private void Start()
    {
        if (ammoDrops.Count == 0 || ammoDrops[0] == null)
        {
            ammoDrops.Clear();
            foreach (AmmoPickup ammo in GetComponentsInChildren<AmmoPickup>(true)) ammoDrops.Add(ammo.GetComponent<PickUpBase>());
        }
        if (healthDrop == null) healthDrop = GetComponentInChildren<HealthPickup>(true);

        DropPercentage = Random.Range(0, 100);
        gameObject.SetActive(!destroyed);
    }

    public void OnCollisionEnter(Collision collision)
    { //check if a bullet has collided
        if (collision.gameObject.TryGetComponent(out ElementHazardAilments hitter))
        {
            hitter.RegisterHit();
            destroyed = true;
            Drop(DropPercentage);
            gameObject.SetActive(false);
            sfxScript.PlaySFX3D(destroySound, transform.position);
        }
    }

    public void Drop(float value)
    {
        if (value <= ammoDropPercent)// drop ammo pickup
        {
            int ChosenType = Random.Range(0, ammoDrops.Count);//picks one to drop
            ammoDrops[ChosenType].Spawn();
        }
        else if (value <= ammoDropPercent + healDropPercent) // drop health pickup
        {
            healthDrop.Spawn();
        }
    }

    public void SaveInteractable(string saveID)
    {
        saveID = "Interactable" + saveID;
        SaveManager.UpdateSavedBool(saveID + "Destroyed", destroyed);
    }

    public void LoadInteractable(string loadID)
    {
        loadID = "Interactable" + loadID;
        destroyed = SaveManager.GetBool(loadID + "Destroyed");

        gameObject.SetActive(!destroyed);
    }
}
