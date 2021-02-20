using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyAir : Enemy
{
    public int range;
    public int knockbackForce;

    public override void Start()
    {
        base.Start();
        weakAgainst = ElementTypes.Earth;
        strongAgainst = ElementTypes.Water;
    }

    void Update()
    {
        
    }
}
