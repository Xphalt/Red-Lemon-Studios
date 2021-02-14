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

public class AmmoPickup : MonoBehaviour
{
    public int AmmoValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().AddAmmo(AmmoValue);
            gameObject.SetActive(false);
        }
    }
}