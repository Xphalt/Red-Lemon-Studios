using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class AmmoPickup : PickUpBase
{
    public int AmmoValue;
    public ElementTypes Type;

    public override void Update()
    {
        base.Update();

        foreach (Collider other in Physics.OverlapSphere(transform.position, detectionRadius))
        {
            if (other.gameObject.TryGetComponent(out Player player))
            {
                if (player.AddAmmo(AmmoValue, Type)) Collect();
                break;
            }
        }
    }
}