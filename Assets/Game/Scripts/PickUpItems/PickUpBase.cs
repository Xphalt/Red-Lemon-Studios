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

    public void SavePickUp(string identifier)
    {
        identifier = "Pickup" + identifier;
        SaveManager.AddNewBool(identifier + "Collected", collected);
    }

    public void LoadPickUp(string identifier)
    {
        identifier = "Pickup" + identifier;
        collected = SaveManager.GetBool(identifier + "Collected");

        gameObject.SetActive(!collected);
    }
}
