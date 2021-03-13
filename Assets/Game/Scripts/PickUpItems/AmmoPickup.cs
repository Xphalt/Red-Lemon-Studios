/// <summary>
/// Script made by Matt
/// 
/// This allows for ammo to be
/// picked up from the ground
/// for the player to use
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class AmmoPickup : PickUpBase
{
    public int AmmoValue;
    public ElementTypes Type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().AddAmmo(AmmoValue, Type);
            Collect();
        }
    }
}