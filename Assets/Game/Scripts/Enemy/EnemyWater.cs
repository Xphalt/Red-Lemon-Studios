using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyWater : Enemy
{
    public override void Start()
    {
        base.Start();
        elementType = ElementTypes.Water;
        weakAgainst = ElementTypes.Earth;
        strongAgainst = ElementTypes.Fire;
    }

    public override void Update()
    {
        base.Update();

        if (CanSeePlayer()) actionState = EnemyStates.Chasing;
        else actionState = EnemyStates.Patrolling;
    }

    public override void TriggerStatusEffect(ElementAmmoAilments effectStats)
    {
        base.TriggerStatusEffect(effectStats);

        speedMultiplier *= statusMagnitude;
    }

    public override void EndSatusEffect()
    {
        speedMultiplier /= statusMagnitude;

        base.EndSatusEffect();
    }
}
