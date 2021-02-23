using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyEarth : Enemy
{
    private bool rolling = false;
    public float knockbackRecovery; //How quickly AI returns to normal velocity after being knocked back (0-1)

    public override void Start()
    {
        base.Start();
        elementType = ElementTypes.Earth;
        weakAgainst = ElementTypes.Air;
        strongAgainst = ElementTypes.Water;
    }

    public override void Update()
    {
        
    }

    public override void TriggerStatusEffect(ElementHazardAilments effectStats)
    {
        base.TriggerStatusEffect(effectStats);

        Shift((effectStats.gameObject.transform.position - transform.position) * effectStats.statusMagnitude, effectStats.statusEffectDuration, (1-knockbackRecovery), true);
    }
}
