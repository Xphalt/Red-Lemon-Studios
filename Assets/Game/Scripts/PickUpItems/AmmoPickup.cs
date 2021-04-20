using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class AmmoPickup : PickUpBase
{
    public int AmmoValue;
    public ElementTypes Type;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            if (player.AddAmmo(AmmoValue, Type)) Collect();
        }
    }
}