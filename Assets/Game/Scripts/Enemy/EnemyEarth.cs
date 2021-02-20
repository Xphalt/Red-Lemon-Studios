using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyEarth : Enemy
{
    private bool rolling = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        weakAgainst = ElementTypes.Fire;
        strongAgainst = ElementTypes.Air;
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
