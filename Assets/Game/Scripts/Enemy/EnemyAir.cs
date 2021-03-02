﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyAir : Enemy
{
    public float weaponRange;
    public int knockbackSpeed;
    public float knockbackDuration;
    public float postKnockbackMomentum; //Velocity retained by target after being knocked back (0-1)

    public float stationaryZone = 0.05f; //Percentage of weapon range which enemy stays in before running away from target
    private float minWeaponRange; //Point at which enemy runs away^

    public float snipeDamage;

    private bool stunned;

    public override void Start()
    {
        base.Start();

        elementType = ElementTypes.Air;
        weakAgainst = ElementTypes.Fire;
        strongAgainst = ElementTypes.Earth;

        minWeaponRange = weaponRange * (1 - stationaryZone);
    }

    public override void Update()
    {
        base.Update();

        float targetDistance = GetDistance();
        bool inAttackRange = targetDistance < weaponRange;

        if (!stunned)
        {
            if (!CanSeePlayer() && !sentryMode) movementState = EnemyStates.Patrolling;
            //!= is equivalent of XOR
            else if (targetDistance > weaponRange != targetDistance < minWeaponRange)
            {
                if (inAttackRange) movementState = EnemyStates.Fleeing;
                else movementState = EnemyStates.Chasing;
            }

            else movementState = EnemyStates.Idle;

            if (inAttackRange)
            {
                Attack();
            }
        }

        else
        {
            movementState = EnemyStates.Idle;

            DOTTimer += Time.deltaTime;
            if (DOTTimer > DOTInterval)
            {
                TakeDamage(statusMagnitude);
                DOTTimer = 0;
            }
        }
    }

    public override bool Attack()
    {
        if (base.Attack())
        {
            playerScript.TakeDamage(snipeDamage);

            if (!playerScript.movementLocked)
            {
                playerScript.Shift(((target.transform.position - transform.position).normalized * knockbackSpeed), knockbackDuration, postKnockbackMomentum, 1, true);
            }
            return true;
        }
        return false;
    }

    public override void TriggerStatusEffect(ElementHazardAilments effectStats)
    {
        base.TriggerStatusEffect(effectStats);
        stunned = true;
    }

    public override void EndSatusEffect()
    {
        base.EndSatusEffect();
        stunned = false;
        DOTTimer = 0;
    }
}
