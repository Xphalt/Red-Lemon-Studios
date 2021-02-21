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
        elementType = ElementTypes.Earth;
        weakAgainst = ElementTypes.Air;
        strongAgainst = ElementTypes.Water;
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public override void TriggerStatusEffect(ElementAmmoAilments effectStats)
    {
        base.TriggerStatusEffect(effectStats);

        Shift((effectStats.gameObject.transform.position - transform.position) * effectStats.statusMagnitude, effectStats.statusEffectDuration, true);
    }
}
