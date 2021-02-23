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

        if (CanSeePlayer()) movementState = EnemyStates.Chasing;
        else movementState = EnemyStates.Patrolling;
    }

    public override void TriggerStatusEffect(ElementHazardAilments effectStats)
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
