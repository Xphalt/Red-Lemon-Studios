﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBase : MonoBehaviour
{
    protected bool collected = false;
    public bool enemyDrop;
    public bool spawned;

    private void Start()
    {
        gameObject.SetActive(spawned);
    }

    public void Spawn()
    {
        spawned = true;
        gameObject.SetActive(true);
        if (enemyDrop)
        {
            transform.SetParent(null);
            transform.localScale = Vector3.one;
        }
    }

    public void Collect()
    {
        collected = true;
        gameObject.SetActive(false);
    }

    public void SavePickUp(string saveID)
    {
        saveID = "Pickup" + saveID;
        SaveManager.UpdateSavedVector3(saveID + "Position", transform.position);
        SaveManager.UpdateSavedBool(saveID + "Collected", collected);
        SaveManager.UpdateSavedBool(saveID + "Spawned", spawned);
    }

    public void LoadPickUp(string loadID)
    {
        loadID = "Pickup" + loadID;
        transform.position = SaveManager.GetVector3(loadID + "Position");
        collected = SaveManager.GetBool(loadID + "Collected");
        spawned = SaveManager.GetBool(loadID + "Spawned");

        if (spawned) Spawn();

        gameObject.SetActive(spawned && !collected);
    }
}
