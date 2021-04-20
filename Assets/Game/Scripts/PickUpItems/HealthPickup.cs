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