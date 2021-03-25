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

    private void Start()
    {
        DropPercentage = Random.Range(0, 100);
        gameObject.SetActive(!destroyed);
    }

    public void OnCollisionEnter(Collision collision)
    { //check if a bullet has collided
        if (collision.gameObject.CompareTag("Bullet"))
        {
            destroyed = true;
            Drop(DropPercentage);
            gameObject.SetActive(false);
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
