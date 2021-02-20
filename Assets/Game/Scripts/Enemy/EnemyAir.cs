using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

//Raycast to player to check line of sight - Complete
//
//If can't see player, patrol through series of nodes
//Else
//  Check if player is in range
//  

public class EnemyAir : Enemy
{
    public float weaponRange;
    public int knockbackForce;
    public float knockBackDuration;

    public float stationaryZone;
    private float minWeaponRange;

    public override void Start()
    {
        base.Start();

        elementType = ElementTypes.Air;
        weakAgainst = ElementTypes.Earth;
        strongAgainst = ElementTypes.Water;

        minWeaponRange = weaponRange * (1 - stationaryZone);
    }

    public override void Update()
    {
        base.Update();

        float targetDistance = GetDistance();
        bool inAttackRange = targetDistance < weaponRange;

        if (targetDistance > weaponRange != targetDistance < minWeaponRange)
            ChasePlayer(inAttackRange);
        else moving.velocity = Vector3.zero;


        if (inAttackRange)
        {
            Attack();
        }
    }

    public override bool Attack()
    {
        if (base.Attack())
        {
            playerScript.TakeDamage(damage);

            if (!playerScript.movementLocked)
            {
                playerScript.Shift(((target.transform.position - transform.position).normalized * knockbackForce), knockBackDuration, true);
            }

        }

        return true;
    }
}
