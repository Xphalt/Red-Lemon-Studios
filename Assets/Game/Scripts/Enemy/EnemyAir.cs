using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyAir : Enemy
{
    public float weaponRange;
    public int knockbackForce;
    public float knockBackDuration;

    public float stationaryZone;
    private float minWeaponRange;

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
            if (!CanSeePlayer()) actionState = EnemyStates.Patrolling;
            //!= is equivalent of XOR
            else if (targetDistance > weaponRange != targetDistance < minWeaponRange)
            {
                if (inAttackRange) actionState = EnemyStates.Fleeing;
                else actionState = EnemyStates.Chasing;
            }

            else actionState = EnemyStates.Idle;
        }

        else
        {
            actionState = EnemyStates.Idle;

            DOTTimer += Time.deltaTime;
            if (DOTTimer > DOTInterval)
            {
                TakeDamage(statusMagnitude);
                DOTTimer = 0;
            }
        }


        if (inAttackRange)
        {
            Attack();
        }
    }

    public override bool Attack()
    {
        if (base.Attack())
        {
            playerScript.TakeDamage(snipeDamage);

            if (!playerScript.movementLocked)
            {
                playerScript.Shift(((target.transform.position - transform.position).normalized * knockbackForce), knockBackDuration, true, true);
            }
        }

        return true;
    }

    public override void TriggerStatusEffect(ElementAmmoAilments effectStats)
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
