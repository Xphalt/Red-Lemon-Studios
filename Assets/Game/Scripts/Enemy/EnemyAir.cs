using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyAir : Enemy
{
    public float weaponRange;

    public float stationaryZone = 0.05f; //Percentage of weapon range which enemy stays in before running away from target
    private float minWeaponRange; //Point at which enemy runs away^

    private bool stunned;

    public string attackSound;

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
            else if ((targetDistance > weaponRange != targetDistance < minWeaponRange) && !runTooFar)
            {
                if (inAttackRange) movementState = EnemyStates.Fleeing;
                else movementState = EnemyStates.Chasing;
            }

            else
            {
                movementState = EnemyStates.Idle;
                transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
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
        Animate();
    }

    public override bool Attack()
    {
        if (base.Attack() && !stunned)
        {
            shooter.Shoot(elementType, target.transform.position);
            sfxScript.PlaySFX3D(attackSound, transform.position);
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

    public override void Animate()
    {
        if (myAnim)
        {
            base.Animate();
            if (movementState == EnemyStates.Idle)
            {
                myAnim.SetBool("Motion", false);
                myAnim.SetBool("Attacking", true);
            }
            else
            {
                myAnim.SetBool("Motion", true);
                myAnim.SetBool("Attacking", false);
            }
        }
    }
}
