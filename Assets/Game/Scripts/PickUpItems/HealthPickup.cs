﻿/// <summary>
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

public class HealthPickup : PickUpBase
{
    public float RestoreValue;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            if (player.AddHealth(RestoreValue)) Collect();
        }
    }
}