using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBase : MonoBehaviour
{
    protected bool collected = false;

    public void Collect()
    {
        collected = true;
        gameObject.SetActive(false);
    }

    public void SavePickUp(string saveID)
    {
        saveID = "Pickup" + saveID;
        SaveManager.UpdateSavedBool(saveID + "Collected", collected);
    }

    public void LoadPickUp(string loadID)
    {
        loadID = "Pickup" + loadID;
        collected = SaveManager.GetBool(loadID + "Collected");

        gameObject.SetActive(!collected);
    }
}
