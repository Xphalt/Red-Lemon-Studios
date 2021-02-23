using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyWater : Enemy
{
    public float meleeDamage;
    public float meleeRange;

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
        else if (!sentryMode) movementState = EnemyStates.Patrolling;

        if (TargetInFront())
        {
            Attack();
            movementState = EnemyStates.Idle;
        }
    }

    public bool TargetInFront()
    {
        if (!CanSeePlayer()) return false;

        foreach (RaycastHit targetScan in Physics.RaycastAll(transform.position, transform.forward, meleeRange))
        {
            if (targetScan.transform.gameObject == target) return true;
        }

        return false;
    }

    public override bool Attack()
    {
        if (base.Attack())
        {
            playerScript.TakeDamage(meleeDamage);
            return true;
        }
        return false;
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
