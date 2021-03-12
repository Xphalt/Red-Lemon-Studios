using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBase : MonoBehaviour
{
    protected bool collected = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SavePickUp(string identifier)
    {
        identifier = "Pickup" + identifier;
        SaveManager.AddNewBool(identifier + collected, collected);
    }

    public void LoadPickUp(string identifier)
    {
        identifier = "Pickup" + identifier;
        collected = SaveManager.GetBool(identifier + collected);

        gameObject.SetActive(collected);
    }
}
