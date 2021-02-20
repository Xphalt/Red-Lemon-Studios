using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyWater : Enemy
{



    public override void Start()
    {
        base.Start();
        weakAgainst = ElementTypes.Air;
        strongAgainst = ElementTypes.Fire;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
