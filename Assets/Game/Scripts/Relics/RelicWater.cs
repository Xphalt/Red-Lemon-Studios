using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class RelicWater : RelicBase
{
    public float healAmount;
    public int ammoCost;

    void Start()
    {
        relicType = ElementTypes.Water;
    }

    public override bool Activate()
    {
        if (!base.Activate()) return false;

        characterScript.AddHealth(healAmount, ammoCost, relicType);
        readyToUse = false;

        return true;
    }
}
