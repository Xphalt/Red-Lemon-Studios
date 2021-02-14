/// <summary>
/// Script made by Matt
/// 
/// This allows for health to be
/// picked up from the ground
/// to heal the player
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float RestoreValue;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<Player>().AddHealth(RestoreValue);
            gameObject.SetActive(false);
        }
    }
}