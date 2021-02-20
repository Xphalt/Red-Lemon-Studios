using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyFire : Enemy
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        weakAgainst = ElementTypes.Water;
        strongAgainst = ElementTypes.Earth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
