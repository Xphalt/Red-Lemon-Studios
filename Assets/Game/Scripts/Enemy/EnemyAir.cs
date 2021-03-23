using System.Collections;
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

            else
            {
                movementState = EnemyStates.Idle;
                transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
            }
            Animate();
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
        if (base.Attack() && Physics.Raycast(transform.position, transform.forward))
        {
            shooter.Shoot(elementType, target.transform.position);
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
        base.Animate();
        Animator MyAnim = gameObject.GetComponent<Animator>();
        if (movementState == EnemyStates.Idle)
        {
            MyAnim.SetBool("Motion", false);
            MyAnim.SetBool("Attacking", false);
        }
        else if (movementState == EnemyStates.Chasing)
        {
            MyAnim.SetBool("Motion", true);
            MyAnim.SetBool("Attacking", true);
        }
        else if (movementState == EnemyStates.Patrolling)
        {
            MyAnim.SetBool("Motion", true);
            MyAnim.SetBool("Attacking", false);
        }
        else if (movementState == EnemyStates.Fleeing)
        {
            MyAnim.SetBool("Motion", false);
            MyAnim.SetBool("Attacking", true);
        }
    }
}
