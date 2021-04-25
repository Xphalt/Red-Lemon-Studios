using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : PickUpBase
{
    public float RestoreValue;

    public override void Update()
    {
        base.Update();
        
        foreach (Collider other in Physics.OverlapSphere(transform.position, detectionRadius))
        {
            if (other.gameObject.TryGetComponent(out Player player))
            {
                if (player.AddHealth(RestoreValue)) Collect();
                break;
            }
        }
    }
}